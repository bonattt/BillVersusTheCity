using UnityEngine;

public class ChasePlayerBehavior : ISubBehavior  {

    public float shooting_rate { get { return 1f; }}
    
    public ChasePlayerBehavior() { /* do nothing */ }
    ~ChasePlayerBehavior() { /* do nothing */ }
    
    public void SetControllerFlags(EnemyBehavior parent, ManualCharacterMovement player) {
        // parent.controller.ctrl_waypoint = new Vector3(0, 0, 0);
        parent.controller.ctrl_sprint = false;
        parent.controller.ctrl_target = player;
        parent.controller.ctrl_will_shoot = parent.DistanceToTarget() < parent.max_attack_range;
        parent.controller.ctrl_move_mode = MovementTarget.target;
        if (parent.controller.seeing_target) {
            parent.controller.ctrl_aim_mode = AimingTarget.target;
        }
        else {
            // don't aim at unseen target
            parent.controller.ctrl_aim_mode = AimingTarget.movement_direction;
        }
    }
}