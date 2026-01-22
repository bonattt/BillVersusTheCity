using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ManualCharacterMovement : CharCtrl {
    private const string VAULT_OVER_SOUND = "player_vault_over";
    private const string CROUCH_DIVE_SOUND = "player_dive_for_cover";
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
    public float max_movement_speed { get => walk_speed * sprint_multiplier; }

    public override bool is_player { get { return true; } }
    public override float movement_speed {
        get {
            if (move_action == null) {
                // Debug.LogWarning($"{gameObject.name} move_action is null!!");
                return walk_speed;
            }
            return move_action.movement_speed;
        }
        // {
        //         if (crouch_dive_remaining > 0f) {
        //         return sprint_multiplier * walk_speed;
        //     }
        //     if (crouch_percent > 0f) {
        //         return ((1 - crouch_percent) * walk_speed) + (crouched_speed * crouch_percent);
        //     } else if (is_vaulting) {
        //         return vault_over_speed;
        //     } else if (is_sprinting) {
        //         return sprint_multiplier * walk_speed;
        //     } else if (reloading) {
        //         return reload_move_multiplier * walk_speed;
        //     } else if (aiming) {
        //         if (current_firearm == null) {
        //             Debug.LogWarning("aim without weapon!");
        //             return walk_speed;
        //         }
        //         return current_firearm.aim_move_speed * walk_speed;
        //     }
        //     return walk_speed;
        // }
    }

    protected override void Start() {
        base.Start();
    }

    void Update() {
        if (!is_active) { return; } // do nothing while controller disabled
        UpdateReload();
        HandleAnimation();
        UpdatePauseAttackLock();
        PostUpdate();
    }

    private const float GRAVITY = -19.6f;
    [SerializeField] private Vector3 _last_move;
    [SerializeField] private float _last_move_speed;
    [SerializeField] private Vector2 _last_move_flat;
    private IMoveAction move_action = null;
    public override void MoveCharacter(Vector3 move_direction, Vector3 look_direction, bool sprint = false, bool crouch = false, bool walk = false) {
        RemoveOldMoveAction();
        bool is_moving = move_direction.magnitude > 0;
        is_sprinting = sprint && CanSprint() && is_moving;
        UpdateMoveAction(move_direction, look_direction, sprint, crouch);
        UpdateCrouch(crouch);
        if (move_action.override_move_direction) {
            move_direction = move_action.move_direction;
        }
        if (move_action.look_direction == MoveActionLookDirection.look_direction) {
            SetCharacterLookDirection(look_direction);
        } else if (move_action.look_direction == MoveActionLookDirection.move_direction) {
            SetCharacterLookDirection(move_direction);
        } else {
            Debug.LogError($"Unhandled move_action.look_direction '{move_action.look_direction}'");
        }
        _crouch_last_frame = crouch;


        if (is_vaulting) {
            move_direction = vault_over_direction;
        } else if (is_crouch_diving) {
            // if crouch diving, continue in that direction for the duration of the crouch dive
            move_direction = crouch_dive_direction;
        }

        _last_move = move_direction.normalized * move_action.movement_speed;
        _last_move_speed = _last_move.magnitude;
        if (walk) {
            _last_move *= 0.5f;
        }
        if (!controller.isGrounded) {
            _last_move += new Vector3(0, GRAVITY, 0);
        }
        _last_move_flat = new Vector2(_last_move.x, _last_move.z);
        controller.Move(_last_move * GetDeltaTime());
        debug.move_direction = _last_move;
    }

    private void RemoveOldMoveAction() {
        if (move_action == null) { return; }
        move_action.duration -= GetDeltaTime();
        if (move_action.duration <= 0) {
            move_action = null;
        }
    }

    private void UpdateMoveAction(Vector3 move_direction, Vector3 look_direction, bool sprint, bool crouch) {
        // adds a move action if there is no move action
        if (move_action != null) {
            return;
        }
        if (aiming) {
            is_sprinting = false;
            move_action = AimAction();
        } else if (GetStartVaultThisFrame(move_direction, look_direction)) {
            VaultOverCoverZone zone = StartVaultOver(move_direction, look_direction);
            move_action = JumpAction(zone);
        } else if (GetStartCrouchDiveThisFrame(crouch, move_direction)) {
            StartCrouchDive(move_direction);
            move_action = DiveAction(move_direction);
        } else if (crouch || crouch_percent > 0) {
            // cannot crouch and sprint at the same time, if there is no crouch dive
            if (crouch_percent == 0) {
                StartCrouch();
            }
            is_sprinting = false;
            move_action = CrouchAction();
        } else if (sprint) {
            is_sprinting = true;
            move_action = SprintAction();
        } else {
            move_action = WalkAction();
        }
    }

    protected void StartCrouch() {
        crouch_percent = 1f; // start fully crouched
        PlayStartCrouchEffects();
    }
    
    protected void PlayStartCrouchEffects() {
        // no effects... yet
    }

    public override void PlayCrouchDiveEffects() {
        base.PlayCrouchDiveEffects();
        ISFXSounds sound = SFXLibrary.LoadSound(CROUCH_DIVE_SOUND);
        SFXSystem.inst.PlaySound(sound, transform.position);
    }

    public override void PlayVaultOverEffects() {
        base.PlayVaultOverEffects();
        ISFXSounds sound = SFXLibrary.LoadSound(VAULT_OVER_SOUND);
        SFXSystem.inst.PlaySound(sound, transform.position);
    }

    // public ActionCode GetCrouchAction() {
    //     ActionCode result = _GetCrouchAction();
    //     Debug.LogWarning($"crouch_action: {result}, _last_move: {_last_move}, _last_move_flat: {_last_move_flat}"); // TODO --- remove debug
    //     return result;
    // }
    public ActionCode GetCrouchAction() {
        // gets the action for the contextual "crouch or jump" control (the spacebar by default)
        if (crouch_percent != 0 || _last_move_flat == Vector2.zero) {
            return ActionCode.crouch;
        }
        // TODO --- use look direction instead of last move
        else if (GetCouldStartVaultThisFrame(move_direction: _last_move, look_direction: _last_move)) {
            return ActionCode.jump;
        } else if (GetStartCrouchDiveThisFrame(crouch: true, move_direction: _last_move)) {
            return ActionCode.dive;
        }
        return ActionCode.crouch;
    }

    public static void TeleportCharacterController(CharacterController character, Vector3 destination) {
        character.enabled = false;
        character.transform.position = destination;
        character.enabled = true;
    }

    public override void TeleportTo(Vector3 position) {
        TeleportCharacterController(controller, position);
    }

    public override Vector3 GetVelocity() {
        return _last_move;
    }

    protected override void PostUpdate() {
        if (InputSystem.inst.NextWeaponModeInput()) {
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

    public override void DelayedOnDeath(ICharacterStatus status) {
        base.DelayedOnDeath(status);
        LevelConfig.inst.FailLevel();
        // TODO --- this should live somewhere player related 
    }

    protected override void HandleAnimation() {
        base.HandleAnimation();
        _animator_facade.crouch_percent = crouch_percent;
        _animator_facade.crouch_dive = crouch_dive_remaining > 0f;
    }


    private IMoveAction WalkAction() {
        BasicMoveAction action = new BasicMoveAction(walk_speed, 0f);
        action.look_direction = MoveActionLookDirection.look_direction;
        action.name = "walk";
        return action;
    }

    private IMoveAction SprintAction() {
        BasicMoveAction action = new BasicMoveAction(walk_speed * sprint_multiplier, 0f);
        action.look_direction = MoveActionLookDirection.move_direction;
        action.name = "sprint";
        return action;
    }

    private IMoveAction CrouchAction() {
        CrouchAction action = new CrouchAction(this, walk_speed, crouched_speed, 0f);
        action.look_direction = MoveActionLookDirection.look_direction;
        action.name = "crouch";
        return action;
    }


    private IMoveAction AimAction() {
        AimAction action = new AimAction(this, walk_speed, walk_speed * aim_move_multiplier, 0f);
        action.look_direction = MoveActionLookDirection.look_direction;
        string name = current_firearm == null ? "" : $"({current_firearm.item_id})";
        action.name = $"aim {name}";
        return action;
    }

    private IMoveAction DiveAction(Vector3 move_direction) {
        BasicMoveAction action = new BasicMoveAction(walk_speed * sprint_multiplier, crouch_dive_duration);
        action.look_direction = MoveActionLookDirection.move_direction;
        action.move_direction = move_direction;
        action.override_move_direction = true;
        action.name = "crouch dive";
        return action;
    }

    private IMoveAction JumpAction(VaultOverCoverZone zone) {
        float vault_duration = GetJumpDuration(zone);
        BasicMoveAction action = new BasicMoveAction(vault_over_speed, vault_duration);
        action.look_direction = MoveActionLookDirection.move_direction;
        action.name = "jump";
        return action;
    }
    
    private float _flashbang_effectiveness = 1f;
    public float flashbang_effectiveness {
        get {
            float difficulty_adjustment = GameSettings.inst.difficulty_settings.GetFloat(DifficultySettings.PLAYER_FLASHBANG_EFFECTIVENESS);
            return _flashbang_effectiveness * difficulty_adjustment; 
        }
    }
    private float max_blind_duration = 3f;

    public override void FlashBangHit(Vector3 flashbang_position, float intensity) {
        float distance = PhysicsUtils.FlatDistance(flashbang_position, transform.position);
        intensity = intensity / distance;
        Debug.LogWarning($"flash bang hit: '{intensity}' (max: {max_blind_duration})");
        intensity = Mathf.Min(intensity, max_blind_duration);
        intensity = intensity * flashbang_effectiveness;
        FlashbangedUI.inst.FlashbangUntil(intensity);
    }

    public ManualCharacterMovementDebugger debug_subclass = new ManualCharacterMovementDebugger();
    protected override void UpdateDebug() {
        base.UpdateDebug();
        if (move_action == null) { debug_subclass.move_action = "<null>"; }
        else { debug_subclass.move_action = move_action.ToString(); }
    }
}


[Serializable]
public class ManualCharacterMovementDebugger {
    public string move_action;

}
