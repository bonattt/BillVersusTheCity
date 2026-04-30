using UnityEngine;

public class ReloadFromCoverBehavior : ISubBehavior  {

    public float shooting_rate { get { return 1f; }}

    public float calculations_per_second = 0.75f;  // controls how frequently the behavior recalculates it's destination
    private float last_calculation_at = -1f;
    private bool cancel_reload_to_retreat = false;
    private string debug_message = "init";

    private FleeToCoverBehavior flee_to_cover;
    
    public ReloadFromCoverBehavior() : this(false) { /* do nothing */ }
    public ReloadFromCoverBehavior(bool cancel_reload_to_retreat) {
        this.cancel_reload_to_retreat = cancel_reload_to_retreat;
        this.flee_to_cover = new FleeToCoverBehavior(false);
    }
    
    public void SetControllerFlags(EnemyBehavior parent, ManualCharacterMovement player) {
        if (last_calculation_at + calculations_per_second > Time.time) {
            return;
        }
        last_calculation_at = Time.time;
        parent.ctrl_target = player;
        parent.ctrl_will_shoot = false;

        // bool can_see_player = parent.perception.seeing_target;
        bool has_cover_from_player = NavMeshUtils.PositionHasCoverFrom(player.transform.position, parent.movement_script.transform.position);
        parent.ctrl_crouch = has_cover_from_player;
        if (has_cover_from_player) {
            debug_message = $"IN COVER recalculate at: {last_calculation_at + calculations_per_second}";
            Debug.Log("in cover, perform reload.");
            // player cannot be seen, so just reload
            parent.ctrl_sprint = false;
            parent.ctrl_move_mode = MovementTarget.waypoint;
            parent.ctrl_aim_mode = AimingTarget.target;
            parent.ctrl_start_reload = true;
            parent.ctrl_crouch = true;
            parent.ctrl_cancel_reload = false;
        } else {
            // debug_message = $"seeking cover. Already reloading? '{parent.movement_script.reloading}'. recalculate at: {last_calculation_at + calculations_per_second}";
            debug_message = flee_to_cover.GetDebugMessage(parent);
            flee_to_cover.SetControllerFlags(parent, player);
        }

        // recalculate destination
    }

    public string GetDebugMessage(EnemyBehavior parent) {
        return $"ReloadFromCover.{debug_message}";
    }
}