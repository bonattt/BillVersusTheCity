

using UnityEngine;

public class PlayerControls : MonoBehaviour {
    // class for handling player inputs, and calling methods on ManualCharacterMovement to move the player

    public bool sprinting, debug_aim;
    public ManualCharacterMovement player_movement;

    protected IWeapon current_weapon { get { return player_movement.current_weapon; }}

    void Update() {
        if (!player_movement.is_active) { return; /* do nothing while character inactive */ }
        sprinting = SprintInput();
        if (sprinting) {
            player_movement.aiming = false;
        } else {
            player_movement.aiming = AimInput();
        }
        if (player_movement.reloading && sprinting) {
            player_movement.CancelReload();
        }

        if (!sprinting && AimInput()) {
            player_movement.aiming = true;
        } else if (!AimInput() || sprinting) {
            player_movement.aiming = false;
        } else {
            // ????
        }

        if (!sprinting && AttackInput()) {
            if (player_movement.reloading) {
                player_movement.CancelReload();
            } else {
                player_movement.TryToAttack();
            }
        }
        else if (!sprinting && !player_movement.reloading && player_movement.CanReload() && ReloadInput() ) {
            player_movement.StartReload();
            // Debug.LogWarning("~~Start Reload!"); // TODO --- remove debug
        } else if (CancelReloadInput() && player_movement.reloading) {
            // Debug.LogWarning("~~Cancel Reload!"); // TODO --- remove debug
            player_movement.CancelReload();
        }
        // player_movement.Move(sprint:sprinting);
        Vector3 move_dir = MoveDirection();
        // Debug.Log($"move_dir 1: {move_dir}"); // TODO --- remove debug
        player_movement.MoveCharacter(move_dir, LookAtMouseVector(), sprint:sprinting, crouch:CrouchInput());
    }
    
    public bool AttackInput() {
        if (current_weapon == null) { return false; }
        if (current_weapon.auto_fire) {
            return InputSystem.current.AttackHoldInput();
        } 
        return InputSystem.current.AttackClickInput();
    }
    
    public bool ReloadInput() {
        if (player_movement.reloading) {
            return false;
        }
        return InputSystem.current.ReloadInput();
    }
    
    public bool SprintInput() {
        return InputSystem.current.SprintInput();
    }

    public bool CancelReloadInput() {
        if (! player_movement.reloading) {
            return false;
        }
        return InputSystem.current.ReloadInput() || InputSystem.current.AttackClickInput();
    }
    
    public bool AimInput() {
        return InputSystem.current.AimAttackInput();
    }

    public bool CrouchInput() {
        return InputSystem.current.CrouchInput();
    }

    public bool CancelAimInput() {
        if (player_movement.aiming) {
            return !AimInput();
        }
        return false;
    }

    public Vector3 GetMousePosition() {
        Vector3 mouse_pos = InputSystem.current.MouseWorldPosition();
        Vector3 look_target = new Vector3(mouse_pos.x, transform.position.y, mouse_pos.z);
        return look_target;
    }

    // protected Vector3 LookVector() {
    //     if (is_sprinting || crouch_dive_remaining > 0) {
    //         return MoveDirection();
    //     } else {
    //         return base.LookVector();
    //     }
    // }

    protected Vector3 LookAtMouseVector() {
        // gets the vector from the character toward the mouse, and removes any Y component
        Vector3 look_target = GetMousePosition();
        Vector3 look_vector = look_target - transform.position;
        Vector3 look_vector_flat = new Vector3(
            look_vector.x, 0, look_vector.z
        );
        return look_vector_flat;
    }

    public Vector3 MoveDirection() {
        // float move_x, move_y;
        float move_x = InputSystem.current.MoveXInput();
        float move_y = InputSystem.current.MoveYInput();
        Vector3 move = new Vector3(move_x, 0, move_y);
        // Debug.Log($"x: {move_x}, y: {move_y}, move: {move}, normalized: {move.normalized}"); // TODO --- remove debug
        return move.normalized;
    }
}