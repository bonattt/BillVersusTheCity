using System.Collections.Generic;

using UnityEngine;

public class SearchingBehavior : ISubBehavior  {

    public float shooting_rate { get { return 1f; }}
    
    // public SearchingBehavior() {
    // }

    public SearchingBehavior(bool use_initial_search_position, Vector3 initial_search_position) { 
        this.initial_search_target = initial_search_position;
        this.use_initial_search_target = use_initial_search_position;
    }

    private bool use_initial_search_target = false;
    private bool first_search = true;
    private Vector3 initial_search_target, current_search_target;
    private Transform current_search_waypoint = null;
    private bool target_not_found = false;
    private HashSet<Transform> waypoints_searched = new HashSet<Transform>();
    
    public void SetControllerFlags(EnemyBehavior parent, PlayerMovement player) {
        // if (Vector3.Distance(parent.transform.position, parent.perception.last_seen_at) <= 0.25f) {
        if (ReachedDestination(parent, parent.perception.last_seen_at)) {
            parent.perception.last_seen_at_investigated = true;
        }
        if (ReachedDestination(parent)) {
            UpdateNextTarget(parent);
        }
        parent.controller.ctrl_target = player;
        parent.controller.ctrl_waypoint = current_search_target;
        parent.controller.ctrl_will_shoot = false;
        parent.controller.ctrl_move_mode = MovementTarget.waypoint;
        parent.controller.ctrl_sprint = false;
        if (parent.controller.seeing_target) {
            parent.controller.ctrl_aim_mode = AimingTarget.target;
        }
        else {
            // don't aim at unseen target
            parent.controller.ctrl_aim_mode = AimingTarget.movement_direction;
        }

        //current_search_target = GetNextSearchDestination(parent);
    }
    public void AssumeBehavior(EnemyBehavior parent) {
        ResetSearch(parent);
    }
    // public void EndBehavior() { /* do nothing by default */ }

    private bool ReachedDestination(EnemyBehavior parent) {
        return ReachedDestination(parent, current_search_target);
    }
    private bool ReachedDestination(EnemyBehavior parent, Vector3 dest) {
        return WaypointSystem.ReachedDestination(parent.transform.position, dest);
    }

    public void ResetSearch(EnemyBehavior parent) {
        if (first_search && use_initial_search_target) {
            current_search_target = initial_search_target;
        } else {
            current_search_target = parent.perception.last_seen_at;
        }
        current_search_waypoint = null;
        first_search = false;
        target_not_found = false;
        waypoints_searched = new HashSet<Transform>();
    }

    private void UpdateNextTarget(EnemyBehavior parent) {
        if (current_search_waypoint != null) {
            waypoints_searched.Add(current_search_waypoint);
        }
        current_search_waypoint = GetNextSearchDestination(parent);
        current_search_target = current_search_waypoint.position;
    }
    
    public Transform GetNextSearchDestination(EnemyBehavior parent) {
        Vector3 start_pos = parent.transform.position;
        return WaypointSystem.inst.GetClosestCoverPositionNotInSet(start_pos, start_pos, waypoints_searched);
    }
}