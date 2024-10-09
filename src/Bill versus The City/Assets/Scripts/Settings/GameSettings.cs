using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameSettings {

    public static GameSettings inst { get; private set; }

    // public const string AUDIO_SETTINGS = "audio";
    // public const string GRAPHICS_SETTINGS = "graphics";
    // public const string GAMEPLAY_SETTINGS = "gameplay";
    // public const string DIFFICULTY_SETTINGS = "difficulty";

    private GeneralSettings _general_settings = new GeneralSettings();
    public GeneralSettings general_settings {
        get { return _general_settings; }
        set {
            ReplaceModule(_general_settings, value);
            _general_settings = value;
        }
    }
    
    private AudioSettings _audio_settings = new AudioSettings();
    public AudioSettings audio_settings { 
        get { return _audio_settings; }
        set { 
            ReplaceModule(_audio_settings, value);
            _audio_settings = value; 
        }
    }
    
    private GamePlaySettings _game_play_settings = new GamePlaySettings();
    public GamePlaySettings game_play_settings { 
        get { return _game_play_settings; }
        set { 
            ReplaceModule(_game_play_settings, value);
            _game_play_settings = value; 
        }
    }

    private DifficultySettings _difficulty_settings = new DifficultySettings();
    public DifficultySettings difficulty_settings { 
        get { return _difficulty_settings; }
        set { 
            ReplaceModule(_difficulty_settings, value);
            _difficulty_settings = value; 
        }
    }

    public GameSettings() {
        if (inst != null) { Debug.LogWarning("overwriting existing settings!"); }
        inst = this;
    }


    private void ReplaceModule(ISettingsModule old_module, ISettingsModule new_module) {
        // add `old_module`s subscribers to `new_module` so the old module can be replaced seemlessly
        foreach (ISettingsObserver sub in old_module.GetSubscribers()) {
            new_module.Subscribe(sub);
        }
    }
    
    public string AsJson() {
        // returns json data for the settings in this module
        DuckDict data = new DuckDict();
        
        data.SetObject("general", JsonParser.ReadAsDuckDict(general_settings.AsJson()));
        data.SetObject("difficulty", JsonParser.ReadAsDuckDict(difficulty_settings.AsJson()));
        data.SetObject("gameplay", JsonParser.ReadAsDuckDict(game_play_settings.AsJson()));
        data.SetObject("audio", JsonParser.ReadAsDuckDict(audio_settings.AsJson()));
        data.SetObject("graphics", new DuckDict());

        return data.Jsonify();
    }
    public void LoadFromJson(string json_str) {
        // sets the settings module from a JSON string
        DuckDict data = JsonParser.ReadAsDuckDict(json_str);
        
        general_settings.LoadFromJson(data.GetObject("general").Jsonify());
        difficulty_settings.LoadFromJson(data.GetObject("difficulty").Jsonify());
        game_play_settings.LoadFromJson(data.GetObject("gameplay").Jsonify());
        audio_settings.LoadFromJson(data.GetObject("audio").Jsonify());
    }

    // private Dictionary<string, ISettingsModule> settings_modules = new Dictionary<string, ISettingsModule>();

    // public ISettingsModule GetModule(string key) => settings_modules[key];
    
    // public void SetModule(string key, ISettingsModule module) => settings_modules[key] = module;
    


}
