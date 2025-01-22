using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum CharacterActionKey {
    attack,
    sprint,
}

public abstract class CharCtrl : MonoBehaviour, IAttackTarget, ICharStatusSubscriber, IReloadManager
{
    // list of codes which don't correspond to actual actions, and should be reset to ActionCode.none
    public readonly HashSet<ActionCode> NON_ACTIONABLE_CODES = new HashSet<ActionCode>{ActionCode.cancel_aim, ActionCode.cancel_reload, ActionCode.sprint};

    protected CharacterController controller;
    protected AttackController attack_controller;
    protected ICharacterStatus char_status;
    public Transform aim_target;
    public Transform crouch_target; // moves up and down when the character crouches

    public float last_attack_time { get; protected set; }
    public float rotation_degrees_per_second = 400;
    public float rotation_speed = 0.85f;
    public ActionCode current_action = ActionCode.none;

    public float reload_rate = 1f; // multiplies how fast weapons are reloaded; 1f is the weapons unmodified reload time, 0.5 takes twice as long, and 2f takes half as long
    public float reload_move_multiplier = 0.33f;
    public float walk_speed = 4.0f;
    public float sprint_multiplier = 1.75f;

    private AmmoContainer ammo_container;

    public float start_reload_at;

    public GameObject animatior_controller_ref;
    protected IAnimationFacade _animator_facade;
    private float hit_stun_until = -1f;
    
    private bool _is_active = true;
    private float _attack_paused_locked_for = 0f;
    private const float ATTACK_LOCK_AFTER_PAUSE = 0.75f; // how long will attacking be blocked after unpausing
    public bool is_active {
        get { return _is_active; }
        set {
            if (value && !_is_active) {
                // used to avoid instantly shooting when clicking out of a dialogue or menu
                _attack_paused_locked_for = ATTACK_LOCK_AFTER_PAUSE;
            }
            _is_active = value;
        }
    }
    public virtual float movement_speed {
        get {
            float move_speed = walk_speed;
            if (is_spinting) {
                move_speed *= sprint_multiplier;
            }

            if (reloading) {
                move_speed *= reload_move_multiplier;
            }

            if (aiming) {
                float aim_multiplier;
                if (current_weapon == null) {
                    aim_multiplier = 0.75f;
                } else {
                    aim_multiplier = current_weapon.aim_move_speed;
                }
                move_speed *= aim_multiplier;
            }

            if (this.is_hit_stunned) {
                move_speed *= 0.1f;
            }
            return move_speed;
        }
    }

    public virtual bool is_spinting {
        get {
            return this.current_action == ActionCode.sprint;
        }
    }

    private bool _reloading = false;
    public bool reloading {
        get { return _reloading; }
        protected set {
            _reloading = value;
            try {
                ((PlayerAttackController) attack_controller).switch_weapons_blocked = value;
            } catch (InvalidCastException) {
                // do nothing
            }
        }
    }
    private bool _aiming = false;
    public bool aiming {
        get { return _aiming; }
        protected set {
            _aiming = value;
            if (_aiming) {
                attack_controller.StartAim();
            } else {
                attack_controller.StopAim();
            }
        }
    }

    public float reload_time {
        get {
            if (current_weapon == null) {
                return 0f;
            }
            return current_weapon.reload_time;
        }
    }

    protected IWeapon current_weapon {
        get {
            return attack_controller.current_weapon;
        }
    }

    // percent (0f - 1f) progress on reload completion
    public float reload_progress {
        get {
            if (! reloading) { return 0f; }
            float progress_seconds = (Time.time - start_reload_at) * reload_rate;
            return progress_seconds / reload_time;
        }
    }

    public bool is_hit_stunned { get { return hit_stun_until > Time.time; }}

    ////////// debug fields /////////
    public ActionCode debug_action_input = ActionCode.none;
    public bool debug_attack_input, debug_can_attack, debug_weapon_isnull, debug_full_auto, debug_reloading, debug_sprint_input, debug_pause_blocked;
    public int debug_current_ammo;
    public float debug_movement_speed, debug_reload_progress, debug_inaccuracy, debug_recoil, debug_aim_percent, debug_pause_blocked_for;

