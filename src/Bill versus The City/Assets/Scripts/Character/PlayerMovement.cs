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
        if (attack_controller.current_weapon.auto_fire) {
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
                Debug.Log($"crouch dive speed: {walk_speed} * {sprint_multiplier}");
                return walk_speed * sprint_multiplier;
            }
            float move_speed = base.movement_speed;
            float crouch_multiplier = (1 - crouch_percent) + (this.crouched_speed * crouch_percent);
            if ((is_spinting && crouch_percent >= 1f) || CrouchInput()) {
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
            crouch_percent = 1;
            crouch_dive_remaining = crouch_dive_duration;
            crouch_dive_direction = MoveVector();
            Debug.Log("start crouch dive!");
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
        Vector3 move;
        if (crouch_dive_remaining > 0f) {
            Debug.Log($"crouch_dive_remaining: {crouch_dive_remaining}, crouch_dive_direction: {crouch_dive_direction}");
            move = crouch_dive_direction;
        } else {
            move = MoveVector();
        }        
        move = ModifyMoveVector(move);
        controller.SimpleMove(move);
    }

    protected override void PostUpdate() {
        if (InputSystem.current.NextWeaponModeInput()) {
            attack_controller.current_weapon.NextWeaponSetting();
            if (attack_controller.current_weapon.HasWeaponSettings()) {
                ISoundSet sound = SFXLibrary.LoadSound("weapon_mode_switch");
                SFXSystem.instance.PlaySound(sound, transform.position);

            }
            attack_controller.UpdateSubscribers();
        }
    }

    protected Vector3 ModifyMoveVector(Vector3 move) {
        return move * this.movement_speed;
    }

    public override Vector3 LookTarget() {
        Vector3 mouse_pos = InputSystem.current.MouseWorldPosition();
        Vector3 look_target = new Vector3(mouse_pos.x, transform.position.y, mouse_pos.z);
        return look_target;
    }
    
    public override Vector3 MoveVector() {
        // float move_x, move_y;
        float move_x = InputSystem.current.MoveXInput();
        float move_y = InputSystem.current.MoveYInput();
        if (move_x != 0 && move_y != 0) {
            // remove crabwalk
            move_x = move_x / 1.4f;
            move_y = move_y / 1.4f;
        }
        Vector3 move_vector = new Vector3(move_x, 0, move_y);

        return move_vector;
    }

    
    protected override void CharacterDeath() {
        MenuManager.inst.PlayerDefeatPopup();
    }
}
