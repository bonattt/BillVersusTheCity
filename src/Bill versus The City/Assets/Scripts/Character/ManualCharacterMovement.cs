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
            if (crouch_dive_remaining > 0f) {
                return sprint_multiplier * walk_speed;
            }
            if (crouch_percent > 0f) {
                return ((1 - crouch_percent) * walk_speed) + (crouched_speed * crouch_percent); 
            }
            else if (is_sprinting) {
                return sprint_multiplier * walk_speed;
            }
            else if (reloading) {
                return reload_move_multiplier * walk_speed; 
            }
            else if (aiming) {
                if (current_weapon == null) {
                    Debug.LogWarning("aim without weapon!");
                    return walk_speed;
                }
                return current_weapon.aim_move_speed * walk_speed;
            }
            return walk_speed;
        }
    }

    void Update()
    {   
        if (! is_active) { return; } // do nothing while controller disabled
        UpdateReload();
        HandleAnimation();
        UpdatePauseAttackLock();
        PostUpdate();
    }

    [SerializeField]
    private Vector3 _last_move;
    public override void MoveCharacter(Vector3 move_direction, Vector3 look_direction, bool sprint=false, bool crouch=false) {
        base.MoveCharacter(move_direction, look_direction, sprint, crouch);
        
        _last_move = move_direction * movement_speed;
        controller.SimpleMove(_last_move);
    }

    public override Vector3 GetVelocity() {
        return _last_move;
    }

    protected override void PostUpdate() {
        if (InputSystem.current.NextWeaponModeInput()) {
            attack_controller.current_weapon.NextWeaponSetting();
            if (attack_controller.current_weapon.HasWeaponSettings()) {
                ISFXSounds sound = SFXLibrary.LoadSound("weapon_mode_switch");
                SFXSystem.inst.PlaySound(sound, transform.position);
            }
            attack_controller.UpdateSubscribers();
        }
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
    
}
