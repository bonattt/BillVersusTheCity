using UnityEngine;

public class UpdateCameraChoreographyStep : AbstractChoreographyStep {

    public ChoreographyCameraMode camera_mode = ChoreographyCameraMode.manual;
    public Transform camera_target = null;
    public override void Activate(IChoreography choreography) {
        base.Activate(choreography);
        choreography.camera_mode = camera_mode;
        if (camera_target != null) {
            choreography.camera_follow_target = camera_target;
        }
        Complete();
    }
}