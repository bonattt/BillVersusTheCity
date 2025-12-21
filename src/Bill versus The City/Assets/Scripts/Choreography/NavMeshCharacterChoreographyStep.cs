

using UnityEngine;

public class NavMeshCharacterChoreographyStep : AbstractChoreographyStep {
    public SimpleNavMeshAgentMovement character_controller;

    [SerializeField]
    private Transform _destination;
    public Transform destination { get => _destination; }
    private const float arrival_threashold = 0.1f;
    public float debug__distance_to_destination;
    public override bool activate_when_skipped { get => false; }

    void Update() {
        if (!active || choreography_complete) { return; } // if the choreography step has not yet been activated, or was already completed, do nothing.

        float distance_to_dest = PhysicsUtils.FlatDistance(character_controller.transform.position, destination.position);
        debug__distance_to_destination = distance_to_dest;
        if (distance_to_dest <= arrival_threashold) {
            Complete();
            return;
        }
        // UpdateNavMeshMovement();
        MoveManually();
    }

    // private void UpdateNavMeshMovement() {
    //     Vector3 move_direction = destination.position - character_controller.transform.position;
    //     character_controller.choreography_movement = true;
    //     character_controller.MoveCharacter(destination.position, move_direction, sprint: false, crouch: false);
    // }

    private void MoveManually() {
        Debug.LogWarning($"TODO --- implement rotation???"); // TODO --- remove debug
        Vector3 move_direction = destination.position - character_controller.transform.position;
        move_direction = new Vector3(move_direction.x, 0f, move_direction.z).normalized;
        character_controller.transform.position += move_direction * character_controller.nav_mesh_agent.speed * Time.unscaledDeltaTime;
        character_controller.nav_mesh_agent.nextPosition = character_controller.transform.position;
    }

    public override void Activate(IChoreography choreography) {
        base.Activate(choreography);
        // character_controller.time_scale_setting = TimeScaleSetting.unscaled_time;
        character_controller.nav_mesh_agent.updatePosition = false;
        character_controller.nav_mesh_agent.updateRotation = false;
    }

    public override void Complete() {
        base.Complete();
        // character_controller.time_scale_setting = TimeScaleSetting.scaled_time;
        character_controller.nav_mesh_agent.updatePosition = true;
        character_controller.nav_mesh_agent.updateRotation = true;
    }

    protected void SkipStep() {
        character_controller.TeleportTo(destination.position);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(destination.position, 0.4f);
    }

}