
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class AbstractLevelCondition : MonoBehaviour, ILevelCondition {
    
    public int remaining_enemies_target = 0;
    public bool is_active { get; set; }
    public bool was_triggered { get; set; }

    public virtual bool condition_effects_completed {
        get {
            if (!was_triggered) return false;
            for (int i = 0; i < effects.Count; i++) {
                if (!effects[i].effect_completed) return false;
            }
            return true;
        } 
    }

    public List<MonoBehaviour> init_effects;
    private List<IGameEventEffect> _effects = new List<IGameEventEffect>();
    public List<IGameEventEffect> effects { 
        get { return _effects; }
    }

    // private bool start_initialized = false;
    // private List<IGameEventEffect> pre_loaded_effects = new List<IGameEventEffect>();
    public void AddEffect(IGameEventEffect new_effect) => effects.Add(new_effect);

    protected virtual void Start() {
        List<IGameEventEffect> events_from_init = GameEventEffectUtil.LoadEventsFromMonoBehaviour(init_effects);
        _effects = _effects.Concat(events_from_init).ToList();
    }

    public void TriggerEffects() {
        if (effects == null) {
            Debug.LogWarning($"effects null in ILevelCondition {this.GetType()}: {this}");
            return;
        }
        for (int i = 0; i < effects.Count; i++) {
            effects[i].ActivateEffect();
        }
        was_triggered = true;
    }

    public abstract bool ConditionMet();
}