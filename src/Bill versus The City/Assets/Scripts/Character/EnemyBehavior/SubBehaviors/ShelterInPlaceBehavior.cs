using UnityEngine;

public class ShelterInPlaceBehavior : ISubBehavior  {
    
    public ShelterInPlaceBehavior() { /* do nothing */ }
    
    // shoot twice as fast
    public float shooting_rate  { get { return 0.5f; }}
    
    public void SetControllerFlags(EnemyBehavior parent, ManualCharacterMovement player) {
        // parent.ctrl_waypoint = new Vector3(0, 0, 0);
        parent.ctrl_will_shoot = false;
        parent.ctrl_always_shoot = false;
        parent.ctrl_move_mode = MovementTarget.stationary;
        parent.ctrl_sprint = false;
        parent.ctrl_aim_mode = AimingTarget.stationary;
        parent.ctrl_crouch = true;
    }
}
