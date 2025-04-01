using UnityEngine;

public class DeadBehavior : ISubBehavior  {
    // behavior for once an enemy is killed

    public float shooting_rate { get { return 1f; }}
    
    public DeadBehavior() { /* do nothing */ }
    ~DeadBehavior() { /* do nothing */ }
    
    public void SetControllerFlags(EnemyBehavior parent, ManualCharacterMovement player) {
        parent.ctrl_waypoint = parent.transform.position;
        parent.ctrl_sprint = false;
        parent.ctrl_target = player;
        parent.ctrl_will_shoot = false;
        parent.ctrl_move_mode = MovementTarget.stationary;
        parent.ctrl_aim_mode = AimingTarget.stationary;
    }
}