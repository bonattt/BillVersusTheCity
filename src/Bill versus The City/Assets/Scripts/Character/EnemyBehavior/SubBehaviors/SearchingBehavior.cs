using System.Collections.Generic;

using UnityEngine;

public class SearchingBehavior : ISubBehavior  {

    public float shooting_rate { get { return 1f; }}
    
    // public SearchingBehavior() {
    // }

    public SearchingBehavior(bool use_initial_search_position, Vector3 initial_search_position) { 
        this.initial_search_target = initial_search_position;
        this.use_initial_search_target = use_initial_search_position;
        if (use_initial_search_position) {
            current_search_target = initial_search_position;
        }
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
            target_not_found = true;
            Debug.Log("set target_not_found = true");
        }
        else if (!target_not_found && use_initial_search_target && first_search && ReachedDestination(parent, initial_search_target)) {
            Debug.Log("set target_not_found = true");
            target_not_found = true;
        }
        if (ReachedDestination(parent)) {
            Debug.LogWarning($"{parent.gameObject.name}: REACHED DEST!!"); // TODO --- remove debug
            UpdateNextTarget(parent);
        }

        // Debug.LogWarning($"{parent.gameObject.name}.current_search_target: {current_search_target}"); // TODO --- remove debug
        parent.controller.ctrl_target = player;
        parent.controller.ctrl_waypoint = current_search_target;
        // Debug.LogWarning($"initial_move_target: {initial_search_target}, current_search_targe: {current_search_target}"); // TODO --- remove debug
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
    }
    public void AssumeBehavior(EnemyBehavior parent) {
        ResetSearch(parent);
    }
    // public void EndBehavior() { /* do nothing by default */ }

    private bool ReachedDestination(EnemyBehavior parent) {
        // returns true if the enemy has reached it's current destination
        return ReachedDestination(parent, current_search_target);
    }
    private bool ReachedDestination(EnemyBehavior parent, Vector3 dest) {
        // returns true if the enemy has reached it's current destination
        return WaypointSystem.ReachedDestination(parent.transform.position, dest);
    }

    public void ResetSearch(EnemyBehavior parent) {
        Debug.LogWarning($"{parent.gameObject.name}.SearchingBehavior.ResetSearch"); // TODO --- remove debug
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
        Vector3 previous_target = current_search_target;
        use_initial_search_target = false;
        if (current_search_waypoint != null) {
            waypoints_searched.Add(current_search_waypoint);
        }
        current_search_waypoint = GetNextSearchDestination(parent);
        current_search_target = current_search_waypoint.position;
        // Debug.LogWarning($"Update Search target {previous_target} --> {current_search_target}");
    }
    
    public Transform GetNextSearchDestination(EnemyBehavior parent) {
        // Debug.LogWarning($"{parent.gameObject.name}.SearchingBehavior.GetNextSearchDestination"); // TODO --- remove debug
        Vector3 start_pos = parent.transform.position;
        // return WaypointSystem.inst.GetClosestCoverPositionNotInSet(start_pos, start_pos, waypoints_searched);
        // return WaypointSystem.inst.GetClosestCoverPosition(start_pos, start_pos);
        return RandomNextWaypoint();
    }

    public Transform RandomNextWaypoint() {
        // selects a waypoint at random, excluding any waypoints in `waypoints_searched`
        // List<Transform> valid_waypoints = WaypointSystem.inst.GetWaypointsWithout(waypoints_searched);
        // int i = Random.Range(0, valid_waypoints.Count);
        // return valid_waypoints[i];
        return SwarmIntelligence.inst.GetNewPoint();
    }

    public string GetDebugMessage(EnemyBehavior parent) {
        float dist = WaypointSystem.FlattenedDistance(parent.transform.position, current_search_target);
        return $"dist: {dist}, n waypoints searched: {waypoints_searched.Count}, use_initial_search_target: {use_initial_search_target}, first_search: {first_search}, target_not_found: {target_not_found}";
    }
}