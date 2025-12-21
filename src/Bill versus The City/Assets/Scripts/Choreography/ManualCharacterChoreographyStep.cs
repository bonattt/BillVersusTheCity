

using UnityEngine;

public class ManualCharacterChoreographyStep : AbstractChoreographyStep {
    public ManualCharacterMovement character_controller;

    [SerializeField]
    private Transform _destination;
    public Transform destination { get => _destination; }

    private const float arrival_threashold = 0.4f;

    public float debug__distance_to_destination = -1f;

    void Update() {
        if (!active || choreography_complete) { return; } // if the choreography step has not yet been activated, or was already completed, do nothing.

        debug__distance_to_destination = PhysicsUtils.FlatDistance(character_controller.transform.position, destination.position);
        if (PhysicsUtils.FlatDistance(character_controller.transform.position, destination.position) < arrival_threashold) {
            Complete();
            return;
        }
        Vector3 move_direction = destination.position - character_controller.transform.position;
        character_controller.MoveCharacter(move_direction, look_direction: move_direction, sprint: false, crouch: false, walk: true);
    }

    public override void Activate(IChoreography choreography) {
        base.Activate(choreography);
        character_controller.time_scale_setting = TimeScaleSetting.unscaled_time;
    }

    public override void Complete() {
        base.Complete();
        // character animation continues to play until a zero'd move is called, because MoveCharacter is expected to be called on every Update loop
        Vector3 move_direction = destination.position - character_controller.transform.position;
        character_controller.MoveCharacter(Vector3.zero, look_direction: move_direction, sprint: false, crouch: false, walk: true);
        character_controller.time_scale_setting = TimeScaleSetting.scaled_time;
    }

    protected override void ImplementSkip() {
        base.ImplementSkip();
        character_controller.TeleportTo(destination.position);
    }
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(destination.position, 0.4f);
    }
}