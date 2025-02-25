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


    public override List<string> all_fields { get { return new List<string>(){ "show_fps", "debug_mode" }; }}
    public override string AsJson() {
        // returns json data for the settings in this module
        DuckDict data = new DuckDict();
        data.SetBool("show_fps", show_fps);
        data.SetBool("debug_mode", debug_mode);
        data.SetBool("show_damage_numbers", show_damage_numbers);
        return data.Jsonify();
    }
    public override void LoadFromJson(string json_str) {
        // sets the settings module from a JSON string
        DuckDict data = JsonParser.ReadAsDuckDict(json_str);
        show_fps = UnpackBool(data, "show_fps");
        debug_mode = UnpackBool(data, "debug_mode");
        show_damage_numbers = UnpackBool(data, "show_damage_numbers");
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