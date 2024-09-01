using UnityEngine;

public class StationaryBehavior : ISubBehavior {
    
    public StationaryBehavior() { /* do nothing */ }
    
    public void SetControllerFlags(EnemyBehavior parent) {
        parent.controller.ctrl_will_shoot = false;
        parent.controller.ctrl_move_mode = MovementTarget.stationary;
        parent.controller.ctrl_aim_mode = AimingTarget.stationary;
    }
}