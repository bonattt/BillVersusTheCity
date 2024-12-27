using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class WaypointSystem : MonoBehaviour {


    private static WaypointSystem _inst;
    public static WaypointSystem inst {
        get { return _inst; }
    }

    public List<Transform> waypoints;

    void Start() {
        _inst = this;
        foreach (Transform child in transform) {
            waypoints.Add(child);
        }
    }

    private static bool UseAllTransforms(Transform t) => true; // include all Transforms in the method 

    private Func<Transform, bool> GetCoverFromFilter(Vector3 cover_from) {
        return (Transform t) => NavMeshUtils.PositionHasCoverFrom(cover_from, t.position);
    }

    public Transform GetClosestWaypoint(Vector3 start_point) {
        // gets the closest waypoint to `start_point` by NavMesh travel distance 
        return _GetClosestPoint(start_point, UseAllTransforms);
    }

    public Transform GetClosestCoverPosition(Vector3 start_point, Vector3 cover_from) {
        // takes a start position and a position for an enemy to take cover from
        // returns the closest by travel distance point from waypoints which cannot raycast to `cover_from`
        return _GetBestPosition(start_point, GetCurriedCoverDistanceScore(cover_from), GetCoverFromFilter(cover_from));
    }

    private Transform _GetClosestPoint(Vector3 start_point, Func<Transform, bool> use_transform) {
        // float closest_distance = float.PositiveInfinity;
        // Transform closet_destination = null;
        // foreach(Transform p in waypoints) {
        //     if (! use_transform(p)) { continue; }
        //     float dist = DistanceToPoint(start_point, p.position);
        //     if (dist < closest_distance) {
        //         closest_distance = dist;
        //         closet_destination = p;
        //     }
        // }
        // return closet_destination;
        return _GetBestPosition(start_point, RawDistanceScore, use_transform);
    }

    private static float RawDistanceScore(Vector3 start_pos, Vector3 destination) {
        // returns a score that sorts for the closest point
        return -TravelDistanceToPoint(start_pos, destination);
    }

    private Transform _GetBestPosition(Vector3 start_point, Func<Vector3, Vector3, float> score_funct, Func<Transform, bool> filter_func) {
        // takes a start_point, scoring method, and filtering method.
        // returns the Waypoint with the highest score which is not excluded by the filter.
        // highest score is returned. filter_func should return true to include a waypoint in the decision.
        float best_score = float.NegativeInfinity;
        Transform best_destination = null;
        foreach(Transform p in waypoints) {
            if (! filter_func(p)) { continue; }
            float score = score_funct(start_point, p.position);
            if (score > best_score) {
                best_score = score;
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
        float distance_from_enemy = TravelDistanceToPoint(destination, cover_from);
        return Mathf.Sqrt(distance_from_enemy) - Mathf.Pow(travel_distance/4, 2);
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