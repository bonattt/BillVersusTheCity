using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualCharacterMovement : CharCtrl
{
    // returns a list of all the nodes which enemies raycast to see the player
    public Transform vision_target;
    private List<Transform> _vision_nodes = null;
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

    public override bool is_player { get { return true; }}

    public override float movement_speed {
        get {
            // TODO --- move this to PlayerControls
            return walk_speed;
            // if (crouch_dive_remaining > 0f) {
            //     return walk_speed * sprint_multiplier;
            // }
            // float move_speed = base.movement_speed;
            // float crouch_multiplier = (1 - crouch_percent) + (this.crouched_speed * crouch_percent);
            // if ((is_sprinting && crouch_percent >= 1f) || CrouchInput()) {
            //     // disallow sprinting while fully crouched, but not while standing up
            //     move_speed = move_speed / sprint_multiplier;
            // }
            // return move_speed * crouch_multiplier;
        }
    }

    void Update()
    {   
        // SetDebugData();
        if (! is_active) { return; } // do nothing while controller disabled
        UpdateReload();
        // SetAction();
        HandleAnimation();
        // Move();
        // TryToAttack();
        UpdatePauseAttackLock();
        PostUpdate();
    }

    // TODO --- move this to PlayerControls
    // private void HandleCrouch() {
    //     if (crouch_dive_remaining >= 0) { crouch_dive_remaining -= Time.deltaTime; }
    //     bool is_moving =  MoveVector() != new Vector3(0f, 0f, 0f);
    //     if (is_moving && CrouchInput() && SprintInput() && crouch_percent <= 0f && crouch_dive_remaining <= 0) { // crouch diving
    //         crouch_dive_direction = MoveVector();
    //         // WARNING: , crouch_dive_remaining must be set AFTER setting crouch_dive_direction, because it effects the output of MoveDirection()
    //         crouch_dive_remaining = crouch_dive_duration;  
    //         crouch_percent = 1;
    //     } else if (CrouchInput()) {  // is crouching
    //         crouch_percent += crouch_rate * Time.deltaTime;
    //     } 
    //     else {  // not crouching
    //         if (crouch_dive_remaining < 0) {
    //             // un-crouch, but only if you're not in a crouch dive
    //             crouch_percent -= uncrouch_rate * Time.deltaTime;
    //         }
    //     }
    //     Vector3 destination;
    //     Transform _crouch_target = GetCrouchTarget();
    //     if (crouch_percent == 1) {
    //         destination = new Vector3(_crouch_target.position.x, crouch_height, _crouch_target.position.z);
    //     } else if (crouch_percent == 0) {
    //         destination = new Vector3(_crouch_target.position.x, uncrouched_height, _crouch_target.position.z);
    //     }
    //     else {
    //         float y = (uncrouched_height * (1 - crouch_percent)) + (crouch_height * crouch_percent);
    //         destination = new Vector3(_crouch_target.position.x, y, _crouch_target.position.z);
    //     }
    //     _crouch_target.position = destination;
    // }

    // private bool use_look_as_direction = false;
    // private Vector3 look_vector;

    // public void SetLookTarget(Vector3 target) {
    //     // sets the look target, and voids look direction.
    //     use_look_as_direction = false;
    //     look_vector = target;
    // }

    // public void SetLookDirection(Vector3 direction) {
    //     // sets the look direction, and voids look target.
    //     use_look_as_direction = true;
    //     look_vector = direction;
    // }

    [SerializeField]  // TODO --- remove debug
    private Vector3 _last_move;
    public override void MoveCharacter(Vector3 move_direction, Vector3 look_direction, bool sprint=false, bool crouch=false) {
        // is_sprinting = sprint && CanSprint();
        // if (crouch_dive_remaining > 0f && crouch_dive_direction != Vector3.zero) {
        //     // if crouch diving, continue in that direction for the duration of the crouch dive
        //     // Debug.Log("continue crouch dive!"); // TODO --- remove debug
        //     move_direction = crouch_dive_direction;
        // } else if (crouch && sprint && !_crouch_last_frame && crouch_dive_remaining <= 0) {
        //     // Start crouch dive
        //     // Debug.Log("start crouch dive!"); // TODO --- remove debug
        //     crouch_dive_remaining = crouch_dive_duration;
        //     crouch_dive_direction = move_direction;
        //     crouch_percent = 1f;
        // } else {
        //     if (crouch) {
        //         // cannot crouch and sprint at the same time, if there is no crouch dive
        //         is_sprinting = false;
        //     }
        // }

        // if (crouch_dive_remaining > 0f || is_sprinting) {
        //     // always face forward during crouch dive or sprint!
        //     look_direction = move_direction;
        // }
        // UpdateCrouch(crouch);
        // SetCharacterLookDirection(look_direction);
        // _crouch_last_frame = crouch;
        base.MoveCharacter(move_direction, look_direction, sprint, crouch);
        
        _last_move = move_direction * GetMoveSpeed();
        controller.SimpleMove(_last_move);
    }

    public override Vector3 GetVelocity() {
        return _last_move;
    }

    public float GetMoveSpeed()  {
        // Debug.Log($"walk_speed: {walk_speed}, crouch_speed {crouched_speed}, sprint_multiplier {sprint_multiplier}"); // TODO --- remove debug
        if (crouch_dive_remaining > 0f) {
            return sprint_multiplier * walk_speed;
        }
        if (crouch_percent > 0f) {
            return ((1 - crouch_percent) * walk_speed) + (crouched_speed * crouch_percent); 
        }
        else if (is_sprinting) {
            return sprint_multiplier * walk_speed;
        }
        return walk_speed;
    }

    // public override void Move(bool sprint=false) {
    //     sprint = sprint && CanSprint();
    //     LookWithAction();
    //     // HandleCrouch(); // TODO --- move this to PlayerControls
    //     controller.SimpleMove(MoveVector());
    // }





    protected override void PostUpdate() {
        if (InputSystem.current.NextWeaponModeInput()) {
            attack_controller.current_weapon.NextWeaponSetting();
            if (attack_controller.current_weapon.HasWeaponSettings()) {
                ISounds sound = SFXLibrary.LoadSound("weapon_mode_switch");
                SFXSystem.inst.PlaySound(sound, transform.position);

            }
            attack_controller.UpdateSubscribers();
        }
    }

    // protected Vector3 ModifyMoveVector(Vector3 move) {
    //     return move * this.movement_speed;
    // }

    // public override Vector3 GetLookTarget() {
    //     Vector3 mouse_pos = InputSystem.current.MouseWorldPosition();
    //     Vector3 look_target = new Vector3(mouse_pos.x, transform.position.y, mouse_pos.z);
    //     return look_target;
    // }

    // protected  override Vector3 LookVector() {
    //     if (is_sprinting || crouch_dive_remaining > 0) {
    //         return MoveDirection();
    //     } else {
    //         return base.LookVector();
    //     }
    // }

    // public override Vector3 MoveDirection() {
    //     Vector3 move;
    //     if (crouch_dive_remaining > 0f) {
    //         move = crouch_dive_direction;
    //     } else {
    //         // float move_x, move_y;
    //         float move_x = InputSystem.current.MoveXInput();
    //         float move_y = InputSystem.current.MoveYInput();
    //         move = new Vector3(move_x, 0, move_y);
    //     }        
    //     return move.normalized;
    // }
    
    // public override Vector3 MoveVector() {
    //     return MoveDirection() * movement_speed;
    // }

    public override void DelayedOnDeath(ICharacterStatus status) {
        base.DelayedOnDeath(status);
        LevelConfig.inst.FailLevel();
    }
    
    protected override void HandleAnimation() {
        base.HandleAnimation();
        _animator_facade.crouch_percent = crouch_percent;
        _animator_facade.crouch_dive = crouch_dive_remaining > 0f;
    }
    
}
