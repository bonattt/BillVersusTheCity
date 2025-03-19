
using System.Collections.Generic;

using UnityEngine;


public class SwarmIntelligence { 


    private static SwarmIntelligence _inst = null;
    public static SwarmIntelligence inst {
        get {
            if (_inst == null) {
                _inst = new SwarmIntelligence();
            }
            return _inst;
        }
    }

    private HashSet<Transform> waypoints_investigated = new HashSet<Transform>();
    
    public Vector3 last_seen_position;
    public float last_seen_time;


    public void StartInvestigatingWaypoint(Transform waypoint) {
        waypoints_investigated.Add(waypoint);
    }

    public void SeePlayer(Vector3 position, float time) {
        last_seen_position = position;
        last_seen_time = time;
        ResetWaypoints();
    }

    private void ResetWaypoints() {
        waypoints_investigated = new HashSet<Transform>();
    }

    public Transform GetNewPoint(bool mark_investigated=true) {
        // returns a transform from WaypointSystem that hasn't been explored yet.
        List<Transform> valid_points = UncheckedWaypoints();
        if (valid_points.Count == 0) {
            ResetWaypoints();
            valid_points = WaypointSystem.inst.waypoints;
        }
        int r = (int) Random.Range(0, valid_points.Count);
        if (mark_investigated) {
            waypoints_investigated.Add(valid_points[r]);
        }
        return valid_points[r];
    }

    public List<Transform> UncheckedWaypoints() {
        // return a list of all Waypoints from WaypointSystem which are NOT in waypoints_investigated
        List<Transform> result = new List<Transform>();
        foreach (Transform t in WaypointSystem.inst.waypoints) {
            if (!result.Contains(t)) {
                result.Add(t);
            }
        }
        return result;
    }

    public IEnumerable<Transform> InvestigatedIterator() {
        foreach (Transform t in waypoints_investigated) {
            yield return t;
        }
    }
}