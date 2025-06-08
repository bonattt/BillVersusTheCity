

using UnityEngine;

public class NavMeshCharacterChoreographyStep : AbstractChoreographyStep {
    public SimpleNavMeshAgentMovement character_controller;

    [SerializeField]
    private Transform _destination;
    public Transform destination { get => _destination; }
    private const float arrival_threashold = 0.1f;
    public float debug__distance_to_destination;

    void Update() {
        if (!active || choreography_complete) { return; } // if the choreography step has not yet been activated, or was already completed, do nothing.

        float distance_to_dest = FlatDistance(character_controller.transform.position, destination.position);
        debug__distance_to_destination = distance_to_dest;
        if (distance_to_dest <= arrival_threashold) {
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
        character_controller.choreography_movement = true;
        character_controller.MoveCharacter(destination.position, move_direction, sprint: false, crouch: false);
    }

    public static float FlatDistance(Vector3 first, Vector3 second) {
        // negates the Y-axis component before finding the distance between two points
        first = new Vector3(first.x, 0f, first.z);
        second = new Vector3(second.x, 0f, second.z);
        return Vector3.Distance(first, second);
    }

}