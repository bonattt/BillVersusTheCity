using System;
using System.Collections.Generic;


public interface ILevelCondition {
    public bool is_active { get; set; }
    public bool was_triggered { get; set; }
    public bool ConditionMet();
    public void TriggerEffects() {
        for (int i = 0; i < effects.Count; i++) {
            effects[i].ActivateEffect();
        }
        was_triggered = true;
    }
    public List<IGameEventEffect> effects { get; }
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
