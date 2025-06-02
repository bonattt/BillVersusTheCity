

using UnityEngine;

public class NavMeshChoreographyStep : AbstractChoreographyStep {
    public NavMeshAgentMovement character_controller;


    void Update() {
        if (!active || complete) { return; } // if the choreography step has not yet been activated, or was already completed, do nothing.

        if (character_controller.HasReachedDestination()) {
            Complete();
            return;
        }
        UpdateNavMeshMovement();
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(destination.position, 0.4f);
    }

    private void UpdateNavMeshMovement() {
        Vector3 move_direction = destination.position - character_controller.transform.position;
        character_controller.MoveCharacter(destination.position, move_direction, sprint: false, crouch: false);
    }

    public static float FlatDistance(Vector3 first, Vector3 second) {
        // negates the Y-axis component before finding the distance between two points
        first = new Vector3(first.x, 0f, first.z);
        second = new Vector3(second.x, 0f, second.z);
        return Vector3.Distance(first, second);
    }

}