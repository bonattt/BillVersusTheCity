using UnityEngine;

public class ChasePlayerBehavior : ISubBehavior  {
    
    public ChasePlayerBehavior() { /* do nothing */ }
    
    public void SetControllerFlags(EnemyBehavior parent) {
        // parent.controller.ctrl_waypoint = new Vector3(0, 0, 0);
        parent.controller.ctrl_will_shoot = true;
        parent.controller.ctrl_move_mode = MovementTarget.target;
        if (parent.controller.seeing_target) {
            parent.controller.ctrl_aim_mode = AimingTarget.target;
        }
        else {
            Debug.Log("don't aim at unseen target");
            parent.controller.ctrl_aim_mode = AimingTarget.stationary;
        }
    }
}