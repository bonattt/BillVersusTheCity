using UnityEngine;

public class WaitChoreographyStep : AbstractChoreographyStep {

    [Tooltip("number of seconds waited before this choreography step is complete.")]
    public float wait_for_seconds = 0.75f;
    private float wait_start_time = 0f;
    
    public override bool activate_when_skipped { get => false; }
    public override void Activate(IChoreography choreography) {
        base.Activate(choreography);
        wait_start_time = Time.unscaledTime;
    }

    void Update() {
        if (!active || choreography_complete) { return; }
        if (wait_start_time + wait_for_seconds <= Time.unscaledTime) {
            Complete();
        }
    }
}
