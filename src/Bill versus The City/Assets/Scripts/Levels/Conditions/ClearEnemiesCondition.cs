
using System;
using System.Collections.Generic;

using UnityEngine;


public class ClearEnemiesCondition : MonoBehaviour, ILevelCondition {
    
    public int remaining_enemies_target = 0;
    public bool is_active { get; set; }
    public bool was_triggered { get; set; }

    public List<MonoBehaviour> init_effects;
    public List<IGameEventEffect> effects { get; private set; }

    void Start() {
        effects = GameEventEffectUtil.LoadEventsFromMonoBehaviour(init_effects);
    }

    public bool ConditionMet() {
        return EnemiesManager.inst.remaining_enemies <= remaining_enemies_target;
    }
}