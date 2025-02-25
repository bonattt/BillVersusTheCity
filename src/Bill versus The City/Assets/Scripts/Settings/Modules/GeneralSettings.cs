using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class GeneralSettings : AbstractSettingsModule {
    
    private bool _placeholder = false;
    public bool placeholder {
        get { return _placeholder; }
        set {
            _placeholder = value;
            DebugMode.inst.debug_enabled = _placeholder;
        }
    }


    public override List<string> all_fields { get { return new List<string>(){ "placeholder" }; }}
    public override string AsJson() {
        // returns json data for the settings in this module
        DuckDict data = new DuckDict();
        data.SetBool("placeholder", placeholder);
        return data.Jsonify();
    }
    public override void LoadFromJson(string json_str) {
        // sets the settings module from a JSON string
        DuckDict data = JsonParser.ReadAsDuckDict(json_str);
        placeholder = UnpackBool(data, "placeholder");
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