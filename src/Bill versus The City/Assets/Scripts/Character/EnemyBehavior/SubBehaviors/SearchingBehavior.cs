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
    
    public void SetControllerFlags(EnemyBehavior parent, ManualCharacterMovement player) {
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
            UpdateNextTarget(parent);
        }

        parent.ctrl_target = player;
        parent.ctrl_waypoint = current_search_target;
        parent.ctrl_will_shoot = false;
        parent.ctrl_move_mode = MovementTarget.waypoint;
        parent.ctrl_sprint = false;
        if (parent.controller.seeing_target) {
            parent.ctrl_aim_mode = AimingTarget.target;
        }
        else {
            // don't aim at unseen target
            parent.ctrl_aim_mode = AimingTarget.movement_direction;
        }
    }
    public void AssumeBehavior(EnemyBehavior parent) {
        ResetSearch(parent);
    }

    private bool ReachedDestination(EnemyBehavior parent) {
        // returns true if the enemy has reached it's current destination
        return ReachedDestination(parent, current_search_target);
    }
    private bool ReachedDestination(EnemyBehavior parent, Vector3 dest) {
        // returns true if the enemy has reached it's current destination
        return WaypointSystem.ReachedDestination(parent.transform.position, dest);
    }

    public void ResetSearch(EnemyBehavior parent) {
        Debug.Log($"{parent.gameObject.name}.SearchingBehavior.ResetSearch"); // TODO --- remove debug
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
    }
    
    public Transform GetNextSearchDestination(EnemyBehavior parent) {
        // Vector3 start_pos = parent.transform.position;
        return RandomNextWaypoint();
    }

    public Transform RandomNextWaypoint() {
        return SwarmIntelligence.inst.GetNewPoint();
    }

    public string GetDebugMessage(EnemyBehavior parent) {
        float dist = WaypointSystem.FlattenedDistance(parent.transform.position, current_search_target);
        return $"dist: {dist}, n waypoints searched: {waypoints_searched.Count}, use_initial_search_target: {use_initial_search_target}, first_search: {first_search}, target_not_found: {target_not_found}";
    }
}