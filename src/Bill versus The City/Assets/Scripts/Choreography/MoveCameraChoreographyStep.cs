

using System;
using UnityEditor.EditorTools;
using UnityEngine;

public class MoveCameraChoreographyStep : AbstractChoreographyStep
{
    [Tooltip("positon to move the camera to. Y component will be ignored, and the base camera height will be used.")]
    public Transform destination;

    [Tooltip("interpolation in move to possition. 1 will be instantaneous transfer.")]
    public float lerp = 0.05f;
    [Tooltip("change in Y coordinates from base camera height")]
    public float zoom;

    private Camera camera_;
    private Vector3 original_position;
    private ChoreographyCameraMode original_mode;
    
    public override void Activate(IChoreography choreography) {
        base.Activate(choreography);
        SetupCameraMovement(choreography);
        debug.frames_count = 0;
        debug.move_start_time = Time.time;
    }
    
    public override void Complete() {
        base.Complete();
        // TODO --- do I need to do anything here?
        debug.move_end_time = Time.time;
        float duration = debug.move_end_time - debug.move_start_time;
        debug.move_duration = Mathf.Round(duration * 100) / 100; // round to 2 decimal places
    }

    private void SetupCameraMovement(IChoreography choreography) {
        camera_ = choreography.GetCamera();

        original_mode = choreography.camera_mode;
        original_position = camera_.transform.position;
        
        choreography.camera_mode = ChoreographyCameraMode.manual;
    }

    public Vector3 target_position {
        get {
            float base_hieght = original_position.y;
            return new Vector3(destination.position.x, base_hieght + zoom, destination.position.z);
        }
    }

    private void MoveCamera() {
        camera_.transform.position = Vector3.Lerp(camera_.transform.position, target_position, lerp);
    }

    public bool HasReachedTarget() {
        return PhysicsUtils.VectorEquals(target_position, camera_.transform.position);
    }

    void Update() {
        if (!active || choreography_complete) { return; }
        MoveCamera();
        if (HasReachedTarget()) {
            Complete();
        }
        debug.frames_count += 1;
    }

    public MoveCameraStepDebug debug;
}

[Serializable]
public class MoveCameraStepDebug {
    public int frames_count;
    public float move_start_time, move_end_time, move_duration;
}