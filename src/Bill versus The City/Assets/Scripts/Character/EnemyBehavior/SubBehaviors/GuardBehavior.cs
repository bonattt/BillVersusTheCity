using UnityEngine;

public class GuardBehavior : ISubBehavior  {
    
    public GuardBehavior() { /* do nothing */ }
    
    // shoot twice as fast
    public float shooting_rate  { get { return 0.5f; }}
    
    public void SetControllerFlags(EnemyBehavior parent, ManualCharacterMovement player) {
        // parent.ctrl_waypoint = new Vector3(0, 0, 0);
        parent.ctrl_move_mode = MovementTarget.stationary;
        parent.ctrl_sprint = false;
        parent.ctrl_target = player;
        parent.ctrl_waypoint = parent.initial_movement_target;
        if (parent.movement_script.seeing_target) {
            parent.ctrl_will_shoot = true;
            parent.ctrl_aim_mode = AimingTarget.target;
            parent.ctrl_target = player;
        }
        else {
            parent.ctrl_will_shoot = false;
            parent.ctrl_aim_mode = AimingTarget.stationary;
        }
    }
}