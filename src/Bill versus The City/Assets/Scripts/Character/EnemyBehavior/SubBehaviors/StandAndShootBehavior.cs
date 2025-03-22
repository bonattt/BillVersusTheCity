using UnityEngine;

public class StandAndShootBehavior : ISubBehavior  {
    
    public StandAndShootBehavior() { /* do nothing */ }
    
    // shoot twice as fast
    public float shooting_rate  { get { return 0.5f; }}
    
    public void SetControllerFlags(EnemyBehavior parent, ManualCharacterMovement player) {
        // parent.controller.ctrl_waypoint = new Vector3(0, 0, 0);
        parent.controller.ctrl_will_shoot = true;
        parent.controller.ctrl_target = player;
        parent.controller.ctrl_move_mode = MovementTarget.stationary;
        parent.controller.ctrl_sprint = false;
        if (parent.controller.seeing_target) {
            parent.controller.ctrl_aim_mode = AimingTarget.target;
        }
        else {
            // don't aim at unseen target
            parent.controller.ctrl_aim_mode = AimingTarget.stationary;
        }
    }
}