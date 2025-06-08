using System;

using UnityEngine;

public class SimpleActionEvent : IGameEventEffect, IInteractionEffect {
    // wraps a C# Action in a IGameEventEffect and IInteractionEffect interfaces for compatability and easy scripting
    private Action ActionEffect;
    public bool effect_completed { get; protected set; }
    public SimpleActionEvent(Action action) {
        this.ActionEffect = action;
        effect_completed = false;
    }

    public void Interact(GameObject _) {
        this.ActionEffect();
    }

    public void ActivateEffect() {
        this.ActionEffect();
        effect_completed = true;
    }

}