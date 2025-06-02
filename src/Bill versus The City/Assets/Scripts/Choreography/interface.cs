


using UnityEngine;

public abstract class AbstractChoreographyStep : MonoBehaviour {
    // interface for a single step of choreography

    [SerializeField]
    private Transform _destination;
    public Transform destination { get => _destination; }

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
    public virtual void Activate() { active = true; }
    public virtual void Complete() { complete = true; }

    void Start() {
        active = false;
        complete = false;
    }
}


public interface IChoreography {
    // full set of choreography steps
}