

using System.Collections.Generic;
using UnityEngine;

public class SequentialChoreographyStep : AbstractChoreographyStep {
    public List<AbstractChoreographyStep> choreography_steps;

    [Tooltip("the index in `choreography_steps` the choreography is currently handling. (published in inspector for debuging purposes)")]
    [SerializeField]
    private int _choreography_index = 0;
    public int choreography_index {
        get => _choreography_index;
        private set { _choreography_index = value; }
    }

    public override bool activate_when_skipped { get => false; }
    void Update() {
        if (!active || choreography_complete) { return; }

        AbstractChoreographyStep step = choreography_steps[choreography_index];
        if (!step.active) { step.Activate(this.choreography); }

        if (step.choreography_complete) {
            if (IsOnFinalStep()) {
                Complete();
            } else {
                choreography_index += 1;
            }
        }
    }

    protected override void ImplementSkip() {
        base.ImplementSkip(); 
        foreach (AbstractChoreographyStep step in choreography_steps) {
            if (step.choreography_complete) { continue; } // don't call skip on steps already completed
            step.SkipStep(this.choreography);
        }
    }

    public bool IsOnFinalStep() {
        return choreography_index == choreography_steps.Count - 1;
    }

    public IEnumerable<IChoreographyStep> AllSteps() {
        foreach (IChoreographyStep step in choreography_steps) {
            yield return step; 
        }
    }
}