

using UnityEngine;

public class MoveVehicleStep : AbstractChoreographyStep
{
    public Transform target_to_move;
    public Transform destination;
    public float base_movement_speed, start_acceleration, stop_deceleration;
    public VehicleMovementProfile start_profile, stop_profile;

    private Vector3 speed;

    public override void Activate(IChoreography choreography) {
        base.Activate(choreography);
        target_to_move.gameObject.SetActive(true);
    }
    public override void Complete() {
        base.Complete();
    }

    void Update() {
        if (!active || choreography_complete) { return; }
        MoveVehicle();
        if (HasReachedDestination()) {
            Complete();
        }
    }

    private bool HasReachedDestination() {
        return PhysicsUtils.VectorEquals(target_to_move.position, destination.position);
    }

    private void MoveVehicle() {
        Vector3 direction = (destination.position - target_to_move.position).normalized;
        target_to_move.position += direction * base_movement_speed * Time.deltaTime;
        // TODO --- implement acceleration
    }

}

public enum VehicleMovementProfile {
    flat, // move without physics, start and stop instantly
    acceleration, // accelerate into movement, decellerate for stops
}