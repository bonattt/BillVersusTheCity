using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public static class NavMeshUtils {

    private const int DEFAULT_MAX_TRIES = 10;
    
    public const string NAV_MESH_DOORWAY_AREA = "Doorway";
    // NavMeshAreaMask (not LayerMask) for propegation of sounds.
    // NOTE unlike LayerMask, NavMesh area masks are just an int, and you have to set them up manually. handy dandy Inspector UI is not supported.
    // public static int nav_mesh_sound_area_mask = NavMesh.AllAreas & ~(1 << NavMesh.GetAreaFromName(NAV_MESH_DOORWAY_AREA));
    public static int nav_mesh_sound_area_mask {
        get => LayerMaskSystem.inst.nav_mesh_sound_area_mask;
    }

    public static Vector3 GetRandomPoint(NavMeshAgent agent, float radius) {
        return GetRandomPoint(agent, radius, agent.transform.position, DEFAULT_MAX_TRIES);
    }
    public static Vector3 GetRandomPoint(NavMeshAgent agent, float radius, int max_tries) {
        return GetRandomPoint(agent, radius, agent.transform.position, max_tries);
    }
    public static Vector3 GetRandomPoint(NavMeshAgent agent, float radius, Vector3 origin) {
        return GetRandomPoint(agent, radius, origin, DEFAULT_MAX_TRIES);
    }

    public static Vector3 GetRandomPoint(NavMeshAgent agent, float radius, Vector3 origin, int max_tries, int nav_mesh_area_mask = NavMesh.AllAreas) {
        // Gets a random position within a radius which is actually reachable by a navmesh agent.

        for (int i = 0; i < max_tries; i++) // Try up to 10 random points
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += origin;
            // TODO --- fix this bad ChatGPT code
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, nav_mesh_area_mask)) {
                // Check if the agent can reach the point
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete) {
                    return hit.position;
                }
            }
        }
        // if a random position fails after `max_tries`, try sampling at the origin
        (bool success, Vector3 origin_pos) = _SamplePosition(agent, origin, radius, nav_mesh_area_mask);
        if (success) { 
            return origin_pos; 
        }
        else { 
            return new Vector3(float.NaN, float.NaN, float.NaN); 
        }
    }

    private static (bool, Vector3) _SamplePosition(NavMeshAgent agent, Vector3 position, float radius, int nav_mesh_area_mask) {
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, radius, nav_mesh_area_mask)) {
            // Check if the agent can reach the point
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete) {
                return (true, hit.position);
            }
        }
        return (false, new Vector3(float.NaN, float.NaN, float.NaN));
    }

    public static float GetPathLength(NavMeshPath path) {
        float length = 0f;
        for (int i = 1; i < path.corners.Length; i++)  {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return length;
    }

    public static void DrawPath(NavMeshPath path, Color color, float duration = 0f) {
        // draws a navmesh path using Debug.DrawLine
        for (int i = 1; i < path.corners.Length; i++) {
            Debug.DrawLine(path.corners[i -1], path.corners[i], color, duration);
        }
    }

    public static bool WillBecomeCornered(EnemyBehavior parent, ManualCharacterMovement player, float towards_player_threshold) {
        // if (is_cornered) { return false; } // already cornered, don't BECOME cornered
        Vector3 travel_direction = parent.movement_script.nav_mesh_agent.velocity.normalized;
        Vector3 toward_player = (player.transform.position - parent.transform.position).normalized;

        float dot = Vector3.Dot(toward_player, travel_direction);
        return dot >= towards_player_threshold;
    }

    public static bool RaycastTowardPlayer(EnemyBehavior enemy, ManualCharacterMovement player, out RaycastHit hit, bool debug_ray = false) {
        Vector3 start_pos = new Vector3(enemy.transform.position.x, 0.85f, enemy.transform.position.z);
        Vector3 end_pos = new Vector3(player.transform.position.x, .85f, player.transform.position.z);
        Vector3 towards_player = end_pos - start_pos;
        if (debug_ray) {
            Debug.DrawRay(start_pos, towards_player, Color.red, 0.25f);
        }
        return Physics.Raycast(start_pos, towards_player, out hit, towards_player.magnitude, LayerMaskSystem.inst.has_cover_raycast);
    }

    public static bool RaycastHitsPlayer(RaycastHit hit) {
        // TODO --- find a better home for this helper method
        Transform hit_transform = hit.transform;
        while (hit_transform.parent != null) {
            hit_transform = hit_transform.parent;
        }
        bool result = hit_transform.gameObject.GetComponent<ManualCharacterMovement>() != null;
        return result;
    }

    public static bool PositionHasCoverFrom(Vector3 cover_from, Vector3 position, bool draw_debug_ray = false) {
        float ray_length = (position - cover_from).magnitude;
        return _PositionHasCover(cover_from, position, LayerMaskSystem.inst.has_cover_raycast, ray_length, draw_debug_ray:draw_debug_ray);
    }

    public static bool PositionHasSoftCover(Vector3 cover_from, Vector3 position, bool draw_debug_ray = false) {
        // determines if `position` has cover from `cover_from` while crouching.
        // `cover` should be near the `position` for it to apply.
        return _PositionHasCover(cover_from, position, LayerMaskSystem.inst.soft_cover_raycast, ray_length:4f, draw_debug_ray:draw_debug_ray);
    }

    private static bool _PositionHasCover(Vector3 cover_from, Vector3 position, LayerMask layer_mask, float ray_length, bool draw_debug_ray = false) {
        // move raycast points to position just above the ground to avoid treating the floor as cover
        cover_from = new Vector3(cover_from.x, 1f, cover_from.z);
        position = new Vector3(position.x, 1f, position.z);

        Vector3 raycast_direction = position - cover_from;
        RaycastHit hit;
        // float ray_length = raycast_direction.magnitude;

        if (draw_debug_ray) { Debug.DrawRay(cover_from, raycast_direction, Color.cyan, Time.deltaTime); }
        if (Physics.Raycast(cover_from, raycast_direction, out hit, raycast_direction.magnitude, layer_mask)) {
            // if raycast hits something other than the player
            return !RaycastHitsPlayer(hit);
        }
        return false;
    }

    public static List<Vector3> GetNDirections(int n) {
        // returns a list of n equidistant unit-vectors pointing in all directions 
        // TODO --- find a better home for this helper method
        List<Vector3> vectors = new List<Vector3>();
        float incriment = 360f / n;  // degrees of rotation between each vector
        Vector3 base_vector = new Vector3(1f, 0, 0);

        for (int i = 0; i < n; i++) {
            float angle = i * incriment;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            vectors.Add(rotation * base_vector);
        }
        return vectors;
    }


    public static Vector3 DestinationAwayFromPosition(EnemyBehavior parent, Vector3 position_to_evade) {
        Vector3 toward_pos = (position_to_evade - parent.transform.position).normalized;
        Debug.DrawRay(parent.transform.position, toward_pos, Color.red);

        Vector3 raycast_start = new Vector3(parent.transform.position.x, 0.5f, parent.transform.position.z);
        Vector3 raycast_target = new Vector3(parent.transform.position.x, 0.5f, parent.transform.position.z);
        Vector3 direction = (raycast_target - raycast_start);
        float magnitude = direction.magnitude;
        RaycastHit hit;
        if (Physics.Raycast(raycast_start, direction.normalized, out hit, magnitude)) {
            Vector3 destination = hit.point - (direction.normalized / 2);
            return new Vector3(destination.x, 0, destination.z);
        } else {
            return parent.transform.position + (-toward_pos * 3);
        }
    }

    public static bool IsPathBlocked(NavMeshAgent agent) {
        return agent.pathStatus != NavMeshPathStatus.PathInvalid;
    }
}
