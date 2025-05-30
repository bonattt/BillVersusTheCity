using UnityEngine;

public class ChasePlayerSprintBehavior : ISubBehavior  {

    public float shooting_rate { get { return 1f; }}
    
    public ChasePlayerSprintBehavior() { /* do nothing */ }
    ~ChasePlayerSprintBehavior() { /* do nothing */ }
    
    public void SetControllerFlags(EnemyBehavior parent, ManualCharacterMovement player) {
        // parent.ctrl_waypoint = new Vector3(0, 0, 0);
        parent.ctrl_sprint = true;
        parent.ctrl_target = player;
        parent.ctrl_will_shoot = false;
        parent.ctrl_move_mode = MovementTarget.target;
        if (parent.controller.seeing_target) {
            parent.ctrl_aim_mode = AimingTarget.target;
        }
        else {
            // don't aim at unseen target
            parent.ctrl_aim_mode = AimingTarget.movement_direction;
        }
    }
}