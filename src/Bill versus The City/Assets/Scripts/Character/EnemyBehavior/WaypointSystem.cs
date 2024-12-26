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
        return _GetClosestPoint(start_point, GetCoverFromFilter(cover_from));
    }

    private Transform _GetClosestPoint(Vector3 start_point, Func<Transform, bool> use_transform) {
        float closest_distance = float.PositiveInfinity;
        Transform closet_destination = null;
        foreach(Transform p in waypoints) {
            if (! use_transform(p)) { continue; }
            float dist = DistanceToPoint(start_point, p.position);
            if (dist < closest_distance) {
                closest_distance = dist;
                closet_destination = p;
            }
        }
        return closet_destination;
    }

    public float DistanceToPoint(Vector3 start_point, Vector3 end_point) {
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