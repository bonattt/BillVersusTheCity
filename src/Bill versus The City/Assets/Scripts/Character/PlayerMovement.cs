using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharCtrl
{
    
    public float walk_speed = 4.0f;
    public float sprint_speed = 10.0f;
    public float sprint_cost = 15.0f;
    public float attack_cost = 10.0f;
    public float move_x = 0f;
    public float move_y = 0f;

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
        return false;
    }

    public override bool CancelReloadInput() {
        if (! this.reloading) {
            return false;
        }
        return InputSystem.current.ReloadInput();
    }
    
    protected override void Move() {
        LookWithAction();
        Vector3 move = MoveVector();
        move = ModifyMoveVector(move);
        controller.SimpleMove(move);
    }

    protected Vector3 ModifyMoveVector(Vector3 move) {
        if (reloading) {
            move *= reload_move_speed;
        }
        if (this.is_hit_stunned) {
            move *= 0.1f;
        }
        return move;
    }

    public override Vector3 LookTarget() {
        Vector3 mouse_pos = InputSystem.current.MouseWorldPosition();
        Vector3 look_target = new Vector3(mouse_pos.x, transform.position.y, mouse_pos.z);
        return look_target;
    }
    
    public override Vector3 MoveVector() {
        // float move_x, move_y;
        move_x = InputSystem.current.MoveXInput();
        move_y = InputSystem.current.MoveYInput();
        if (move_x != 0 && move_y != 0) {
            // remove crabwalk
            move_x = move_x / 1.4f;
            move_y = move_y / 1.4f;
        }
        Vector3 move_vector = new Vector3(move_x, 0, move_y);

        return move_vector * walk_speed;
    }
}
