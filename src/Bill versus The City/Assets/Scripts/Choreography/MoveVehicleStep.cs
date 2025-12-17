

using System;
using UnityEngine;

public class MoveVehicleStep : AbstractChoreographyStep
{
    public Transform target_to_move;
    public Transform destination;
    public float base_movement_speed, start_acceleration, stop_deceleration;
    public VehicleMovementProfile start_profile = VehicleMovementProfile.flat;
    public VehicleMovementProfile stop_profile = VehicleMovementProfile.acceleration;

    private Vector3 velocity, delta_v;
    private float _stopping_distance;
    private bool fully_stopped = false;

    public override void Activate(IChoreography choreography) {
        base.Activate(choreography);
        target_to_move.gameObject.SetActive(true);
        _stopping_distance = CalculateStoppingDistance();
        SetInitialSpeed();
    }
    public override void Complete() {
        base.Complete();
        debug.movement_stage = "complete";
    }

    void Update() {
        if (!active || choreography_complete) { return; }
        MoveVehicle();
        if (fully_stopped || HasReachedDestination()) {
            Complete();
        }
        UpdateDebug();
    }

    private void SetInitialSpeed() {
        delta_v = Vector3.zero;
        fully_stopped = false;
        if (start_profile == VehicleMovementProfile.acceleration) {
            velocity = new Vector3(0, 0, 0);
        } else if (start_profile == VehicleMovementProfile.flat) {
            velocity = movement_direction * base_movement_speed;
        } else {
            Debug.LogError($"unhandled vehicle start_profile: '{start_profile}'");
            velocity = movement_direction * base_movement_speed;
        }
    }

    private bool HasReachedDestination() {
        return PhysicsUtils.VectorEquals(target_to_move.position, destination.position);
    }

    private Vector3 movement_direction { get => (destination.position - target_to_move.position).normalized; }

    private void MoveVehicle() {
        float distance_to = DistanceToDestination();
        if (stop_profile == VehicleMovementProfile.acceleration && _stopping_distance >= distance_to) {
            // nearing target, start decellerating
            delta_v = movement_direction * stop_deceleration * Time.deltaTime;
            if (delta_v.magnitude > velocity.magnitude) {
                velocity = Vector3.zero;
                debug.movement_stage = "stopped";
                fully_stopped = true;
            } else {
                velocity -= delta_v;
                debug.movement_stage = "decelerating";
            }
        }
        else if (start_profile == VehicleMovementProfile.acceleration && velocity.magnitude < base_movement_speed) {
            // accelerate up to target speed
            delta_v = movement_direction * start_acceleration * Time.deltaTime;
            velocity += delta_v;
            debug.movement_stage = "acceleration";
            _stopping_distance = CalculateStoppingDistance(velocity.magnitude);
        } else {
            delta_v = Vector3.zero;
            velocity = movement_direction * base_movement_speed;
            debug.movement_stage = "move";
        } 
        Debug.DrawRay(target_to_move.position, movement_direction * 5, Color.red);
        target_to_move.position += velocity * Time.deltaTime;
        // TODO --- implement acceleration
    }

    private float CalculateStoppingDistance() => CalculateStoppingDistance(base_movement_speed);
    private float CalculateStoppingDistance(float speed) {
        float accel = -Mathf.Abs(stop_deceleration);

        debug.time_to_stop = Mathf.Abs(speed / accel); // how long will it take to stop decelerating
        // debug.time_to_stop = time_to_stop;

        // // Acceleration formula: D = (accel * t^2) + (v * t) + x
        // return (accel * (time_to_stop * time_to_stop)) + (speed * time_to_stop);


        return Mathf.Abs(speed * speed / (2 * accel));
    }

    private float DistanceToDestination() {
        return Vector3.Distance(target_to_move.position, destination.position);
    }

    public MoveVehivleStepDebugger debug;

    void UpdateDebug() {
        debug.speed = velocity.magnitude;
        debug.stopping_distance = _stopping_distance;
        debug.distance_to_destination = DistanceToDestination();
        debug.delta_v = delta_v.magnitude;
    }
}

public enum VehicleMovementProfile {
    flat, // move without physics, start and stop instantly
    acceleration, // accelerate into movement, decellerate for stops
}


[Serializable]
public class MoveVehivleStepDebugger {
    public float speed = 0f;
    public float stopping_distance = 0f;
    public float distance_to_destination = 0f;
    public float delta_v;
    [Tooltip("WARNING: time_to_stop isn't used in calculation, and may not be accurate anymore.")]
    public float time_to_stop = 0f;
    public string movement_stage = "not started";
}