    protected virtual void SetDebugData() {
        debug_movement_speed = this.movement_speed;
        debug_attack_input = AttackInput();
        debug_can_attack = CanAttack();
        debug_weapon_isnull = current_weapon == null;
        if (!debug_weapon_isnull) {
            debug_full_auto = current_weapon.auto_fire;
            debug_current_ammo = current_weapon.current_ammo;
        } else {
            debug_full_auto = false;
            debug_current_ammo = -1;
        }
        debug_reloading = reloading;
        debug_reload_progress = reload_progress;
        debug_inaccuracy = attack_controller.current_inaccuracy;
        debug_recoil = attack_controller.current_recoil;
        debug_aim_percent = attack_controller.aim_percent;
        debug_action_input = GetActionInput();
        debug_sprint_input = SprintInput();
        debug_pause_blocked_for = _attack_paused_locked_for;
        debug_pause_blocked = AttackPauseLocked();
    }
    /////////////////////////////////

    void Start() {
        SetupCharacter();
    }
    public virtual void SetupCharacter() {
        char_status = GetComponent<CharacterStatus>();
        char_status.Subscribe(this);
        controller = GetComponent<CharacterController>();
        attack_controller = GetComponent<AttackController>();
        ammo_container = GetComponent<AmmoContainer>();
        if (animatior_controller_ref != null) {
            _animator_facade = animatior_controller_ref.GetComponent<IAnimationFacade>();
        } else {
            _animator_facade = null;
            Debug.LogWarning($"{gameObject.name} initialized animator facade to null!");
        }
    }

    // Update is called once per frame
    void Update()
    {   
        if (! is_active) { return; } // do nothing while controller disabled
        PreUpdate();
        SetAction();
        Move();
        TryToAttack();
        SetDebugData();
        UpdatePauseAttackLock();
        PostUpdate();
        HandleAnimation();
    }

    protected virtual void HandleAnimation() {
        if (_animator_facade == null) { 
            Debug.Log($"{gameObject.name} animator is null!");
            return; // no animation set, do nothing.
        }
        // model is rotated weird...
        _animator_facade.forward_direction = this.transform.right;
        _animator_facade.right_direction = -this.transform.forward;
        _animator_facade.move_velocity = MoveVector();
        _animator_facade.action = AnimationActionType.idle;
        _animator_facade.weapon_class = current_weapon.weapon_class;
        _animator_facade.aim_percent = attack_controller.aim_percent;
        _animator_facade.shot_at = last_attack_time;
        _animator_facade.crouch_percent = 0f;
        _animator_facade.crouch_dive = false;
        
    }

    protected virtual void PreUpdate() {
        // do nothing. Extension hook for subclasses.
    }

    protected virtual void PostUpdate() {
        // do nothing. Extension hook for subclasses.
    }

    protected void SetAction() {
        if (NON_ACTIONABLE_CODES.Contains(current_action)) {
            current_action = ActionCode.none;
        }
        ActionCode action_input = GetActionInput();
        if (reloading) {
            if (ReloadIsFinished()) {
                FinishReload();
            }
            else if (
                    action_input == ActionCode.cancel_reload || 
                    action_input == ActionCode.sprint ||  
                    action_input == ActionCode.aim 
            ) {
                CancelReload();
            }
        }

        else if (aiming) {
            if (action_input == ActionCode.cancel_aim || action_input == ActionCode.sprint) {
                aiming = false;
                current_action = ActionCode.none;
            }
        }

        if (current_action == ActionCode.none) {
            current_action = action_input;

            switch (current_action) {
                case ActionCode.none:
                    break;

                case ActionCode.reload:
                    if (CanReload()) {
                        StartReload();
                    } else {
                        // cannot reload, reset action code.
                        current_action = ActionCode.none;
                    }
                    break;

                case ActionCode.aim:
                    aiming = true;
                    break;
            }
        }
    }

    public virtual ActionCode GetActionInput() {
        if (ReloadInput()) {
            return ActionCode.reload;
        }
        else if (AimInput()) {
            return ActionCode.aim;
        }
        else if (CrouchInput()) {
            return ActionCode.crouch;
        }
        else if (SprintInput()) {
            return ActionCode.sprint;
        }
        else if (CancelReloadInput()) {
            return ActionCode.cancel_reload;
        }
        else if (CancelAimInput()) {
            return ActionCode.cancel_aim;
        }
        return ActionCode.none;
    }
    
    public virtual bool CrouchInput() { 
        return false;
    }

    public virtual bool AimInput() {
        return false;
    }

    public virtual bool CancelAimInput() {
        return false;
    }

    public virtual bool ReloadInput() {
        return false;
    }

