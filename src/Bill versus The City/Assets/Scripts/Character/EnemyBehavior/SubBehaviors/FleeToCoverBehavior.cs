using UnityEngine;

public class FleeToCoverBehavior : ISubBehavior  {

    public float shooting_rate { get { return 1f; }}

    public float calculations_per_second = 1f;  // controls how frequently the behavior recalculates it's destination
    private float last_calculation_at = -1f;
    
    public FleeToCoverBehavior() { /* do nothing */ }
    ~FleeToCoverBehavior() { /* do nothing */ }
    
    public void SetControllerFlags(EnemyBehavior parent, PlayerMovement player) {
        // parent.controller.ctrl_waypoint = new Vector3(0, 0, 0);
        parent.controller.ctrl_target = player;
        parent.controller.ctrl_will_shoot = false;
        parent.controller.ctrl_sprint = true;
        parent.controller.ctrl_move_mode = MovementTarget.waypoint;
        parent.controller.ctrl_aim_mode = AimingTarget.movement_direction;

        Vector3 start = parent.controller.transform.position;
        Vector3 cover_from = player.transform.position;
        // recalculate destination
        if (last_calculation_at + calculations_per_second <= Time.time) {
            last_calculation_at = Time.time;
            Transform dest = WaypointSystem.inst.GetClosestCoverPosition(start, cover_from);
            parent.controller.ctrl_waypoint = dest.position;
            Debug.DrawLine(start, parent.controller.ctrl_waypoint, Color.green, Time.deltaTime);
        }
    }
}