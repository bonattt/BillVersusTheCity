using System;
using System.Collections.Generic;


public interface ILevelCondition {
    public bool is_active { get; set; }
    public bool was_triggered { get; set; }
    public bool ConditionMet();
    public void TriggerEffects() {
        for (int i = 0; i < effects.Count; i++) {
            effects[i].ActivateEffect();
        }
        was_triggered = true;
    }
    public List<IGameEventEffect> effects { get; }
}
