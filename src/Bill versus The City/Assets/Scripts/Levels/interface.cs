using System;
using System.Collections.Generic;

using UnityEngine;

public interface ILevelCondition {
    public bool is_active { get; set; }
    public bool was_triggered { get; set; }
    public bool condition_effects_completed { get; } // flag, if true, effects from "TriggerEffects" have finished evaluation, and the next conditions may be evaluated
    public bool ConditionMet();
    public void TriggerEffects() {
        if (effects == null) { 
            Debug.LogWarning($"effects null in ILevelCondition: {this}");
            return; 
        }
        for (int i = 0; i < effects.Count; i++) {
            effects[i].ActivateEffect();
        }
        was_triggered = true;
    }
    public List<IGameEventEffect> effects { get; }
    public void AddEffect(IGameEventEffect new_effect);
}


public enum LevelMusicStart {
    load_scene,
    start_level
}

public enum LevelMusicStop {
    complete_objectives,
    exit_scene
}

public interface ILevelMusic {
    public LevelMusicStart music_start { get; }
    public LevelMusicStop music_stop { get; }
    public void StartLevelMusic();
    public void StopLevelMusic();
}
