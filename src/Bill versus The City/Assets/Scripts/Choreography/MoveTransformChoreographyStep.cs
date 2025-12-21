

using UnityEngine;

public class MoveTransformChoreographyStep : AbstractChoreographyStep {
    // ChoreographyStep for moving a Unity Character Controller

    public Transform destination;
    public float speed = 8f;
    public Transform moved_character;

    public const float arrival_threashold = 0.5f;

    public float debug__distance_to_destination = -1f;

    void Update() {
        if (!active || choreography_complete) { return; } // if the choreography step has not yet been activated, or was already completed, do nothing.

        float dist = PhysicsUtils.FlatDistance(destination.position, moved_character.position);
        if (dist <= arrival_threashold) {
            Complete();
            return;
        }
        debug__distance_to_destination = dist;
        Debug.LogWarning($"TODO --- implement rotation???"); // TODO --- remove debug
        Vector3 move_direction = destination.position - moved_character.transform.position;
        move_direction = new Vector3(move_direction.x, 0f, move_direction.z).normalized;
        moved_character.position += move_direction * speed * Time.unscaledDeltaTime;
    }

    protected override void ImplementSkip() {
        base.ImplementSkip();
        moved_character.position = destination.position;
    }
}