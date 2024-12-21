
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public static class NavMeshUtils {

    private const int DEFAULT_MAX_TRIES = 10;

    public static Vector3 GetRandomPoint(NavMeshAgent agent, float radius) {
        return GetRandomPoint(agent, radius, agent.transform.position, DEFAULT_MAX_TRIES);
    }
    public static Vector3 GetRandomPoint(NavMeshAgent agent, float radius, int max_tries) {
        return GetRandomPoint(agent, radius, agent.transform.position, max_tries);
    }
    public static Vector3 GetRandomPoint(NavMeshAgent agent, float radius, Vector3 origin) {
        return GetRandomPoint(agent, radius, origin, DEFAULT_MAX_TRIES);
    }

    public static Vector3 GetRandomPoint(NavMeshAgent agent, float radius, Vector3 origin, int max_tries) {
        // Gets a random position within a radius which is actually reachable by a navmesh agent.

        for (int i = 0; i < max_tries; i++) // Try up to 10 random points
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += origin;
            // TODO --- fix this bad ChatGPT code
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                // Check if the agent can reach the point
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    return hit.position;
                }
            }
        }

        // If no valid point is found after 10 attempts, return Vector3.zero
        return Vector3.zero;
    }

    public static bool CanRetreatFrom(Vector3 start_point, Vector3[] run_from) {
        return CanRetreatFrom(start_point, run_from, 0.99f);
    }
    public static bool CanRetreatFrom(Vector3 start_point, Vector3[] run_from, float threashold) {
        // returns a boolean whether it is possible to retreat from threats in all the given positions.
        Vector3 final_escape_vector = GetEscapeVector(start_point, run_from);
        return final_escape_vector.normalized.magnitude >= threashold;
    }

    public static Vector3 GetEscapeVector(Vector3 start_point, Vector3 run_from) {
        Vector3[] as_array = new Vector3[]{run_from};
        return GetEscapeVector(start_point, as_array);
    }
    public static Vector3 GetEscapeVector(Vector3 start_point, Vector3[] run_from) {
        // returns a vector roughly poiting away from all given threats. Vector is NOT normalized.
        Vector3 final_escape_vector = new Vector3(0, 0, 0);
        foreach (Vector3 v in run_from) {
            Vector3 escape_vector = (start_point - v).normalized;
            final_escape_vector += escape_vector;
            // Debug.Log($"escape away: {escape_vector}"); // TODO --- remove debug
        }
        // Debug.Log($"final_escape_vector: {final_escape_vector}"); // TODO --- remove debug
        return final_escape_vector;
    }

    public static Vector3 GetMultiRetreatPoint(Vector3 start_point, Vector3[] run_from) {
        // gets the best escape point from several un-weighted threats
        Vector3 escape_vector = GetEscapeVector(start_point, run_from);

        Vector3 near_escape_point = start_point + escape_vector;
        float radius = 1f;
        if(NavMesh.SamplePosition(near_escape_point, out NavMeshHit hit, radius, NavMesh.AllAreas)) {
            return hit.position; // return the first val
        }
        // Debug.LogWarning($"{near_escape_point} is not on the nav mesh!"); // TODO --- remove debug
        return near_escape_point; // TODO --- this is not actually a valid NavMeshPoint
    }

    public static Vector3 GetRetreatWithCover(Vector3 start_point, Vector3 cover_from) {
        // gets a position on the NavMesh to take cover against `cover_from`
        int max_tries = 6;  // arbitrary max_tries to find a cover position
        Vector3 escape_vector = GetEscapeVector(start_point, cover_from).normalized;
        (Vector3 escape_left, Vector3 escape_right) = GetPerpendicularVectors(escape_vector);
        Vector3 escape_point = start_point + escape_vector;
        float radius = 1f;
        if(NavMesh.SamplePosition(escape_point, out NavMeshHit hit, radius, NavMesh.AllAreas)) {
            escape_point = hit.position;
        }

        for (int i = 1; i < max_tries + 1; i++) {
            Vector3 p_forward = start_point + (escape_vector * i);
            Vector3 p_left = start_point + (escape_left * i);
            Vector3 p_right = start_point + (escape_right * i);

            List<Vector3> forward_points = _GetCanidatePoints(p_forward, i, i);
            List<Vector3> left_points = _GetCanidatePoints(p_left, i, i);
            List<Vector3> right_points = _GetCanidatePoints(p_right, i, i);

            List<Vector3> valid_points = new List<Vector3>();
            _TryCoverPoints(cover_from, left_points, valid_points);
            _TryCoverPoints(cover_from, right_points, valid_points);
            _TryCoverPoints(cover_from, forward_points, valid_points);

            if (valid_points.Count == 0) { continue; }

            float min_distance = float.PositiveInfinity;
            Vector3 closest_point = new Vector3(float.NaN, 0, 0);
            foreach(Vector3 p in valid_points) {
                float dist = Vector3.Distance(start_point, p);
                if (dist <= min_distance) {
                    closest_point = p;
                    min_distance = dist;
                }
            }
            Debug.Log($"found retreat point on iteration i = {i} / max = {max_tries}"); // TODO --- remove debug
            return closest_point;
        }
        Debug.LogWarning($"failed to find valid retreat in {max_tries} iterations"); // TODO --- remove debug
        return escape_point;
    }

    private static List<Vector3> _TryCoverPoints(Vector3 cover_from, List<Vector3> trial_points, List<Vector3> valid_points) {
        // tries a list of points, and returns a list containing only points which have no line-of-sight to `cover_point`
        // ADDs all valid points to an exiting list that is passed in.
        foreach(Vector3 v in trial_points) {
            // NavMeshHit hit;
            // checks that 1) point is on NavMesh and 2) there is also collision between the points.
            if (NavMesh.Raycast(cover_from, v, out NavMeshHit _, NavMesh.AllAreas)) {
                valid_points.Add(v);
            }
        }
        return valid_points;
    }

    private static List<Vector3> _GetCanidatePoints(Vector3 center, float radius, int random_points ) {
        List<Vector3> canidates = new List<Vector3>();
        canidates.Add(center);
        for (int i = 0; i < random_points; i++) {
            (Vector3 p, float distance) = _GetRandomPointInRadiusWithDistance(center, radius);
            if (! float.IsNaN(p.x)) {
                canidates.Add(p);
            }
        }
        return canidates;
    }

    private static (Vector3, float) _GetRandomPointInRadiusWithDistance(Vector3 center, float radius) {
        // returns a single random point within {radius} distance of {center}, and the distance from center the point is.
        NavMeshHit hit;
        Vector3 random_point = center + (Random.insideUnitSphere * radius);
        if (NavMesh.SamplePosition(random_point, out hit, radius, NavMesh.AllAreas)) {
            return (hit.position, Vector3.Distance(center, hit.position));
        }
        return (new Vector3(float.NaN, float.NaN, float.NaN), float.PositiveInfinity);
    }

    private static Vector3 _GetRandomPointInRadius(Vector3 center, float radius, int max_tries) {
        // tries {max_tries} times to find a point on the NavMesh inside {radius}
        NavMeshHit hit;
        for (int j = 0; j < max_tries; j++) {
            Vector3 random_point = center + (Random.insideUnitSphere * radius);
            if (NavMesh.SamplePosition(random_point, out hit, radius, NavMesh.AllAreas)) {
                return hit.position;
            }
        }
        return new Vector3(float.NaN, float.NaN, float.NaN);
    }


    public static (Vector3, Vector3) GetPerpendicularVectors(Vector3 base_vector) {
        // returns the vectors perpendicular to the base vector, and returns both.
        // returns the "left" then "right" vector in that order.
        // "left" is rotated 90 degrees anti-clockwise, and "right" is rotated 90 degrees clockwise
        return (new Vector3(-base_vector.z, 0, base_vector.x).normalized, new Vector3(base_vector.z, 0, -base_vector.x).normalized);
    }
}