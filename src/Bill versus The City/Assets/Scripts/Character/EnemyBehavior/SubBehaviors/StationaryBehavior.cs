using UnityEngine;

public class StationaryBehavior : ISubBehavior {
    
    public float shooting_rate { get { return 1f; }}
    
    public StationaryBehavior() { /* do nothing */ }
    
    public void SetControllerFlags(EnemyBehavior parent, ManualCharacterMovement player) {
        parent.ctrl_will_shoot = false;
        parent.ctrl_move_mode = MovementTarget.stationary;
        parent.ctrl_aim_mode = AimingTarget.stationary;
        parent.ctrl_sprint = false;
    }
}