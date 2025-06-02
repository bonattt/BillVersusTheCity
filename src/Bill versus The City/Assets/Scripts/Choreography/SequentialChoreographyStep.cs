

using System.Collections.Generic;
using UnityEngine;

public class SequentialChoreographyStep : AbstractChoreographyStep {
    public List<AbstractChoreographyStep> choreography_steps;

    [Tooltip("the index in `choreography_steps` the choreography is currently handling. (published in inspector for debuging purposes)")]
    [SerializeField]
    private int choreography_index = 0;

    void Update() {
        if (!active || complete) { return; }

        AbstractChoreographyStep step = choreography_steps[choreography_index];
        if (!step.active) { step.Activate(); }

        if (step.complete) {
            if (OnFinalStep()) {
                Complete();
            } else {
                choreography_index += 1;
            }
        }
    }
    public bool OnFinalStep() {
        return choreography_index == choreography_steps.Count - 1;
    }
}