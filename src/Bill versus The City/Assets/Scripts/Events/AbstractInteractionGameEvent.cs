
using UnityEngine;

public abstract class AbstractInteractionGameEvent : MonoBehaviour, IGameEventEffect, IInteractionEffect {
    // This class implements a simple abstract class which reduces boilerplate when implementing a class which 
    // implements both the IGameEventEffect and IIteractionEffect interfaces.
    // most IGameEventEffects can be used as interactions. Some interactions will be unable to implemnent IGameEventEffect


    public bool effect_completed { get; protected set; }
    protected virtual void Start() {
        effect_completed = false;
    }

    public virtual void Interact(GameObject actor) {
        Effect();
    }

    public virtual void ActivateEffect() {
        Effect();
        effect_completed = true;
    }

    protected abstract void Effect();
}