using UnityEngine;

public class SearchingBehavior : ISubBehavior  {

    public float shooting_rate { get { return 1f; }}
    
    public SearchingBehavior() { /* do nothing */ }
    
    public void SetControllerFlags(EnemyBehavior parent, PlayerMovement player) {
        if (Vector3.Distance(parent.transform.position, parent.perception.last_seen_at) <= 0.25f) {
            parent.perception.last_seen_at_investigated = true;
        }
        parent.controller.ctrl_target = player;
        parent.controller.ctrl_waypoint = parent.perception.last_seen_at;
        parent.controller.ctrl_will_shoot = false;
        parent.controller.ctrl_move_mode = MovementTarget.waypoint;
        parent.controller.ctrl_sprint = false;
        if (parent.controller.seeing_target) {
            parent.controller.ctrl_aim_mode = AimingTarget.target;
        }
        else {
            // don't aim at unseen target
            parent.controller.ctrl_aim_mode = AimingTarget.movement_direction;
        }

    }
}