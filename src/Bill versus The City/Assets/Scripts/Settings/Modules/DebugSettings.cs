using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class DebugSettings : AbstractSettingsModule {
    
    private bool _show_fps = true;
    public bool show_fps { 
        get { return _show_fps; }
        set {
            _show_fps = value;
            UpdateSubscribers("show_fps");
        }
    }
    private bool _debug_mode = false;
    public bool debug_mode {
        get { return _debug_mode; }
        set {
            _debug_mode = value;
            UpdateSubscribers("debug_mode");
        }
    }

    private bool _show_damage_numbers = false;
    public bool show_damage_numbers {
        get { return _show_damage_numbers; }
        set {
            _show_damage_numbers = value;
            UpdateSubscribers("show_damage_numbers");
        }
    }

    private bool _player_invincibility = false;
    public bool player_invincibility {
        get { return _player_invincibility; }
        set {
            _player_invincibility = value;
            UpdateSubscribers("player_invincibility");
        }
    }

    private bool _player_invisible = false;
    public bool player_invisible {
        get { return _player_invisible; }
        set {
            _player_invisible = value;
            UpdateSubscribers("player_invisible");
        }
    }

    private bool _allow_debug_actions = true;
    public bool allow_debug_actions { 
        get { return _allow_debug_actions; }
        set {
            _allow_debug_actions = value;
            UpdateSubscribers("allow_debug_actions");
        }
    }


    public override List<string> all_fields { get { return new List<string>(){ "show_fps", "debug_mode" }; }}
    public override DuckDict AsDuckDict() {
        // returns json data for the settings in this module
        DuckDict data = new DuckDict();
        data.SetBool("show_fps", show_fps);
        data.SetBool("debug_mode", debug_mode);
        data.SetBool("show_damage_numbers", show_damage_numbers);
        data.SetBool("player_invincibility", player_invincibility);
        data.SetBool("player_invisible", player_invisible);
        data.SetBool("allow_debug_actions", allow_debug_actions);
        return data;
    }
    public override void LoadFromJson(DuckDict data) {
        // sets the settings module from a JSON string
        // DuckDict data = JsonParser.ReadAsDuckDict(json_str);
        show_fps = UnpackBool(data, "show_fps");
        debug_mode = UnpackBool(data, "debug_mode");
        show_damage_numbers = UnpackBool(data, "show_damage_numbers");
        player_invincibility = UnpackBool(data, "player_invincibility");
        player_invisible = UnpackBool(data, "player_invisible");
        allow_debug_actions = UnpackBool(data, "allow_debug_actions");
        this.AllFieldsUpdates();
    }

    private bool UnpackBool(DuckDict data, string field_name) {
        bool? v = data.GetBool(field_name);
        if (v == null) {
            v = false;
        }
        return (bool) v;
    }
}