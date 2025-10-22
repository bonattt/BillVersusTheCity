using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum CharacterActionKey {
    attack,
    sprint,
}

public abstract class CharCtrl : MonoBehaviour, IAttackTarget, ICharStatusSubscriber, IReloadManager, ICharacterMovement, ISettingsObserver {
    // list of codes which don't correspond to actual actions, and should be reset to ActionCode.none
    public readonly HashSet<ActionCode> NON_ACTIONABLE_CODES = new HashSet<ActionCode> { ActionCode.cancel_aim, ActionCode.cancel_reload, ActionCode.sprint };

    protected CharacterController controller;
    private IAttackController _attack_controller;
    public IAttackController attack_controller {
        get {
            if (_attack_controller == null) {
                _attack_controller = GetComponent<IAttackController>();
            }
            return _attack_controller;
        }
    }
    protected ICharacterStatus char_status;
    public Transform aim_target;
    public Transform crouch_target; // moves up and down when the character crouches
    public VaultingAreaDetector vaulting_area_detector;

    public float last_attack_time { get; set; }
    public bool attack_this_frame { get; private set; }
    public bool attack_last_frame { get; private set; }
    public float rotation_degrees_per_second = 400;
    public float rotation_speed = 0.85f;
    public ActionCode current_action = ActionCode.none;

    public float reload_rate = 1f; // multiplies how fast weapons are reloaded; 1f is the weapons unmodified reload time, 0.5 takes twice as long, and 2f takes half as long
    public float reload_move_multiplier = 0.33f;
    public float walk_speed = 4.0f;
    [SerializeField]
    private float _sprint_multiplier = 1.75f;
    public float sprint_multiplier {
        get {
            float difficulty_multiplier;
            if (is_player) {
                difficulty_multiplier = 1f; // player move speed not adjustable as difficulty setting
            } else {
                difficulty_multiplier = GameSettings.inst.difficulty_settings.GetMultiplier(DifficultySettings.ENEMY_RUN_SPEED);
            }
            return _sprint_multiplier * difficulty_multiplier;
        }
    }
    public float crouched_speed = 0.25f;
    public float crouch_rate = 4f;
    public float uncrouch_rate = 4f;
    public float crouch_height = 0.5f;
    public float uncrouched_height = 1.1f;

    [Tooltip("if true, the character will reload again if a reload is completed and the gun is not full.")]
    public bool keep_reloading = true;

    private AmmoContainer ammo_container;

    public float start_reload_at;

    public GameObject animatior_controller_ref;
    protected IAnimationFacade _animator_facade;
    private float hit_stun_until = -1f;

    public virtual bool is_player { get { return false; } }
    private bool _is_alive = true;
    public bool is_alive { get { return _is_alive; } }
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
            if (is_sprinting) {
                move_speed *= sprint_multiplier;
            }
            if (reloading) {
                move_speed *= reload_move_multiplier;
            }
            if (aiming) {
                float aim_multiplier;
                if (current_firearm == null) {
                    aim_multiplier = 0.75f;
                } else {
                    aim_multiplier = current_firearm.aim_move_speed;
                }
                move_speed *= aim_multiplier;
            }

