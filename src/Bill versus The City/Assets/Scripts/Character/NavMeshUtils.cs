
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

    // public static Vector3 GetRetreatPoint
}