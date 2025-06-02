


using UnityEngine;

public abstract class AbstractChoreographyStep : MonoBehaviour {
    // interface for a single step of choreography

    [SerializeField]
    private Transform _destination;
    public Transform destination { get => destination; }
    public bool active { get; private set; }
    public bool complete { get; private set; }
    public virtual void Activate() { active = true; }
    public virtual void Complete() { complete = true; }
}


public interface IChoreography {
    // full set of choreography steps
}