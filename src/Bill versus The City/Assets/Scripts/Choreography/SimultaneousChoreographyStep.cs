

using System.Collections.Generic;
using UnityEngine;

public class SimultaneousChoreographyStep : AbstractChoreographyStep {
    
    public List<AbstractChoreographyStep> choreography_steps;

    public int debug__completed = -1;

    public override void Activate(IChoreography choreography_) {
        base.Activate(choreography_);
        foreach (AbstractChoreographyStep step in choreography_steps) {
            step.Activate(choreography_);
        }
    }

    void Update() {
        if (!active || complete) { return; }

        if (AllStepsComplete()) {
            Complete();
        }
    }

    public bool AllStepsComplete() {
        debug__completed = 0;
        foreach (AbstractChoreographyStep step in choreography_steps) {
            if (!step.complete) { return false; }
            debug__completed += 1;
        }
        return true;
    }
}