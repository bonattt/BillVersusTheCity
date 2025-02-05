
using System;
using System.Collections.Generic;

using UnityEngine;


public abstract class AbstractLevelCondition : MonoBehaviour, ILevelCondition {
    
    public int remaining_enemies_target = 0;
    public bool is_active { get; set; }
    public bool was_triggered { get; set; }

    public List<MonoBehaviour> init_effects;
    public List<IGameEventEffect> effects { get; private set; }

    private bool start_initialized = false;
    private List<IGameEventEffect> pre_loaded_effects = new List<IGameEventEffect>();
    public void AddEffect(IGameEventEffect new_effect) {
        if (!start_initialized) {
            pre_loaded_effects.Add(new_effect);
        }
        else {
            effects.Add(new_effect);
        }
    }

    void Start() {
        effects = GameEventEffectUtil.LoadEventsFromMonoBehaviour(init_effects);
        foreach (IGameEventEffect e in pre_loaded_effects) {
            Debug.LogWarning("preload effects called!"); // TODO --- remove debug
            effects.Add(e);
        } 
        ExtendStart();
        start_initialized = true;
    }

    protected virtual void ExtendStart() {
        // do nothing, allow sub-classes to extend start
    }

    public abstract bool ConditionMet();
}