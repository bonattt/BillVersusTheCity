using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SetInteractionActive : MonoBehaviour, IGameEventEffect, IInteractionEffect {
    // LevelCondition which is completed once it has been triggered as an IGameEvent 

    public Interaction target;
    public bool set_enabled_to = true;
    private bool event_triggered = false;

    ////////// IGameEventEffect ////////// 
    public bool effect_completed { get => event_triggered; }
    public void ActivateEffect() {
        _Effect();
    }
    ////////// IInteractionEffect ////////// 
    public void Interact(GameObject actor) {
        _Effect();
    }

    private void _Effect() {
        event_triggered = true;
        target.enabled = set_enabled_to;
    }

}
