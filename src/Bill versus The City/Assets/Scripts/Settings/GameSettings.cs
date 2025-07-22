using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameSettings {

    private static GameSettings _inst = null;
    public static GameSettings inst { 
        get { 
            if (_inst == null) {
                _inst = new GameSettings();
            }
            return _inst; 
        }
    }

    public void SetToNewGameDefault()
    {
        // sets settings to new game defaults. Some settings modules are effected by this, others or not, 
        //   but that responsibility is handed off to the modules.
        general_settings.SetToNewGameDefault();
        difficulty_settings.SetToNewGameDefault();
        game_play_settings.SetToNewGameDefault();
        audio_settings.SetToNewGameDefault();
        debug_settings.SetToNewGameDefault();
    }

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
    
    private DebugSettings _debug_settings = new DebugSettings();
    public DebugSettings debug_settings {
        get { return _debug_settings; }
        set {
            ReplaceModule(_debug_settings, value);
            _debug_settings = value;
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
    
    // public Dictionary<string, AbstractSettingsModule> all_modules
    // {
    //     get
    //     {
    //         return new Dictionary<string, AbstractSettingsModule>()
    //         {
    //             {"general", general_settings},
    //             {"gameplay", game_play_settings},
    //             {"difficulty", difficulty_settings},
    //             {"debug", debug_settings},
    //             {"audio", audio_settings},
    //             // {"graphics", "TODO"}
    //         };
    //     }        
    // }

    private GameSettings()
    {
        // if (inst != null) { Debug.LogWarning("overwriting existing settings!"); }
        // inst = this;
    }


    private void ReplaceModule(ISettingsModule old_module, ISettingsModule new_module) {
        // add `old_module`s subscribers to `new_module` so the old module can be replaced seemlessly
        foreach (ISettingsObserver sub in old_module.SubscriberIterator()) {
            new_module.Subscribe(sub);
        }
    }

    public DuckDict AsDuckDict()
    {
        // returns json data for the settings in this module
        DuckDict data = new DuckDict();

        data.SetObject("general", JsonParser.ReadAsDuckDict(general_settings.AsJson()));
        data.SetObject("difficulty", JsonParser.ReadAsDuckDict(difficulty_settings.AsJson()));
        data.SetObject("gameplay", JsonParser.ReadAsDuckDict(game_play_settings.AsJson()));
        data.SetObject("audio", JsonParser.ReadAsDuckDict(audio_settings.AsJson()));
        data.SetObject("graphics", new DuckDict());
        data.SetObject("debug", JsonParser.ReadAsDuckDict(debug_settings.AsJson()));
        return data;
    }
    
    public string AsJson()
    {
        // returns json data for the settings in this module
        DuckDict data = AsDuckDict();
        return data.Jsonify();
    }
    public void LoadFromJson(string json_str) {
        // sets the settings module from a JSON string
        DuckDict data = JsonParser.ReadAsDuckDict(json_str);
        LoadFromJson(data);
    }

    public void LoadFromJson(DuckDict data) {
        // Debug.LogWarning($"GameSettings.LoadFromJson, data: {data.Jsonify()}"); // TODO --- remove debug
        // Debug.LogWarning($"GameSettings.LoadFromJson, data: {data.GetObject("general")}"); // TODO --- remove debug
        general_settings.LoadFromJson(data.GetObject("general"));
        difficulty_settings.LoadFromJson(data.GetObject("difficulty"));
        game_play_settings.LoadFromJson(data.GetObject("gameplay"));
        audio_settings.LoadFromJson(data.GetObject("audio"));
        debug_settings.LoadFromJson(data.GetObject("debug"));
        // videos_settings.LoadFromJson(data); // TODO --- implement graphics settings
    }
}
