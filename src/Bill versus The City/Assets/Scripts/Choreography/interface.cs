


using UnityEngine;

public abstract class AbstractChoreographyStep : MonoBehaviour {
    // interface for a single step of choreography

    [SerializeField]
    private bool _active = false;
    public bool active {
        get => _active;
        private set { _active = value; }
    }
    
    [SerializeField]
    private bool _complete = false;
    public bool complete {
        get => _complete;
        private set { _complete = value; }
    }
    protected IChoreography choreography;
    public virtual void Activate(IChoreography choreography) {
        active = true;
        this.choreography = choreography;
    }
    public virtual void Complete() { complete = true; }

    protected virtual void Start() {
        active = false;
        complete = false;
    }
}


public interface IChoreography {
    // full set of choreography steps
    public Transform camera_follow_target { get; set; }
    public ChoreographyCameraMode camera_mode { get; set; }
}