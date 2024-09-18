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
    protected CharacterController controller;
    protected AttackController attack_controller;
    protected ICharacterStatus char_status;

    public float last_attack_time { get; protected set; }
    public float rotation_degrees_per_second = 400;
    public float rotation_speed = 0.85f;
    public ActionCode current_action = ActionCode.none;

    public float reload_move_speed = 0.33f;

    public float start_reload_at;
    private float hit_stun_until = -1f;


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

    public float reload_time {
        get {
            if (attack_controller.current_weapon == null) {
                return 0f;
            }
            return attack_controller.current_weapon.reload_time;
        }
    }

    // percent (0f - 1f) progress on reload completion
    public float reload_progress {
        get {
            if (! reloading) { return 0f; }
            float progress_seconds = Time.time - start_reload_at;
            return progress_seconds / reload_time;
        }
    }

    public bool is_hit_stunned { get { return hit_stun_until > Time.time; }}

    ////////// debug fields /////////
    public bool debug_attack_input;
    public bool debug_can_attack;
    public bool debug_weapon_isnull;
    public bool debug_full_auto;
    public int debug_current_ammo;
    public bool debug_reloading = false;

    public float debug_inaccuracy = 0f;
    public float debug_recoil = 0f;

    protected virtual void SetDebugData() {
        debug_attack_input = AttackInput();
        debug_can_attack = CanAttack();
        debug_weapon_isnull = attack_controller.current_weapon == null;
        if (!debug_weapon_isnull) {
            debug_full_auto = attack_controller.current_weapon.auto_fire;
            debug_current_ammo = attack_controller.current_weapon.current_ammo;
        } else {
            debug_full_auto = false;
            debug_current_ammo = -1;
        }
        debug_reloading = reloading;
        debug_inaccuracy = attack_controller.current_inaccuracy;
        debug_recoil = attack_controller.current_recoil;
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
    }

    // Update is called once per frame
    void Update()
    {   
        PreUpdate();
        SetAction();
        Move();
        TryToAttack();
        SetDebugData();
        PostUpdate();
    }

    protected virtual void PreUpdate() {
        // do nothing. Extension hook for subclasses.
    }

    protected virtual void PostUpdate() {
        // do nothing. Extension hook for subclasses.
    }

    protected void SetAction() {
        if (reloading) {
            if (ReloadIsFinished()) {
                FinishReload();
            }
            else if (GetActionInput() == ActionCode.cancel_reload) {
                CancelReload();
            }
        }

        else if (current_action == ActionCode.none) {
            current_action = GetActionInput();

            switch (current_action) {
                case ActionCode.none:
                    break;

                case ActionCode.reload:
                    StartReload();
                    break;
            }
        }
    }

    public virtual ActionCode GetActionInput() {
        if (ReloadInput()) {
            return ActionCode.reload;
        }
        else if (CancelReloadInput()) {
            return ActionCode.cancel_reload;
        }
        else if (SprintInput()) {
            return ActionCode.sprint;
        }
        return ActionCode.none;
    }

    public virtual bool ReloadInput() {
        return attack_controller.current_weapon.current_ammo == 0
            && !reloading
            && AttackInput();
    }

    public virtual bool SprintInput() {
        return false;
    }

    public virtual bool CancelReloadInput() {
        return false;
    }

    public void StartReload() {
        // initiate a reload
        reloading = true;
        start_reload_at = Time.time;
        UpdateStartReload(attack_controller.current_weapon);
    }

    public bool ReloadIsFinished() {
        if (!reloading) {
            return false;
        }
        return Time.time >= start_reload_at + this.reload_time;
    }

    public void CancelReload() {
        // end reload before it's finished
        reloading = false;
        current_action = ActionCode.none;
        UpdateCancelReload(attack_controller.current_weapon);
    }
    
    private void FinishReload() {
        // complete a reload successfully.
        reloading = false;
        current_action = ActionCode.none;
        IWeapon wpn = attack_controller.current_weapon;
        wpn.current_ammo += wpn.reload_amount;
        if (wpn.current_ammo > wpn.ammo_capacity) {
            wpn.current_ammo = wpn.ammo_capacity;
        }

        // for weapons that reload single rounds, keep reloading
        if(wpn.current_ammo 
                != wpn.ammo_capacity) {
            StartReload();
        }
        attack_controller.UpdateSubscribers();
        UpdateFinishReload(attack_controller.current_weapon);
    }

    private void TryToAttack() {
        if (!is_hit_stunned && AttackInput() && CanAttack() && !reloading) {
            PerformAttack();
        }
    }

    protected virtual void PerformAttack() {
        last_attack_time = Time.time;
        attack_controller.FireAttack(ShootVector());
    }
    
    // public void SetMovementAction(IMovementAction action) {
    //     this.movement_action = action;
    //     action.new_this_frame = true;
    //     action.remaining_duration = action.action_duration;
    // }

    protected abstract void Move();

    // private void MoveWithAction() {
    //     LookWithAction();
    //     movement_action.action_used_for += Time.deltaTime;
    //     Vector3 dir;
    //     if (movement_action.overrides_move_vector) {
    //         dir = movement_action.move_vector;   
    //     } else {
    //         dir = MoveVector();
    //     }
    //     dir *= movement_action.move_speed_multiplier;

    //     controller.SimpleMove(dir);
    // }

    // public bool MoveActionNameMatch(string name) {
    //     return movement_action != null 
    //         && movement_action.action_name.Equals(name);
    // }
    protected void LookWithAction() {
        // Handles adjusting the player character's rotation with hooks to
        // allow a movement_action to override the value
        Vector3 forward; 
        // if (movement_action == null) {
        //     forward = LookVector();
        // }
        // else if (movement_action.overrides_look_vector) {
        //     forward = movement_action.look_vector;
        // } 
        // else if (movement_action.overrides_look_target) {
        //     forward = VectorFromLookTarget(movement_action.look_target);
        // }
        // else {
            forward = LookVector();
        // }
        LookRotate(forward);
    }

    // private void LookNormal() {
    //     Vector3 forward = LookVector();
    //     LookRotate(forward);
    // }

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

    public virtual bool CanAttack() {
        // Indicates whether an attack can be started this frame;
        return true;
    }

    public abstract Vector3 MoveVector();

    public void OnAttackHitDealt(IAttack attack, IAttackTarget target) {
        // TODO
        Debug.Log($"{this} scored a hit with attack {attack}!");
    }

    public void OnAttackHitRecieved(IAttack attack) {
        // char_status.GetAttackTarget().OnAttackHitRecieved(attack, attacker);
        Debug.Log($"{this} was hit by an attack {attack}!");
        SetHitStun(attack);
    }     

    public void SetHitStun(IAttack attack) {
        hit_stun_until = Time.time + 0.25f;
    }

    private int times_killed = 0;
    public void StatusUpdated(ICharacterStatus status) {
        if (char_status.health <= 0) {
            if (times_killed == 0) {
                // prevent CharacterDeath from being called multiple times in shotgun deaths
                CharacterDeath();
            }
            times_killed += 1;
        }
    }

    protected virtual void CharacterDeath() {
        Debug.LogWarning($"Character '{gameObject.name}' has died!");
        // TODO ---
    }

    public ICharacterStatus GetStatus() {
        return this.char_status;
    }

    public GameObject GetHitTarget() {
        return this.gameObject;
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
    reload,
    cancel_reload

}