

using System.Collections.Generic;
using UnityEngine;

public class Choreography : MonoBehaviour, IChoreography {
    public List<AbstractChoreographyStep> choreography_steps;

    [Tooltip("the index in `choreography_steps` the choreography is currently handling. (published in inspector for debuging purposes)")]
    [SerializeField]
    private int choreography_index = 0;

    public bool active { get; private set; }
    public bool complete { get; private set; }
    public void Activate() { active = true; }

    void Start() {
        active = false;
        complete = false;
    }


    void Update() {
        if (!active || complete) { return; }

        AbstractChoreographyStep step = choreography_steps[choreography_index];
        if (!step.active) { step.Activate(); }

        if (step.complete) {
            Debug.LogWarning("next choreography step"); // TODO --- remove debug
            choreography_index += 1;
        }
    }

    public bool HasNextStep() {
        return choreography_index < choreography_steps.Count - 1;
    }

    public bool OnFinalStep() {
        return choreography_index == choreography_steps.Count - 1;
    }
}