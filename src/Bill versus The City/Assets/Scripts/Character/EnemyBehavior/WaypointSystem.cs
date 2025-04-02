using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class WaypointSystem : MonoBehaviour {


    private static WaypointSystem _inst;
    public static WaypointSystem inst {
        get { return _inst; }
    }

    private List<Transform> _waypoints = null;
    public List<Transform> waypoints {
        get {
            if (_waypoints == null) {
                _waypoints = CalculateWaypoints();
            }
            return _waypoints;
        }
    }

    public List<Transform> GetWaypointsWithout(ICollection<Transform> excluded) {
        // returns a copy of the list of waypoints, with all the elements of `excluded` removed.
        List<Transform> result = new List<Transform>();
        foreach(Transform t in waypoints) {
            if (!excluded.Contains(t)) {
                result.Add(t);
            }
        }
        return result;
    }

    void Start() {
        _inst = this;
    }

    protected List<Transform> CalculateWaypoints() {
        List<Transform> waypoints_calculated = new List<Transform>();
        // calculates the waypoints from children of this GameObject, rather than a stored list
        foreach (Transform child in transform) {
            waypoints_calculated.Add(child);
        }
        return waypoints_calculated;
    }

    public static Vector3 FlattenVector(Vector3 v) {
        // returns a "flattened" vector, with the height set to 0
        return new Vector3(v.x, 0f, v.z);
    }

    public static float FlattenedDistance(Vector3 v1, Vector3 v2) {
        return Vector3.Distance(FlattenVector(v1), FlattenVector(v2));
    }

    public static bool ReachedDestination(Vector3 position, Vector3 destination, float distance_threshold=1.1f) {
        float dist = FlattenedDistance(position, destination);
        return dist <= distance_threshold;
    }

    private static bool UseAllTransforms(Transform t) => true; // include all Transforms in the method 

    private Func<Transform, bool> GetCoverFromFilter(Vector3 cover_from) {
        // filter allows only positions which have cover from the `cover_from` position
        return (Transform t) => NavMeshUtils.PositionHasCoverFrom(cover_from, t.position);
    }

    public Transform GetClosestWaypoint(Vector3 start_point) {
        // gets the closest waypoint to `start_point` by NavMesh travel distance 
        return _GetClosestPoint(start_point, UseAllTransforms);
    }

    public Transform GetClosestCoverPositionNotInSet(Vector3 start_point, Vector3 cover_from, HashSet<Transform> excluded_positions) {
        // finds a cover position from the Waypoints, while excluding the 
        Func<Transform, bool> Filter = CurriedAndFilter(GetExcludeSetFilter(excluded_positions), GetCoverFromFilter(cover_from));
        Transform result = _GetBestPosition(start_point, GetCurriedCoverDistanceScore(cover_from), Filter);
        return result;
    }

    private static Func<Transform, bool> GetExcludeSetFilter(HashSet<Transform> excluded_positions) {
        return (Transform t) => !excluded_positions.Contains(t);
    }

    private static Func<Transform, bool> CurriedAndFilter(Func<Transform, bool> filter_func1, Func<Transform, bool> filter_func2) {
        // returns a composite filter that requires 2 filters to both be passed
        return (Transform t) => filter_func1(t) && filter_func2(t);
    }

    public Transform GetClosestCoverPosition(Vector3 start_point, Vector3 cover_from) {
        // takes a start position and a position for an enemy to take cover from
        // returns the closest by travel distance point from waypoints which cannot raycast to `cover_from`
        return _GetBestPosition(start_point, GetCurriedCoverDistanceScore(cover_from), GetCoverFromFilter(cover_from));
    }

    private Transform _GetClosestPoint(Vector3 start_point, Func<Transform, bool> use_transform) {
        return _GetBestPosition(start_point, RawDistanceScore, use_transform);
    }

    private static float RawDistanceScore(Vector3 start_pos, Vector3 destination) {
        // returns a score that sorts for the closest point
        return TravelDistanceToPoint(start_pos, destination);
    }

    private Transform _GetBestPosition(Vector3 start_point, Func<Vector3, Vector3, float> score_funct, Func<Transform, bool> filter_func) {
        // takes a start_point, scoring method, and filtering method.
        // returns the Waypoint with the lowest distance-score which is not excluded by the filter.
        // lowest score is returned. filter_func should return true to include a waypoint in the decision.
        float best_score = float.PositiveInfinity;
        Transform best_destination = null;
        foreach(Transform p in waypoints) {
            if (! filter_func(p)) { continue; }
            float distance_score = score_funct(start_point, p.position);
            if (distance_score < best_score) {
                best_score = distance_score;
                best_destination = p;
            }
        }
        return best_destination;
    }

    public static Func<Vector3,Vector3,float> GetCurriedCoverDistanceScore(Vector3 cover_from) {
        // curries `DistanceCoverScore` to conform it to the distance scoring interface. 
        return (Vector3 start_pos, Vector3 destination) => DistanceCoverScore(start_pos, destination, cover_from);
    }

    public static float DistanceCoverScore(Vector3 start_pos, Vector3 destination, Vector3 cover_from) {
        // returns a score that values short travel distance, and also distance from an enemy. 
        // distance from an enemy is square-rooted to give it diminishing returns the further from the enemy you go.
        float travel_distance = TravelDistanceToPoint(start_pos, destination);
       
        // TODO --- maybe getting the travel direction of the first movement on the NavMesh path, instead of bird's eye direction to the destination will be more reliable here???
        Vector3 enemy_direction = (cover_from - start_pos).normalized;  
        Vector3 travel_direction = (destination - start_pos).normalized;
        // get a number 2-1 representing how towards the player character the destination is, to avoid taking cover in the direction of the threat you're taking cover from.
        float dot_product = Vector3.Dot(enemy_direction, travel_direction);
        float towards_enemy_score = dot_product + 1f / Mathf.Max(0.25f, Mathf.Abs(dot_product));
        // +1 added to dot-product to avoid multiplying by ZERO
        // divide by dot-product to favor moving sideways from player, rather than directly away, and Mathf.Max to avoid div-by-zero

        return towards_enemy_score * travel_distance;
    }

    public static float TravelDistanceToPoint(Vector3 start_point, Vector3 end_point) {
        // retuns the distance to travers a NavMesh path from `start_point` to `end_point`
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(start_point, end_point, NavMesh.AllAreas, path)) {
            float distance = 0f;
            for (int i = 1; i < path.corners.Length; i++) {
                distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
            return distance;
        }
        return float.PositiveInfinity;
    }

}