using System;
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
                _vision_nodes = new List<Transform> { transform };
                foreach (Transform child in vision_target) {
                    _vision_nodes.Add(child);
                }
            }
            return _vision_nodes;
        }
    }
    
    // returns the max possible speed a player can move; used by FootstepSounds to sanity check movement to avoid loud sounds when teleporting (from stairs etc...)
    public float max_movement_speed { get => movement_speed * sprint_multiplier; }

    public override bool is_player { get { return true; } }
    public override float movement_speed {
        get {
            if (crouch_dive_remaining > 0f) {
                return sprint_multiplier * walk_speed;
            }
            if (crouch_percent > 0f) {
                return ((1 - crouch_percent) * walk_speed) + (crouched_speed * crouch_percent);
            } else if (is_vaulting) {
                return vault_over_speed;
            } else if (is_sprinting) {
                return sprint_multiplier * walk_speed;
            }  else if (reloading) {
                return reload_move_multiplier * walk_speed;
            } else if (aiming) {
                if (current_firearm == null) {
                    Debug.LogWarning("aim without weapon!");
                    return walk_speed;
                }
                return current_firearm.aim_move_speed * walk_speed;
            }
            return walk_speed;
        }
    }

    protected override void Start() {
        base.Start();
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
    public override void MoveCharacter(Vector3 move_direction, Vector3 look_direction, bool sprint=false, bool crouch=false, bool walk=false) {
        base.MoveCharacter(move_direction, look_direction, sprint, crouch);
        
        if (is_crouch_diving) {
            // if crouch diving, continue in that direction for the duration of the crouch dive
            move_direction = crouch_dive_direction;
        } else if (is_vaulting) {
            move_direction = vault_over_direction;
        }
        
        _last_move = move_direction * movement_speed;
        if (walk) {
            _last_move *= 0.5f;
        }
        debug.move_direction = _last_move;
        controller.SimpleMove(_last_move);
    }

    public override Vector3 GetVelocity() {
        return _last_move;
    }

    protected override void PostUpdate() {
        if (InputSystem.current.NextWeaponModeInput()) {
            attack_controller.current_gun.NextWeaponSetting();
            if (attack_controller.current_gun.HasWeaponSettings()) {
                ISFXSounds sound = SFXLibrary.LoadSound("weapon_mode_switch");
                SFXSystem.inst.PlaySound(sound, transform.position);
            }
            try {
                ((IWeaponManager)attack_controller).UpdateSubscribers();
            } catch (InvalidCastException) {
                Debug.LogWarning($"cannot cast attack controller '{attack_controller}' to update subscribers");
            }
        }
    }

    public override void DelayedOnDeath(ICharacterStatus status)
    {
        base.DelayedOnDeath(status);
        LevelConfig.inst.FailLevel();
        // TODO --- this should live somewhere player related 
    }
    
    protected override void HandleAnimation() {
        base.HandleAnimation();
        _animator_facade.crouch_percent = crouch_percent;
        _animator_facade.crouch_dive = crouch_dive_remaining > 0f;
    }
    
}
