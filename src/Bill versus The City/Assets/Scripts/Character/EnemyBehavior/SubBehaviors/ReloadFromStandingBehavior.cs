using UnityEngine;

public class ReloadFromStandingBehavior : ISubBehavior  {

    public float shooting_rate { get { return 1f; }}

    public ReloadFromStandingBehavior() { /* do nothing */ }
    
    public void SetControllerFlags(EnemyBehavior parent, PlayerMovement player) {
        parent.controller.ctrl_target = player;
        parent.controller.ctrl_will_shoot = false;

        parent.controller.ctrl_sprint = false;
        parent.controller.ctrl_move_mode = MovementTarget.stationary;
        parent.controller.ctrl_aim_mode = AimingTarget.target;
        parent.controller.ctrl_start_reload = true;
        parent.controller.ctrl_cancel_reload = false;
    }
}