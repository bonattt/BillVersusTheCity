

using UnityEngine;

public class CharacterControllerChoreographyStep : AbstractChoreographyStep {
    // ChoreographyStep for moving a Unity Character Controller

    public Transform destination;
    public float speed;
    public CharacterController char_ctrl;

    public const float arrival_threashold = 0.5f;

    public float debug__distance_to_destination = -1f;

    void Update() {
        if (!active || complete) { return; } // if the choreography step has not yet been activated, or was already completed, do nothing.

        float dist = FlatDistance(destination.position, char_ctrl.transform.position);
        if (dist <= arrival_threashold) {
            Complete();
            return;
        }
        debug__distance_to_destination = dist;
        Debug.LogWarning($"TODO --- implement rotation???"); // TODO --- remove debug
        Vector3 move_direction = destination.position - char_ctrl.transform.position;
        move_direction = new Vector3(move_direction.x, 0f, move_direction.z).normalized;
        char_ctrl.Move(move_direction * speed * Time.deltaTime);
    }

    private float FlatDistance(Vector3 a, Vector3 b) {
        a = new Vector3(a.x, 0, a.z);
        b = new Vector3(b.x, 0, b.z);
        return Vector3.Distance(a, b);
    }
}