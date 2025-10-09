

using UnityEngine;

public class PlayerControls : MonoBehaviour {
    // class for handling player inputs, and calling methods on ManualCharacterMovement to move the player

    public bool sprint_input, aim_input, cancel_aim_input, reload_input;
    public ManualCharacterMovement player_movement;

    public bool resume_aim_after_reload = false;
    public bool resume_sprint_after_reload = false;

    [Tooltip("Set to true to lock the player's controls without pausing the game. (Used for Choreography)")]
    public bool controls_locked = false;

    protected IFirearm current_weapon { get { return player_movement.current_firearm; } }

    void Update() {
        if (controls_locked || !player_movement.is_active) { return; /* do nothing while character inactive */ }
        sprint_input = SprintInput();
        aim_input = AimInput();
        cancel_aim_input = CancelAimInput();
        reload_input = ReloadInput();

        // if not reloading, don't carryover sprint or aim
        if (!player_movement.reloading) {
            resume_aim_after_reload = false;
            resume_sprint_after_reload = false;
        }

        // resume aim while reloading if aim is held, but don't actually aim
        if (resume_aim_after_reload && aim_input) {
            resume_aim_after_reload = true;
            aim_input = false;
        } else if (resume_aim_after_reload) {
            resume_aim_after_reload = false;
        }

        // resume sprint while reloading if sprint is held, but don't actually sprint
        if (resume_sprint_after_reload && sprint_input) {
            resume_sprint_after_reload = true;
            sprint_input = false;
        } else if (resume_sprint_after_reload) {
            resume_sprint_after_reload = false;
        }

        // resume
        player_movement.keep_reloading = !(resume_aim_after_reload || resume_sprint_after_reload);

        if (sprint_input) {
            player_movement.aiming = false;
        } else {
            player_movement.aiming = aim_input && player_movement.combat_enabled;
            if (player_movement.reloading && player_movement.aiming) {
                player_movement.CancelReload();
            }
        }
        if (player_movement.reloading && sprint_input) {
            player_movement.CancelReload();
        }

        if (!sprint_input && aim_input && player_movement.combat_enabled) {
            player_movement.aiming = true;
        } else if (!aim_input || sprint_input) {
            player_movement.aiming = false;
        } else {
            // ????
            // TODO --- why did I comment "????" here?
        }

        if (InputSystem.inst.AttackReleasedInput()) {
            player_movement.AttackReleased();
        }
        if (AttackInput()) {
            if (player_movement.reloading) {
                player_movement.CancelReload();
            } else {
                bool hold = !InputSystem.inst.AttackClickInput() && InputSystem.inst.AttackHoldInput();
                bool attack_made = player_movement.TryToAttack(hold);
                // Debug.LogWarning($"attack made? {attack_made}, hold? {hold} = (!{InputSystem.current.AttackClickInput()} && {InputSystem.current.AttackHoldInput()})"); // TODO --- remove debug
            }
        } else if (sprint_input && !player_movement.reloading && ReloadInput()) { // && player_movement.CanReload()) {
            player_movement.StartReload();
            resume_aim_after_reload = false;
            resume_sprint_after_reload = true;
        } else if (aim_input && !player_movement.reloading && ReloadInput()) { // && player_movement.CanReload()) {
            player_movement.StartReload();
            resume_aim_after_reload = true;
            resume_sprint_after_reload = false;
        } else if (!sprint_input && !player_movement.reloading && ReloadInput()) { // && player_movement.CanReload()) {
            player_movement.StartReload();
            resume_aim_after_reload = false;
            resume_sprint_after_reload = false;
        } else if (CancelReloadInput() && player_movement.reloading) {
            player_movement.CancelReload();
        }

        Vector3 move_dir = MoveDirection();
        player_movement.MoveCharacter(move_dir, LookAtMouseVector(), sprint: sprint_input, crouch: CrouchInput());

        UpdateDebug();
    }

    public void StartReload(bool sprint_input, bool aim_input) {
        player_movement.StartReload();
        resume_aim_after_reload = aim_input;
        resume_sprint_after_reload = sprint_input;
    }

    public void CancelReload() {
        resume_aim_after_reload = false;
        resume_sprint_after_reload = false;
        player_movement.CancelReload();
    }

    public bool AttackInput() {
        if (current_weapon == null) { return false; }
        if (current_weapon.auto_fire) {
            return InputSystem.inst.AttackHoldInput();
        }
        return InputSystem.inst.AttackClickInput();
    }

    public bool ReloadInput() {
        if (player_movement.reloading) {
            return false;
        }
        return InputSystem.inst.ReloadInput();
    }

    public bool SprintInput() {
        return InputSystem.inst.SprintInput();
    }

    public bool CancelReloadInput() {
        if (!player_movement.reloading) {
            return false;
        }
        return InputSystem.inst.ReloadInput() || InputSystem.inst.AttackClickInput();
    }

    public bool AimInput() {
        return InputSystem.inst.AimHoldInput();
    }

    public bool CrouchInput() {
        return InputSystem.inst.CrouchInput();
    }

    public bool CancelAimInput() {
        if (player_movement.aiming) {
            return !AimInput();
        }
        return false;
    }

    public Vector3 GetMousePosition() {
        Vector3 mouse_pos = InputSystem.inst.MouseWorldPosition();
        Vector3 look_target = new Vector3(mouse_pos.x, transform.position.y, mouse_pos.z);
        return look_target;
    }

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
        float move_x = InputSystem.inst.MoveXInput();
        float move_y = InputSystem.inst.MoveYInput();
        Vector3 move = new Vector3(move_x, 0, move_y);
        return move.normalized;
    }


    //////////////////// DEBUG //////////////////// 

    public bool debug__attack_input;
    private float debug__attack_at = -1f;
    void UpdateDebug() {
        float show_attack_input_for = 0.1f;
        if (AttackInput()) {
            debug__attack_at = Time.time;
            debug__attack_input = true;
        } else if (debug__attack_at + show_attack_input_for >= Time.time) {
            debug__attack_input = true;
        } else {
            debug__attack_input = false;
        }
    }
}