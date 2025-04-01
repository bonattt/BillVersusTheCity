using UnityEngine;

public class ReloadFromCoverBehavior : ISubBehavior  {

    public float shooting_rate { get { return 1f; }}

    public float calculations_per_second = 0.75f;  // controls how frequently the behavior recalculates it's destination
    private float last_calculation_at = -1f;
    private bool cancel_reload_to_retreat = false;
    
    public ReloadFromCoverBehavior() : this(false) { /* do nothing */ }
    public ReloadFromCoverBehavior(bool cancel_reload_to_retreat) {
        this.cancel_reload_to_retreat = cancel_reload_to_retreat;
     }
    
    public void SetControllerFlags(EnemyBehavior parent, ManualCharacterMovement player) {
        if (last_calculation_at + calculations_per_second > Time.time) {
            return;
        }
        last_calculation_at = Time.time;
        parent.ctrl_target = player;
        parent.ctrl_will_shoot = false;

        bool can_see_player = parent.perception.seeing_target;
        if (!can_see_player) {
            Debug.Log("in cover, perform reload");
            // player cannot be seen, so just reload
            parent.ctrl_sprint = false;
            parent.ctrl_move_mode = MovementTarget.waypoint;
            parent.ctrl_aim_mode = AimingTarget.target;
            parent.ctrl_start_reload = true;
            parent.ctrl_cancel_reload = false;

        } else {
            Debug.Log("Player has LoS, retreat before reload");
            // player has a shot at the enemy, so retreat to cover
            parent.ctrl_start_reload = false;
            parent.ctrl_cancel_reload = cancel_reload_to_retreat;
            parent.ctrl_sprint = true;
            parent.ctrl_move_mode = MovementTarget.waypoint;
            parent.ctrl_aim_mode = AimingTarget.movement_direction;

            Vector3 start = parent.controller.transform.position;
            Vector3 cover_from = player.transform.position;

            Transform dest = WaypointSystem.inst.GetClosestCoverPosition(start, cover_from);
            parent.ctrl_waypoint = dest.position;
            Debug.DrawLine(start, parent.ctrl_waypoint, Color.green, Time.deltaTime);
        }

        // recalculate destination
    }
}