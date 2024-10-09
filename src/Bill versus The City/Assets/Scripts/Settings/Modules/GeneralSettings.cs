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

    public override List<string> all_fields { get { return new List<string>(){ "show_fps"}; }}
    public override string AsJson() {
        // returns json data for the settings in this module
        DuckDict data = new DuckDict();
        data.SetBool("show_fps", show_fps);
        return data.Jsonify();
    }
    public override void LoadFromJson(string json_str) {
        // sets the settings module from a JSON string
        DuckDict data = JsonParser.ReadAsDuckDict(json_str);
        bool? v = data.GetBool("show_fps");
        if (v == null) {
            v = false;
        }
        show_fps = (bool) v;
        this.AllFieldsUpdates();
    }
}