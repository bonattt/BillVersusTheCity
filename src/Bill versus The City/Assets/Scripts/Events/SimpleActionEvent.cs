using System;

using UnityEngine;

public class SimpleActionEvent : IGameEventEffect, IInteractionEffect {
    // wraps a C# Action in a IGameEventEffect and IInteractionEffect interfaces for compatability and easy scripting
    private Action ActionEffect;
    public SimpleActionEvent(Action action) {

        this.ActionEffect = action;
    }

    public void Interact(GameObject _) {
        this.ActionEffect();
    }

    public void ActivateEffect() {
        this.ActionEffect();
    }

}