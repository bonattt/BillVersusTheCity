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
        parent.controller.ctrl_move_mode = MovementTarget.waypoint;
        parent.controller.ctrl_aim_mode = AimingTarget.movement_direction;

        Vector3 start = parent.controller.transform.position;
        Vector3 cover_from = player.transform.position;
        Vector3 raycast_dir = (cover_from - start);
        raycast_dir = new Vector3(raycast_dir.x, 0, raycast_dir.z).normalized; // flatten raycast on 
        bool cover_from_player = false;
        if (Physics.Raycast(
            new Vector3(start.x, 0.5f, start.z), 
            new Vector3(cover_from.x, 0.5f, cover_from.z),
            out RaycastHit hit, 
            Vector3.Distance(start, cover_from)
        )) {
            // already have cover, do nothing
            Debug.Log($"already have cover to player, shots blocked by {hit.collider.gameObject}");
            GameObject hit_object = hit.collider.gameObject;
            while (hit_object.transform.parent != null) {
                // find the top level object that was hit
                hit_object = hit_object.transform.parent.gameObject;
            }
            cover_from_player = hit_object.GetComponent<PlayerMovement>() == null;
        }
        // recalculate destination
        else if (!cover_from_player && last_calculation_at + calculations_per_second <= Time.time) {
            last_calculation_at = Time.time;
            parent.controller.ctrl_waypoint = NavMeshUtils.GetRetreatWithCover(start, cover_from);
        }
    }
}