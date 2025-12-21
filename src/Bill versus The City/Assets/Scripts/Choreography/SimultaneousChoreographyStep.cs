

using System.Collections.Generic;
using UnityEngine;

public class SimultaneousChoreographyStep : AbstractChoreographyStep {
    
    public List<AbstractChoreographyStep> choreography_steps;

    public int debug__completed = -1;

    public override bool activate_when_skipped { get => false; }
    public override void Activate(IChoreography choreography_) {
        base.Activate(choreography_);
        foreach (AbstractChoreographyStep step in choreography_steps) {
            step.Activate(choreography_);
        }
    }

    void Update() {
        if (!active || choreography_complete) { return; }

        if (AreAllStepsComplete()) {
            Complete();
        }
    }

    protected override void ImplementSkip() {
        base.ImplementSkip(); 
        foreach (AbstractChoreographyStep step in choreography_steps) {
            if (step.choreography_complete) { continue; } // don't call skip on steps already completed
            step.SkipStep(this.choreography);
        }
    }

    public bool AreAllStepsComplete() {
        debug__completed = 0;
        foreach (AbstractChoreographyStep step in choreography_steps) {
            if (!step.choreography_complete) { return false; }
            debug__completed += 1;
        }
        return true;
    }
}