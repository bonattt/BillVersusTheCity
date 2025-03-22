

using UnityEngine;

public class PlayerControls : MonoBehaviour {
    // class for handling player inputs, and calling methods on ManualCharacterMovement to move the player

    public bool placeholder_sprint, debug_aim;
    public ManualCharacterMovement player_movement;

    protected IWeapon current_weapon { get { return player_movement.current_weapon; }}

    void Update() {
        if (!player_movement.is_active) { return; /* do nothing while character inactive */ }
        if (SprintInput()) {
            placeholder_sprint = true;
            debug_aim = false;
        } else {
            placeholder_sprint = false;
            player_movement.aiming = AimInput();
            debug_aim = player_movement.aiming;
        }


        if (AttackInput() && !placeholder_sprint) {
            if (player_movement.reloading) {
                player_movement.CancelReload();
            } else {
                player_movement.TryToAttack();
            }
        }
        else if (ReloadInput() && !player_movement.reloading) {
            player_movement.StartReload();
        } else if (CancelReloadInput() && player_movement.reloading) {
            player_movement.CancelReload();
        }
        player_movement.Move();
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
}