            if (this.is_hit_stunned) {
                move_speed *= 0.1f;
            }
            if (is_vaulting) {
                move_speed = vault_over_speed;
            }
            return move_speed;
        }
    }

    public float crouch_dive_duration = 1f; // how long does a crouch dive last
    public float crouch_lock_duration = 1f; // how long does a player remain crouched after a crouch dive
    protected Vector3 crouch_dive_direction = new Vector3(0, 0, 0);

    protected Vector3 vault_over_direction = new Vector3(0, 0, 0);
    protected float vault_over_remaining = 0f; // seconds remaining in the current vault over an obstacle 

    [Tooltip("How many seconds are left in the current crouch dive.")]
    [SerializeField] protected float crouch_dive_remaining = 0f; // 
    
    [Tooltip("How many seconds are left until the player can stand up after a crouch dive.")]
    [SerializeField] protected float crouch_locked_remaining = 0f; // 

    public virtual bool is_sprinting { get; protected set; }

    private bool _reloading = false;
    public bool reloading {
        get { return _reloading; }
        protected set {
            _reloading = value;
            try {
                ((PlayerAttackController)attack_controller).switch_weapons_blocked = value;
            } catch (InvalidCastException) {
                // do nothing
            }
        }
    }
    private bool _aiming = false;
    public bool aiming {
        get { return _aiming; }
        set {
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
            if (current_firearm == null) {
                return 0f;
            }
            return current_firearm.reload_time;
        }
    }

    public virtual IFirearm current_firearm {
        get {
            return attack_controller.current_gun;
        }
        set {
            attack_controller.current_gun = value;
        }

    }

    // percent (0f - 1f) progress on reload completion
    public float reload_progress {
        get {
            if (!reloading) { return 0f; }
            float progress_seconds = (Time.time - start_reload_at) * reload_rate * _reload_speed_from_difficulty;
            return progress_seconds / reload_time;
        }
    }

    private float _reload_speed_from_difficulty {
        get {
            DifficultySettings settings = GameSettings.inst.difficulty_settings;
            if (is_player) {
                return settings.GetMultiplier(DifficultySettings.PLAYER_RELOAD_SPEED);
            } else {
                return settings.GetMultiplier(DifficultySettings.ENEMY_RELOAD_SPEED);
            }
        }
    }

    public bool is_hit_stunned { get { return hit_stun_until > Time.time; } }

    public bool combat_enabled {
        get {
            if (LevelConfig.inst == null) { return false; }
            return LevelConfig.inst.combat_enabled;
        }
    }

    // ////////// debug fields /////////
    // public ActionCode debug_action_input = ActionCode.none;
    // public bool debug_attack_input, debug_can_attack, debug_weapon_isnull, debug_full_auto, debug_reloading, debug_sprint_input, debug_pause_blocked;
    // public int debug_current_ammo;
    // public float debug_movement_speed, debug_reload_progress, debug_inaccuracy, debug_recoil, debug_aim_percent, debug_pause_blocked_for;
    // public Vector3 debug_move_dir;

    // protected virtual void SetDebugData() {
    //     debug_move_dir = MoveDirection();
    //     debug_movement_speed = this.movement_speed;
    //     debug_attack_input = AttackInput();
    //     debug_can_attack = CanAttack();
    //     debug_weapon_isnull = current_weapon == null;
    //     if (!debug_weapon_isnull) {
    //         debug_full_auto = current_weapon.auto_fire;
    //         debug_current_ammo = current_weapon.current_ammo;
    //     } else {
    //         debug_full_auto = false;
    //         debug_current_ammo = -1;
    //     }
    //     debug_reloading = reloading;
    //     debug_reload_progress = reload_progress;
    //     debug_inaccuracy = attack_controller.current_inaccuracy;
    //     debug_recoil = attack_controller.current_recoil;
    //     debug_aim_percent = attack_controller.aim_percent;
    //     debug_action_input = GetActionInput();
    //     debug_sprint_input = SprintInput();
    //     debug_pause_blocked_for = _attack_paused_locked_for;
    //     debug_pause_blocked = AttackPauseLocked();
    // }
    // /////////////////////////////////

    protected virtual void Start() {
        last_attack_time = float.NegativeInfinity;
        char_status = GetComponent<CharacterStatus>();
        char_status.Subscribe(this);
        controller = GetComponent<CharacterController>();
        ammo_container = GetComponent<AmmoContainer>();
        if (animatior_controller_ref != null) {
            _animator_facade = animatior_controller_ref.GetComponent<IAnimationFacade>();
        } else {
            _animator_facade = null;
            Debug.LogWarning($"{gameObject.name} initialized animator facade to null!");
        }
    }

    void LateUpdate() {
        attack_last_frame = attack_this_frame;
        attack_this_frame = false;
        UpdateDebug();
    }

    // // Update is called once per frame
    // void Update()
    // {   
    //     if (! is_active) { return; } // do nothing while controller disabled
    //     SetAction();
    //     Move();
    //     TryToAttack();
    //     SetDebugData();
    //     UpdatePauseAttackLock();
    //     PostUpdate();
    //     HandleAnimation();
    // }

    protected void UpdateReload() {
        if (reloading && IsReloadFinished()) {
            FinishReload();
        }
    }

    protected virtual void HandleAnimation() {
        if (_animator_facade == null) {
            Debug.Log($"{gameObject.name} animator is null!");
            return; // no animation set, do nothing.
        }
        // model is rotated weird...
        _animator_facade.forward_direction = this.transform.forward;
        _animator_facade.right_direction = this.transform.right;
        _animator_facade.move_velocity = GetVelocity();
        _animator_facade.action = AnimationActionType.idle;
        _animator_facade.weapon_class = current_firearm != null ? current_firearm.weapon_class : WeaponClass.empty;
        _animator_facade.aim_percent = attack_controller.aim_percent;
        _animator_facade.shot_at = last_attack_time;
        _animator_facade.crouch_percent = 0f;
        _animator_facade.crouch_dive = false;
        _animator_facade.is_sprinting = this.is_sprinting;
        _animator_facade.is_reloading = this.reloading;
        _animator_facade.is_vaulting = this.is_vaulting;

    }

    public void SetCharacterLookDirection(Vector3 look_direction) {
        Quaternion desired_rotation = Quaternion.LookRotation(look_direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desired_rotation, rotation_speed);
    }

    protected virtual void PostUpdate() {
        // TODO --- investigate if this can be removed
        // do nothing. Extension hook for subclasses.
    }


    public void StartReload() {
        // initiate a reload
        if (!CanReload()) {
            MenuManager.PlayMenuCancelClick(); // TODO --- placeholder
            return;
        }
        reloading = true;
        start_reload_at = Time.time;
        UpdateReloadStarted(current_firearm);
    }

    public bool IsReloadFinished() {
        if (!reloading) {
            return false;
        }
        return reload_progress >= 1f;
    }

    public void CancelReload() {
        // end reload before it's finished
        if (reloading) {
            reloading = false;
            current_action = ActionCode.none;
            UpdateReloadCancelled(current_firearm);
        } 
    }

    private void FinishReload() {
        // complete a reload successfully.
        reloading = false;
        current_action = ActionCode.none;
        IFirearm wpn = current_firearm;

        if (ShouldReloadWithContainer(wpn)) {
            _ReloadFromContainer(wpn);
        } else {
            _ReloadGeneric(wpn);
        }

        try {
            ((IWeaponManager)attack_controller).UpdateSubscribers();
        } catch (InvalidCastException) {
            // do nothing
        }
        UpdateReloadFinished(current_firearm);

        // for weapons that reload single rounds, keep reloading
        if (keep_reloading && wpn.current_ammo < wpn.ammo_capacity) {
            // if you have infinite ammo, OR if there is at least 1 bullet left, keep reloading
            if (CanReload(wpn)) {
                StartReload();
            }
        }
    }

    public float _crouch_percent = 0f;
    public float crouch_percent {
        get { return _crouch_percent; }
        set {
            _crouch_percent = value;
            if (_crouch_percent >= 1f) {
                _crouch_percent = 1f;
            } else if (_crouch_percent <= 0f) {
                _crouch_percent = 0f;
                if (current_action == ActionCode.crouch) {
                    current_action = ActionCode.none;
                }
            }
        }
    }

    protected virtual void UpdateCrouch(bool crouch) {
        if (crouch_locked_remaining > 0) {
            crouch = true;
            crouch_locked_remaining -= Time.deltaTime;
        }
        if (crouch_dive_remaining > 0) {
            crouch_dive_remaining -= Time.deltaTime;
            crouch_percent = 1f;
        } else if (vault_over_remaining > 0) {
            vault_over_remaining -= Time.deltaTime;
            if (vault_over_remaining <= 0f) {
                FinishVaultOver();
            }
        } else if (crouch) {
            if (crouch_locked_remaining > 0) { crouch_locked_remaining -= Time.deltaTime; }
            crouch_percent += crouch_rate * Time.deltaTime;
        } else {
            crouch_percent -= uncrouch_rate * Time.deltaTime;
        }
        float current_height = (crouch_height * crouch_percent) + (uncrouched_height * (1 - crouch_percent));
        crouch_target.position = new Vector3(crouch_target.position.x, current_height, crouch_target.position.z);
    }
    public bool GetStartCrouchDiveThisFrame(bool crouch, Vector3 move_direction) {
        // returns true if a crouch dive should start, AND isn't already started
        // move_direction = new Vector3(move_direction.x, 0, move_direction.y); // don't crouch dive from gravity...
        Debug.LogWarning($"crouch: {crouch} && move_direction: {move_direction}, _crouch_last_frame: {_crouch_last_frame} && crouch_dive_remaining: {crouch_dive_remaining} ==> {crouch && move_direction != Vector3.zero && !_crouch_last_frame && crouch_dive_remaining <= 0}"); // TODO --- remove debug
        return crouch && move_direction != Vector3.zero && !_crouch_last_frame && crouch_dive_remaining <= 0;
    }

    protected virtual void StartCrouchDive(Vector3 move_direction) {
        crouch_dive_remaining = crouch_dive_duration;
        crouch_dive_direction = move_direction.normalized;
        crouch_percent = 1f;
        PlayCrouchDiveEffects();
    }

    public bool GetStartVaultThisFrame(Vector3 move_direction, Vector3 look_direction) {
        // move_direction = new Vector3(move_direction.x, 0, move_direction.z);
        // if (! || crouch_percent != 0 || is_vaulting || move_direction == Vector3.zero) { return false; }

        // VaultOverCoverZone zone = vaulting_area_detector.GetVaultOverCoverZone();
        // if (zone == null) { return false; }
        // Vector3 direction = move_direction != Vector3.zero ? move_direction : look_direction;
        // return zone.IsInJumpPosition(transform.position, direction);
        return InputSystem.inst.VaultOverInput() && GetCouldStartVaultThisFrame(move_direction, look_direction);
    }

    public bool GetCouldStartVaultThisFrame(Vector3 move_direction, Vector3 look_direction) {
        // returns if a crouch input would trigger a jump, without actually checking for the input
        move_direction = new Vector3(move_direction.x, 0, move_direction.z);
        if (crouch_percent != 0 || is_vaulting || move_direction == Vector3.zero) { return false; }
        
        VaultOverCoverZone zone = vaulting_area_detector.GetVaultOverCoverZone();
        if (zone == null) { return false; }
        
        Vector3 direction = move_direction != Vector3.zero ? move_direction : look_direction;
        return zone.IsInJumpPosition(transform.position, direction);
    }

    private float vault_over_margin = 1.1f; // multiplies the length of a jump to ensure you clear the obstacle
    protected float vault_over_speed { get => walk_speed / 3f; }
    protected virtual void StartVaultOver(Vector3 move_direction, Vector3 look_direction) {
        VaultOverCoverZone zone = vaulting_area_detector.GetVaultOverCoverZone();
        float vault_duration = vault_over_margin * zone.jump_length / vault_over_speed;
        TeleportTo(transform.position + new Vector3(0f, zone.jump_height, 0f));

        vault_over_remaining = vault_duration;
        Vector3 direction = move_direction != Vector3.zero ? move_direction : look_direction;
        vault_over_direction = zone.GetVaultDirection(direction);
        Debug.DrawRay(transform.position, vault_over_direction, Color.yellow, 1.5f);
        PlayVaultOverEffects();
    }

    public virtual void PlayCrouchDiveEffects() {
        // do nothing by default
    }

    public virtual void PlayVaultOverEffects() {
        // do nothing by default
    }

    public virtual void TeleportTo(Vector3 position) {
        transform.position = position;
    }

    protected virtual void FinishVaultOver() {
        vault_over_remaining = -1f;
        vault_over_direction = Vector3.zero;

        transform.position += new Vector3(transform.position.x, 0f, transform.position.z);
    }

    protected bool _crouch_last_frame = false;

    // returns true if the player is leaping over cover/obstacle
    protected bool is_vaulting {
        get => vault_over_remaining > 0f && vault_over_direction != Vector3.zero;
    }
    protected bool is_crouch_diving {
        get => crouch_dive_remaining > 0f && crouch_dive_direction != Vector3.zero;
    }
    public virtual void MoveCharacter(Vector3 move_direction, Vector3 look_direction, bool sprint = false, bool crouch = false, bool walk = false) {
        bool is_moving = move_direction.magnitude > 0;
        is_sprinting = sprint && CanSprint() && is_moving;
        if (crouch_locked_remaining > 0) { crouch = true; }
        if (is_crouch_diving) {
            // if crouch diving, continue in that direction for the duration of the crouch dive
            move_direction = crouch_dive_direction;
            crouch_locked_remaining = crouch_lock_duration + crouch_dive_duration;
        } else if (is_vaulting) {
            move_direction = vault_over_direction.normalized;
        } else if (GetStartVaultThisFrame(move_direction, look_direction)) {
            StartVaultOver(move_direction, look_direction);
        } else if (GetStartCrouchDiveThisFrame(crouch, move_direction)) {
            StartCrouchDive(move_direction);
        } else {
            if (crouch) {
                // cannot crouch and sprint at the same time, if there is no crouch dive
                is_sprinting = false;
            }
        }
        if (is_crouch_diving || is_vaulting || is_sprinting) {
            // always face forward during crouch dive or sprint!
            look_direction = move_direction;
        }
        UpdateCrouch(crouch);
        SetCharacterLookDirection(look_direction);
        _crouch_last_frame = crouch;
    }

    public virtual bool CanSprint() {
        return true;
    }

    public bool CanReload() {
        // returns if the character can reload with the current_weapon.
        IFirearm current = current_firearm;
        if (current == null) {
            // Debug.LogWarning("cannot reload, current weapon is null!");
            return false;
        }
        return CanReload(current);
    }
    public bool CanReload(IFirearm weapon) {
        // cannot reload full weapon
        if (
                weapon.ammo_capacity == weapon.current_ammo 
                || weapon.reload_amount <= 0 
                || weapon.ammo_capacity == weapon.current_ammo
        ) { 
            return false; 
        }
        return HasAmmoToReload(weapon.ammo_type);
    }

    public bool HasAmmoToReload(AmmoType type) {
        // returns if the character is actually able to reload with the given ammo type
        return !ShouldReloadWithContainer(type) || ammo_container.GetCount(type) > 0;
    }

    public bool ShouldReloadWithContainer(IFirearm weapon) {
        // returns True if the character should reload from a container,
        // or False if the character should reload with infinite ammo
        return ShouldReloadWithContainer(weapon.ammo_type);
    }

    public bool ShouldReloadWithContainer(AmmoType type) {
        return (ammo_container != null) && ammo_container.HasAmmoType(type) && !GameSettings.inst.debug_settings.GetBool("infinte_ammo_supply");
    }

    private void _ReloadFromContainer(IFirearm weapon) {
        // reloads a weapon, limitted by the reserves in an AmmoContainer

        int availible_ammo = ammo_container.GetCount(weapon.ammo_type);
        int ammo_needed = Math.Min(weapon.reload_amount, (weapon.ammo_capacity - weapon.current_ammo));
        int reloaded_amount;
        if (availible_ammo >= ammo_needed) {
            reloaded_amount = ammo_needed;
        } else {
            reloaded_amount = availible_ammo;
        }
        weapon.current_ammo += reloaded_amount;
        ammo_container.UseAmmo(weapon.ammo_type, reloaded_amount);
    }

    private void _ReloadGeneric(IFirearm weapon) {
        // reloads with an infinite reserve of ammo
        weapon.current_ammo += weapon.reload_amount;
        if (weapon.current_ammo > weapon.ammo_capacity) {
            weapon.current_ammo = weapon.ammo_capacity;
        }
    }

    public bool CanAttack(bool hold = false) {
        return !is_hit_stunned && !reloading && !is_sprinting && !AttackPauseLocked() && combat_enabled;
    }

    protected void UpdatePauseAttackLock() {
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

    public void AttackReleased() {
        attack_controller.AttackReleased(GetShootVector());
    }

    public bool TryToAttack(bool hold = false) {
        if (CanAttack(hold)) {
            // if (current_action == ActionCode.cancel_reload) {
            //     // shooting can be used to cancel reload. in that case, cancel reload instead of shooting
            //     // cannot shoot the same frame you cancel reload
            //     return; 
            // }
            PerformAttack(hold);
            return true;
        }
        return false;
    }

    protected virtual void PerformAttack(bool hold = false) {
        attack_this_frame = true;
        last_attack_time = Time.time;
        if (hold) {
            attack_controller.AttackHold(GetShootVector());
        } else {
            attack_controller.StartAttack(GetShootVector());
        }
    }

    // public abstract Vector3 MoveDirection();
    public abstract Vector3 GetVelocity();
    // public abstract void Move(bool sprint=false);

    // protected void LookWithAction() {
    //     Vector3 forward; 
    //         forward = GetLookVector();
    //     LookRotate(forward);
    // }

    // private void LookRotate(Vector3 forward) {
    //     float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
    //     Quaternion target_rot = Quaternion.AngleAxis(angle - 90, Vector3.up);
    //     transform.rotation = Quaternion.Slerp(transform.rotation, target_rot, rotation_speed);
    // }

    protected Vector3 GetLookVector() {
        return transform.forward;
    }
    //  {
    //     // Returns a vector of the direction to look in
    //     return DirectionFromLookTarget(GetLookTarget());
    // }

    public Vector3 DirectionFromLookTarget(Vector3 look_target) {
        Vector3 v = look_target - transform.position;
        Vector3 forward = new Vector3(
            v.x, 0, v.z
        );
        return forward;
    }

    // public abstract Vector3 GetLookTarget();  // Vector3 position to look at.

    public virtual Vector3 GetShootVector() {
        return GetLookVector();
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

    public void OnDamage(ICharacterStatus status) {
        // do nothing
    }
    public void OnHeal(ICharacterStatus status) {
        // do nothing
    }


    public virtual void OnDeath(ICharacterStatus status) {
        // triggers immediately on death
        _is_alive = false;
        _is_active = false;
        CancelReload();
        if (_animator_facade != null) {
            _animator_facade.is_killed = true;
        }
    }
    public virtual void DelayedOnDeath(ICharacterStatus status) {
        // triggers after a death animation finishes playing
    }
    public virtual void OnDeathCleanup(ICharacterStatus status) {
        // triggers some time after death to despawn the character
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
    public void UpdateReloadStarted(IFirearm weapon) {
        foreach (IReloadSubscriber sub in _reload_subscribers) {
            sub.StartReload(this, weapon);
        }
    }
    public void UpdateReloadFinished(IFirearm weapon) {
        foreach (IReloadSubscriber sub in _reload_subscribers) {
            sub.ReloadFinished(this, weapon);
        }
    }
    public void UpdateReloadCancelled(IFirearm weapon) {
        foreach (IReloadSubscriber sub in _reload_subscribers) {
            sub.ReloadCancelled(this, weapon);
        }
    }
    public void Subscribe(IReloadSubscriber sub) => _reload_subscribers.Add(sub);
    public void Unsubscribe(IReloadSubscriber sub) => _reload_subscribers.Remove(sub);


    public void SettingsUpdated(ISettingsModule updated, string field) {
        switch (updated) {
            case DifficultySettings settings:
                Debug.LogWarning("TODO --- update difficulty"); // TODO --- remove debug
                break;

            default:
                Debug.LogWarning($"unexpected settings module update event from '{updated}', on field '{field}'!");
                break;
        }
    }

    //////////////////// DEBUG 
    [Tooltip("Read-only class data output for debugging with the inspector for the CharCtrl abstract class.")]
    public CharacterControllerDebugInfo debug = new CharacterControllerDebugInfo();
    public virtual void UpdateDebug() {
        debug.can_attack = CanAttack();
        debug.move_action_remaining = Mathf.Max(crouch_dive_remaining, vault_over_remaining);
        debug.move_action_dir = vault_over_direction;
        debug.is_vaulting = is_vaulting;
        debug.is_crouch_diving = is_crouch_diving;
        debug.move_speed = movement_speed;
        debug.vault_over_speed = vault_over_speed;
    }
}


public enum ActionCode {
    // list of action choices returned by subclasses to pick action
    none,
    sprint,
    crouch,
    reload,
    cancel_reload,
    aim,
    cancel_aim,
    dive,
    jump,
}


[Serializable]
public class CharacterControllerDebugInfo {
    [Tooltip("generic message, used for whatever is currently being debugged, or blank if not being used.")]
    public string message = "";
    public bool can_attack, is_vaulting, is_crouch_diving;
    public float move_action_remaining = -1f, move_speed, vault_over_speed;
    public Vector3 move_direction, move_action_dir;
}