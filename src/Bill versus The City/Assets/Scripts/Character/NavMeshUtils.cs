
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

    
    public static Vector3 GetRetreatWithCover(Vector3 start, Vector3 cover_from) {
        int n_rays = 30;
        return GetRetreatWithCover(start, cover_from, n_rays);
    }
    public static Vector3 GetRetreatWithCover(Vector3 start, Vector3 cover_from, int n_rays) {
        bool start_has_cover = PositionHasCoverFrom(start, cover_from, draw_debug_ray: true);
        if (start_has_cover) { return start; }

        List<Vector3> cover_positions = GetCoverPositions(cover_from, n_rays);
        if (cover_positions.Count == 0) {
            return start + (5 * (cover_from - start).normalized); // if there are no cover positions, just try to move away
        }
        float best_distance_score = float.NegativeInfinity;
        Vector3 best_position = cover_positions[0];
        foreach (Vector3 position in cover_positions) {
            float distance_score = CoverDistanceScore(position, start, cover_from);
            if (distance_score > best_distance_score) {
                best_distance_score = distance_score;
                best_position = position;
            }
        }
        return best_position;
    }

    private static float CoverDistanceScore(Vector3 destination, Vector3 start_pos, Vector3 cover_from) {
        // returns a "score" for ranking the best destination point for an agent trying to move from `start_pos` to `destination` while 
        // avoiding `cover_from`.
        // method only factors distance.
        float avoidance_score = Mathf.Sqrt(2 * Vector3.Distance(destination, cover_from)); // score for how far the point to be avoided is from the destination
        float travel_score = Vector3.Distance(destination, cover_from); // score for how far the destination is from the start
        
        // high avoidance score is good, b/c we want to avoid, but high travel_score is bad b/c it means we have to travel a long way to reach safety
        return avoidance_score - travel_score;  
    }

    private static List<Vector3> GetCoverPositions(Vector3 cover_from, int n_rays) {
        // returns a list of Vector3's which have cover from `cover_from` AND are valid positions on a navmesh
        List<Vector3> rays = GetNDirections(n_rays);
        List<Vector3> valid_positions = new List<Vector3>();
        foreach (Vector3 r in rays) {
            if (Physics.Raycast(cover_from, r, out RaycastHit raycast_hit, float.PositiveInfinity)) {
                if (! RaycastHitsPlayer(raycast_hit)) {
                    float radius = 1f;
                    Vector3 point_past_cover = raycast_hit.point + r.normalized * radius; // move `radius` units past the cover
                    if (NavMesh.SamplePosition(point_past_cover, out NavMeshHit navmesh_hit, radius, NavMesh.AllAreas)) { 
                        Vector3 valid_p = navmesh_hit.position;
                        // retest that the navmesh position still has cover
                        if (Physics.Raycast(cover_from, valid_p, out RaycastHit raycast_hit2, float.PositiveInfinity)) {
                            if (! RaycastHitsPlayer(raycast_hit)) {
                                valid_positions.Add(valid_p);
                            }
                        }
                    }
                }
            }
        }
        return valid_positions;
    }

    public static bool RaycastHitsPlayer(RaycastHit hit) {
        // TODO --- find a better home for this helper method
        Transform hit_transform = hit.transform;
        while (hit_transform.parent != null) {
            hit_transform = hit_transform.parent;
        }
        Debug.LogWarning($"cover raycast hit {hit_transform.gameObject.name}!"); // TODO --- remove debug
        return hit_transform.gameObject.GetComponent<PlayerMovement>() != null;
    }

    public static bool PositionHasCoverFrom(Vector3 cover_from, Vector3 end, bool draw_debug_ray = false) {
        Vector3 raycast_direction = cover_from - end;
        RaycastHit hit;
        float ray_length = raycast_direction.magnitude;

        if (draw_debug_ray) { Debug.DrawRay(cover_from, raycast_direction, Color.cyan, ray_length); }
        if (Physics.Raycast(cover_from, raycast_direction, out hit, raycast_direction.magnitude)) {
            // if raycast hits something other than the player
            return !RaycastHitsPlayer(hit);
        }
        return false; // raycast hit nothing, or hit the player, so there is not cover
    }

    public static List<Vector3> GetNDirections(int n) {
        // returns a list of n equidistant unit-vectors pointing in all directions 
        // TODO --- find a better home for this helper method
        List<Vector3> vectors = new List<Vector3>();
        float incriment = 360f / n;  // degrees of rotation between each vector
        Vector3 base_vector = new Vector3(1f, 0, 0);

        for (int i = 0; i < n; i++) {
            float angle = i * incriment;
            Debug.LogWarning($"i = {i}: angle {angle}"); // TODO --- remove debug
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            vectors.Add(rotation * base_vector);
        }
        return vectors;
    }
}