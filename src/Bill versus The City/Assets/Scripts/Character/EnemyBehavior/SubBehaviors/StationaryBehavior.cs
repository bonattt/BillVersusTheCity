using UnityEngine;

public class StationaryBehavior : ISubBehavior {
    
    public float shooting_rate { get { return 1f; }}
    
    public StationaryBehavior() { /* do nothing */ }
    
    public void SetControllerFlags(EnemyBehavior parent, ManualCharacterMovement player) {
        parent.controller.ctrl_will_shoot = false;
        parent.controller.ctrl_move_mode = MovementTarget.stationary;
        parent.controller.ctrl_aim_mode = AimingTarget.stationary;
        parent.controller.ctrl_sprint = false;
    }
}