    public virtual bool SprintInput() {
        return false;
    }

    public virtual bool CancelReloadInput() {
        return false;
    }

    public void StartReload() {
        Debug.LogWarning($"reloading {gameObject.name}");
        // initiate a reload
        reloading = true;
        start_reload_at = Time.time;
        UpdateStartReload(current_weapon);
    }

    public bool ReloadIsFinished() {
        if (!reloading) {
            return false;
        }
        return reload_progress >= 1f;
    }

    public void CancelReload() {
        // end reload before it's finished
        reloading = false;
        current_action = ActionCode.none;
        UpdateCancelReload(current_weapon);
    }
    
    private void FinishReload() {
        // complete a reload successfully.
        reloading = false;
        current_action = ActionCode.none;
        IWeapon wpn = current_weapon;

        if (ShouldReloadWithContainer(wpn)) {
            _ReloadFromContainer(wpn);
        }
        else {
            _ReloadGeneric(wpn);
        }
        attack_controller.UpdateSubscribers();
        UpdateFinishReload(current_weapon);

        // for weapons that reload single rounds, keep reloading
        if(wpn.current_ammo != wpn.ammo_capacity) {
            // if you have infinite ammo, OR if there is at least 1 bullet left, keep reloading
            if (CanReload(wpn.ammo_type)) {
                StartReload();
            } 
        }
    }
    public bool CanReload() {
        // returns if the character can reload with the current_weapon.
        IWeapon current = current_weapon;
        if (current == null) { 
            Debug.LogWarning("cannot reload, current weapon is null!");
            return false; 
        }
        return CanReload(current);
    }
    public bool CanReload(IWeapon weapon) {
        // cannot reload full weapon
        if (weapon.ammo_capacity == weapon.current_ammo) { return false; }
        return CanReload(weapon.ammo_type);
    }

    public bool CanReload(AmmoType type) {
        // returns if the character is actually able to reload with the given ammo type
        return !ShouldReloadWithContainer(type) || ammo_container.GetCount(type) > 0;
    }

    public bool ShouldReloadWithContainer(IWeapon weapon) {
        // returns True if the character should reload from a container,
        // or False if the character should reload with infinite ammo
        return ShouldReloadWithContainer(weapon.ammo_type);
    }

    public bool ShouldReloadWithContainer(AmmoType type) {
        return (ammo_container != null) && (ammo_container.HasAmmoType(type));
    }

    private void _ReloadFromContainer(IWeapon weapon) {
        // reloads a weapon, limitted by the reserves in an AmmoContainer

        int availible_ammo = ammo_container.GetCount(weapon.ammo_type);
        int ammo_needed = Math.Min(weapon.reload_amount, (weapon.ammo_capacity - weapon.current_ammo));
        int reloaded_amount;
        if (availible_ammo >= ammo_needed) {
            reloaded_amount = ammo_needed;
        }
        else {
            reloaded_amount = availible_ammo;
        }
        weapon.current_ammo += reloaded_amount;
        ammo_container.UseAmmo(weapon.ammo_type, reloaded_amount);
    }

    private void _ReloadGeneric(IWeapon weapon) {
        // reloads with an infinite reserve of ammo
        weapon.current_ammo += weapon.reload_amount;
        if (weapon.current_ammo > weapon.ammo_capacity) {
            weapon.current_ammo = weapon.ammo_capacity;
        }
    }

    private bool CanAttack() {
        return !is_hit_stunned && !reloading && !is_spinting && !AttackPauseLocked();
    }

    private void UpdatePauseAttackLock() {
        _attack_paused_locked_for -= Time.deltaTime;
        if (_attack_paused_locked_for <= 0f) {
            _attack_paused_locked_for = 0f;
        }
    }

    public bool AttackPauseLocked() {
        // returns whether attack input is locked because of pause.
        // attacking locks while the game is paused, AND for a short time after
        if (Time.timeScale == 0f) {
            return true;
        }
        return _attack_paused_locked_for > 0f;
    }

    private void TryToAttack() {
        if (AttackInput() && CanAttack()) {
            if (current_action == ActionCode.cancel_reload) {
                // shooting can be used to cancel reload. in that case, cancel reload instead of shooting
                // cannot shoot the same frame you cancel reload
                return; 
            }
            PerformAttack();
        }
    }

    protected virtual void PerformAttack() {
        last_attack_time = Time.time;
        attack_controller.FireAttack(ShootVector());
    }
    
