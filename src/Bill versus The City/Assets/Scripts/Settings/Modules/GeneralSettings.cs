using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class GeneralSettings : AbstractSettingsModule {
    
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
            DebugMode.inst.debug_enabled = _debug_mode;
        }
    }


    public override List<string> all_fields { get { return new List<string>(){ "show_fps", "debug_mode" }; }}
    public override string AsJson() {
        // returns json data for the settings in this module
        DuckDict data = new DuckDict();
        data.SetBool("show_fps", show_fps);
        data.SetBool("debug_mode", debug_mode);
        return data.Jsonify();
    }
    public override void LoadFromJson(string json_str) {
        // sets the settings module from a JSON string
        DuckDict data = JsonParser.ReadAsDuckDict(json_str);
        show_fps = UnpackBool(data, "show_fps");
        debug_mode = UnpackBool(data, "debug_mode");
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