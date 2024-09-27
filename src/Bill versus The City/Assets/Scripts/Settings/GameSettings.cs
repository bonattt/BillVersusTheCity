using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameSettings {

    public static GameSettings inst { get; private set; }

    // public const string AUDIO_SETTINGS = "audio";
    // public const string GRAPHICS_SETTINGS = "graphics";
    // public const string GAMEPLAY_SETTINGS = "gameplay";
    // public const string DIFFICULTY_SETTINGS = "difficulty";

    public AudioSettings audio_settings = new AudioSettings();
    public GamePlaySettings game_play_settings = new GamePlaySettings();

    public GameSettings() {
        if (inst != null) { Debug.LogWarning("overwriting existing settings!"); }
        inst = this;
    }

    // private Dictionary<string, ISettingsModule> settings_modules = new Dictionary<string, ISettingsModule>();

    // public ISettingsModule GetModule(string key) => settings_modules[key];
    
    // public void SetModule(string key, ISettingsModule module) => settings_modules[key] = module;
    


}