    public abstract Vector3 MoveDirection();
    public abstract Vector3 MoveVector();
    protected abstract void Move();

    protected void LookWithAction() {
        Vector3 forward; 
            forward = LookVector();
        LookRotate(forward);
    }

    private void LookRotate(Vector3 forward) {
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        Quaternion target_rot = Quaternion.AngleAxis(angle - 90, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, target_rot, rotation_speed);
    }
 
    protected virtual Vector3 LookVector() {
        // Returns a vector of the direction to look in
        return VectorFromLookTarget(LookTarget());
    }

    protected Vector3 VectorFromLookTarget(Vector3 look_target) {
        Vector3 v = look_target - transform.position;
        Vector3 forward = new Vector3(
            v.x, 0, v.z
        );
        return forward;
    }

    public abstract Vector3 LookTarget();  // Vector3 position to look at.

    public virtual Vector3 ShootVector() {
        return LookVector();
    }

    public virtual bool AttackInput() {
        // Indicates that a main attack should be made this frame.
        return false;
    }

    public void OnAttackHitDealt(IAttack attack, IAttackTarget target) {
        // TODO
        // Debug.Log($"{this} scored a hit with attack {attack}!");
    }

    public void OnAttackHitRecieved(IAttack attack) {
        // char_status.GetAttackTarget().OnAttackHitRecieved(attack, attacker);
        // Debug.Log($"{this} was hit by an attack {attack}!");
        SetHitStun(attack);
        if (_animator_facade != null) {
            _animator_facade.hurt_at = Time.time;
        }
    }     

    public void SetHitStun(IAttack attack) {
        hit_stun_until = Time.time + 0.25f;
    }

    public void StatusUpdated(ICharacterStatus status) {
        // if (char_status.health <= 0) {
        //     if (times_killed == 0) {
        //         // prevent CharacterDeath from being called multiple times in shotgun deaths
        //         CharacterDeath();
        //     }
        //     times_killed += 1;
        // }

        // do nothing?
    }

    
    public virtual void OnDeath(ICharacterStatus status) {
        // triggers immediately on death
        Debug.Log($"{gameObject.name} has been killed!");
        _is_active = false;
        CancelReload();
        if (_animator_facade != null) {
            _animator_facade.is_killed = true;
        }
    } 
    public virtual void DelayedOnDeath(ICharacterStatus status) {
        // triggers after a death animation finishes playing
        Debug.Log($"DelayedOnDeath: {gameObject.name}");
    } 
    public virtual void OnDeathCleanup(ICharacterStatus status) {
        // triggers some time after death to despawn the character
        Debug.Log($"OnDeathCleanup: {gameObject.name}");
    } 

    public ICharacterStatus GetStatus() {
        if (char_status == null) {
            char_status = GetComponent<CharacterStatus>();
        }
        return this.char_status;
    }

    public GameObject GetHitTarget() {
        return this.gameObject;
    }

    public Transform GetAimTarget() {
        if (aim_target == null) { 
            Debug.LogWarning($"{this.gameObject.name} using implicit aim_target!");
            return transform; 
        }
        return aim_target;
    }
    
    public Transform GetCrouchTarget() {
        // gets the transform that needs to be adjusted for crouching
        if (crouch_target == null) {
            Debug.LogWarning($"{this.gameObject.name} using implicit crouch_target!");
            return transform;
        }
        return crouch_target;
    }

    private List<IReloadSubscriber> _reload_subscribers = new List<IReloadSubscriber>();
    public void UpdateStartReload(IWeapon weapon) {
        foreach (IReloadSubscriber sub in _reload_subscribers) {
            sub.StartReload(this, weapon);
        }
    }
    public void UpdateFinishReload(IWeapon weapon) {
        foreach (IReloadSubscriber sub in _reload_subscribers) {
            sub.FinishReload(this, weapon);
        }
    }
    public void UpdateCancelReload(IWeapon weapon) {
        foreach (IReloadSubscriber sub in _reload_subscribers) {
            sub.CancelReload(this, weapon);
        }
    }
    public void Subscribe(IReloadSubscriber sub) => _reload_subscribers.Add(sub);
    public void Unsubscribe(IReloadSubscriber sub) => _reload_subscribers.Remove(sub);
}


public enum ActionCode {
    // list of action choices returned by subclasses to pick action
    none,
    sprint,
    crouch,
    reload,
    cancel_reload,
    aim,
    cancel_aim

}