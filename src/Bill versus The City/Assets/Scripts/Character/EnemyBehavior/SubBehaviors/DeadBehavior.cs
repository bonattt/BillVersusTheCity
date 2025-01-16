using UnityEngine;

public class DeadBehavior : ISubBehavior  {
    // behavior for once an enemy is killed

    public float shooting_rate { get { return 1f; }}
    
    public DeadBehavior() { /* do nothing */ }
    ~DeadBehavior() { /* do nothing */ }
    
    public void SetControllerFlags(EnemyBehavior parent, PlayerMovement player) {
        Debug.Log("DeadBehavior!!");
        parent.controller.ctrl_waypoint = parent.transform.position;
        parent.controller.ctrl_sprint = false;
        parent.controller.ctrl_target = player;
        parent.controller.ctrl_will_shoot = false;
        parent.controller.ctrl_move_mode = MovementTarget.stationary;
        parent.controller.ctrl_aim_mode = AimingTarget.stationary;
    }
}