using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCondition : AbstractLevelCondition, IGameEventEffect, IInteractionEffect {
    // LevelCondition which is completed once it has been triggered as an IGameEvent 

    private bool event_triggered = false;
    public bool destroy_on_trigger = true;

    ////////// IGameEventEffect ////////// 
    public bool effect_completed { get => event_triggered; }
    public void ActivateEffect() {
        _Effect();
    }
    ////////// IInteractionEffect ////////// 
    public void Interact(GameObject actor) {
        _Effect();
    }
    ////////// AbstractLevelCondition ////////// 

    public override bool ConditionMet() => event_triggered;
    
    
    ////////// this ////////// 
    private void _Effect() {
        event_triggered = true;
        TriggerEffects();
        if (destroy_on_trigger) {
            Destroy(this);
        }
    }

}
