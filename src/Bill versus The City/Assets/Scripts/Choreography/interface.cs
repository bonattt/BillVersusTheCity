


using UnityEngine;

public abstract class AbstractChoreographyStep : MonoBehaviour, IChoreographyStep {
    // interface for a single step of choreography

    [SerializeField]
    private bool _active = false;
    public bool active {
        get => _active;
        private set {
            _active = value;
        }
    }

    [SerializeField]
    private bool _choreography_complete = false;
    public bool choreography_complete {
        get => _choreography_complete;
        private set { _choreography_complete = value; }
    }
    protected IChoreography choreography;
    public virtual void Activate(IChoreography choreography) {
        active = true;
        this.choreography = choreography;
    }
    public virtual void Complete() { choreography_complete = true; }
    
    public virtual bool activate_when_skipped { get => false; }
    public void SkipStep(IChoreography choreography) {
        if (!active) {
            if (activate_when_skipped) {
                Activate(choreography);
            } else {
                active = true;
                this.choreography = choreography;
            }
        }
        ImplementSkip();
        if (!choreography_complete) {
            Complete();
        }
    }

    protected virtual void ImplementSkip() {
        // do nothing by default
    }
    
    public virtual void OnChoreographyStart(IChoreography choreography) { /* do nothing by default */ }
    public virtual void OnChoreographyComplete(IChoreography choreography) { /* do nothing by default */ }

    protected virtual void Start() {
        // do nothing. Setting active and complete in start risks overwriting these values set from another Start method that runs first
    }
}


public interface IChoreography : IGameEventEffect, IInteractionEffect {
    // full set of choreography steps
    public Transform camera_follow_target { get; set; }
    public Vector3 camera_offset { get; set; }
    public ChoreographyCameraMode camera_mode { get; set; }
    public Camera GetCamera();
    public CameraFollowZoom GetCameraScript();
    public void Activate();

}


public interface IChoreographyStep {
    public bool active { get; }

    public bool choreography_complete { get; }

    public void Activate(IChoreography choreography);
    public void Complete();
    public void SkipStep(IChoreography choreography);
    public void OnChoreographyStart(IChoreography choreography);
    public void OnChoreographyComplete(IChoreography choreography);
}