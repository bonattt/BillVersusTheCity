
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class AbstractLevelCondition : MonoBehaviour, ILevelCondition {
    
    public int remaining_enemies_target = 0;
    public bool is_active { get; set; }
    public bool was_triggered { get; set; }

    public List<MonoBehaviour> init_effects;
    private List<IGameEventEffect> _effects = new List<IGameEventEffect>();
    public List<IGameEventEffect> effects { 
        get { return _effects; }
    }

    // private bool start_initialized = false;
    // private List<IGameEventEffect> pre_loaded_effects = new List<IGameEventEffect>();
    public void AddEffect(IGameEventEffect new_effect) => effects.Add(new_effect);

    void Start() {
        List<IGameEventEffect> events_from_init = GameEventEffectUtil.LoadEventsFromMonoBehaviour(init_effects);
        _effects = _effects.Concat(events_from_init).ToList();
        ExtendStart();
    }

    protected virtual void ExtendStart() {
        // do nothing, allow sub-classes to extend start
    }

    public abstract bool ConditionMet();
}