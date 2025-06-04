

using UnityEngine;

public class ManualCharacterChoreographyStep : AbstractChoreographyStep {
    public ManualCharacterMovement character_controller;

    [SerializeField]
    private Transform _destination;
    public Transform destination { get => _destination; }

    private const float arrival_threashold = 0.4f;

    public float debug__distance_to_destination = -1f;

    void Update() {
        if (!active || complete) { return; } // if the choreography step has not yet been activated, or was already completed, do nothing.

        debug__distance_to_destination = FlatDistance(character_controller.transform.position, destination.position);
        if (FlatDistance(character_controller.transform.position, destination.position) < arrival_threashold) {
            Complete();
            return;
        }

        Vector3 move_direction = destination.position - character_controller.transform.position;
        character_controller.MoveCharacter(move_direction, look_direction: move_direction, sprint: false, crouch: false);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(destination.position, 0.4f);
    }

    public static float FlatDistance(Vector3 first, Vector3 second) {
        // negates the Y-axis component before finding the distance between two points
        first = new Vector3(first.x, 0f, first.z);
        second = new Vector3(second.x, 0f, second.z);
        return Vector3.Distance(first, second);
    }

}