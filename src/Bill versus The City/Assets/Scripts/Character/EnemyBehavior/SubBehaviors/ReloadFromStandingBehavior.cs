using UnityEngine;

public class ReloadFromStandingBehavior : ISubBehavior  {

    public float shooting_rate { get { return 1f; }}

    public ReloadFromStandingBehavior() { /* do nothing */ }
    
    public void SetControllerFlags(EnemyBehavior parent, ManualCharacterMovement player) {
        parent.ctrl_target = player;
        parent.ctrl_will_shoot = false;

        parent.ctrl_sprint = false;
        parent.ctrl_move_mode = MovementTarget.stationary;
        parent.ctrl_aim_mode = AimingTarget.target;
        parent.ctrl_start_reload = true;
        parent.ctrl_cancel_reload = false;
    }
}