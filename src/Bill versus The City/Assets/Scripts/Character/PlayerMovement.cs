using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharCtrl
{
    // returns a list of all the nodes which enemies raycast to see the player
    public Transform vision_target;
    private List<Transform> _vision_nodes = null;
    public float crouch_dive_duration = 1f; // how long does a crouch dive last
    private Vector3 crouch_dive_direction = new Vector3(0, 0, 0);
    private float crouch_dive_remaining = 0f; // how long can you crouch dive for
    public List<Transform> vision_nodes {
        get {
            if (_vision_nodes == null) {
                _vision_nodes = new List<Transform>{transform};
                foreach (Transform child in vision_target) {
                    _vision_nodes.Add(child);
                }
            }
            return _vision_nodes;
        }
    }

    public override bool AttackInput() {
        if (current_weapon == null) { return false; }
        if (current_weapon.auto_fire) {
            return InputSystem.current.AttackHoldInput();
        } 
        return InputSystem.current.AttackClickInput();
    }

    public override bool ReloadInput() {
        if (this.reloading) {
            return false;
        }
        return InputSystem.current.ReloadInput();
    }

    public override bool SprintInput() {
        return InputSystem.current.SprintInput();
    }

    public override bool CancelReloadInput() {
        if (! this.reloading) {
            return false;
        }
        return InputSystem.current.ReloadInput() || InputSystem.current.AttackClickInput();
    }
    
    public override bool AimInput() {
        return InputSystem.current.AimAttackInput();
    }

    public override bool CrouchInput() {
        return InputSystem.current.CrouchInput();
    }

    public override bool CancelAimInput() {
        if (this.aiming) {
            return !AimInput();
        }
        return false;
    }

    public float _crouch_percent = 0f;
    public float crouch_percent {
        get { return _crouch_percent; }
        set {
            _crouch_percent = value;
            if (_crouch_percent >= 1f) { 
                _crouch_percent = 1f;
            }
            else if (_crouch_percent <= 0f) {
                _crouch_percent = 0f;
                if (current_action == ActionCode.crouch) {
                    current_action = ActionCode.none;
                }
            }
            
        }
    }
    public float crouch_height = 0.6f;
    public float crouched_speed = 0.25f;
    public float uncrouched_height = 1.4f;
    public float crouch_rate = 4f;
    public float uncrouch_rate = 4f;

    public override float movement_speed {
        get {
            if (crouch_dive_remaining > 0f) {
                return walk_speed * sprint_multiplier;
            }
            float move_speed = base.movement_speed;
            float crouch_multiplier = (1 - crouch_percent) + (this.crouched_speed * crouch_percent);
            if ((is_sprinting && crouch_percent >= 1f) || CrouchInput()) {
                // disallow sprinting while fully crouched, but not while standing up
                move_speed = move_speed / sprint_multiplier;
            }
            return move_speed * crouch_multiplier;
        }
    }

    private void HandleCrouch() {
        if (crouch_dive_remaining >= 0) { crouch_dive_remaining -= Time.deltaTime; }
        bool is_moving =  MoveVector() != new Vector3(0f, 0f, 0f);
        if (is_moving && CrouchInput() && SprintInput() && crouch_percent <= 0f && crouch_dive_remaining <= 0) { // crouch diving
            crouch_dive_direction = MoveVector();
            // WARNING: , crouch_dive_remaining must be set AFTER setting crouch_dive_direction, because it effects the output of MoveDirection()
            crouch_dive_remaining = crouch_dive_duration;  
            crouch_percent = 1;
        } else if (CrouchInput()) {  // is crouching
            crouch_percent += crouch_rate * Time.deltaTime;
        } 
        else {  // not crouching
            if (crouch_dive_remaining < 0) {
                // un-crouch, but only if you're not in a crouch dive
                crouch_percent -= uncrouch_rate * Time.deltaTime;
            }
        }
        Vector3 destination;
        Transform _crouch_target = GetCrouchTarget();
        if (crouch_percent == 1) {
            destination = new Vector3(_crouch_target.position.x, crouch_height, _crouch_target.position.z);
        } else if (crouch_percent == 0) {
            destination = new Vector3(_crouch_target.position.x, uncrouched_height, _crouch_target.position.z);
        }
        else {
            float y = (uncrouched_height * (1 - crouch_percent)) + (crouch_height * crouch_percent);
            destination = new Vector3(_crouch_target.position.x, y, _crouch_target.position.z);
        }
        _crouch_target.position = destination;
    }
    
    protected override void Move() {
        LookWithAction();
        HandleCrouch();
        controller.SimpleMove(MoveVector());
    }

    protected override void PostUpdate() {
        if (InputSystem.current.NextWeaponModeInput()) {
            attack_controller.current_weapon.NextWeaponSetting();
            if (attack_controller.current_weapon.HasWeaponSettings()) {
                ISoundSet sound = SFXLibrary.LoadSound("weapon_mode_switch");
                SFXSystem.inst.PlaySound(sound, transform.position);

            }
            attack_controller.UpdateSubscribers();
        }
    }

    // protected Vector3 ModifyMoveVector(Vector3 move) {
    //     return move * this.movement_speed;
    // }

    public override Vector3 LookTarget() {
        Vector3 mouse_pos = InputSystem.current.MouseWorldPosition();
        Vector3 look_target = new Vector3(mouse_pos.x, transform.position.y, mouse_pos.z);
        return look_target;
    }

    protected  override Vector3 LookVector() {
        if (is_sprinting || crouch_dive_remaining > 0) {
            return MoveDirection();
        } else {
            return base.LookVector();
        }
    }

    public override Vector3 MoveDirection() {
        Vector3 move;
        if (crouch_dive_remaining > 0f) {
            move = crouch_dive_direction;
        } else {
            // float move_x, move_y;
            float move_x = InputSystem.current.MoveXInput();
            float move_y = InputSystem.current.MoveYInput();
            move = new Vector3(move_x, 0, move_y);
        }        
        return move.normalized;
    }
    
    public override Vector3 MoveVector() {
        return MoveDirection() * movement_speed;
    }

    public override void DelayedOnDeath(ICharacterStatus status) {
        base.DelayedOnDeath(status);
        LevelConfig.inst.FailLevel();
    }
    
    protected override void HandleAnimation() {
        base.HandleAnimation();
        _animator_facade.crouch_percent = crouch_percent;
        _animator_facade.crouch_dive = crouch_dive_remaining > 0f;
    }

    
    ////////// debug fields /////////

    public Vector3 debug_crouch_dive_direction;
    public bool debug_crouch_input, debug_space_held;
    
    protected override void SetDebugData() { 
        base.SetDebugData();
        debug_crouch_dive_direction = crouch_dive_direction;
        debug_crouch_input = CrouchInput();
        debug_space_held = Input.GetKey(KeyCode.Space);
    }
}
