

using UnityEditor.EditorTools;
using UnityEngine;

public class MoveCameraChoreographyStep : AbstractChoreographyStep
{
    [Tooltip("positon to move the camera to. Y component will be ignored, and the base camera height will be used.")]
    public Transform destination;
    [Tooltip("interpolation in move to possition. 0 will be instantaneous transfer.")]
    public float slerp;
    [Tooltip("change in Y coordinates from base camera height")]
    public float zoom;

    private Camera camera_;
    private Vector3 original_position;
    
    public override void Activate(IChoreography choreography) {
        base.Activate(choreography);
        SetupCameraMovement();
    }

    public Camera GetCamera() {
        return Camera.main;
    }

    private void SetupCameraMovement() {
        camera_ = GetCamera();
        original_position = camera_.transform.position;
        choreography.camera_mode = ChoreographyCameraMode.manual;
        // choreography.camera_offset = camera_offset;
        // if (camera_target != null) {
        //     choreography.camera_follow_target = camera_target;
        // }
    }

    private void MoveCamera() {
        float base_hieght = camera_.transform.position.y;
        camera_.transform.position = new Vector3(destination.position.x, base_hieght + zoom, destination.position.z);
    }

    void Update() {
        if (!active || choreography_complete) { return; }

    }
}