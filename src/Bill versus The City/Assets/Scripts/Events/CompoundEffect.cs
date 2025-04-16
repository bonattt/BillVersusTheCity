
using System;
using System.Collections.Generic;

using UnityEngine;

public class CompoundEffect : MonoBehaviour, IGameEventEffect {

    public List<MonoBehaviour> effects;
    private List<IGameEventEffect> _effects;

    void Start() {
        InitializeEffects();
    }

    private void InitializeEffects() {
        // initializes _effects with IGameEventEffects from mono behaviours set in the inspector in `effects` 
        _effects = new List<IGameEventEffect>();
        foreach (MonoBehaviour behaviour in effects) {
            // IGameEventEffect[] callbacks = behaviour.GetComponents<IGameEventEffect>();
            // if (callbacks.Length == 0) {
            //     Debug.LogWarning($"{behaviour.gameObject.name} has no IGameEventEffect components!!");
            // }
            // foreach (IGameEventEffect e in callbacks) {
            //     _effects.Add(e);
            // }
            try {
                _effects.Add((IGameEventEffect) behaviour);
            } catch (InvalidCastException) {
                Debug.LogError($"cannot cast {behaviour} of type {behaviour.GetType()} from {behaviour.gameObject.name} to IGameEventEffect!");
            }
        }
    }


    public void ActivateEffect() {
        foreach(IGameEventEffect e in _effects) {
            e.ActivateEffect();
        }
    }

}