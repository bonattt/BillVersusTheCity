
using System;
using System.Collections.Generic;

using UnityEngine;

public class CompoundEffect : MonoBehaviour, IGameEventEffect {

    public List<GameObject> effects;
    private List<IGameEventEffect> _effects;

    void Start() {
        InitializeEffects();
    }

    private void InitializeEffects() {
        // initializes _effects with IGameEventEffects from components set in the inspector in `effects` 
        _effects = new List<IGameEventEffect>();
        foreach (GameObject obj in effects) {
            IGameEventEffect[] callbacks = obj.GetComponents<IGameEventEffect>();
            if (callbacks.Length == 0) {
                Debug.LogWarning($"{obj.name} has no IGameEventEffect components!!");
            }
            foreach (IGameEventEffect e in callbacks) {
                _effects.Add(e);
            }
            // try {
            //     _effects.Add((IGameEventEffect) behaviour);
            // } catch (InvalidCastException) {
            //     Debug.LogError($"cannot cast {behaviour} of type {behaviour.GetType()} from {behaviour.gameObject.name} to IGameEventEffect!");
            // }
        }
    }


    public void ActivateEffect() {
        foreach(IGameEventEffect e in _effects) {
            e.ActivateEffect();
        }
    }

}