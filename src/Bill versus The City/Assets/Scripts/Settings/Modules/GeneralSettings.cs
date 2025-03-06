using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GeneralSettings : AbstractSettingsModule {
    
    private bool _placeholder = false;
    public bool skip_all_tutorials {
        get { return _placeholder; }
        set {
            _placeholder = value;
            DebugMode.inst.debug_enabled = _placeholder;
        }
    }

    public override List<string> all_fields { get { return new List<string>(){ "skip_all_tutorials" }; }}
    public HashSet<string> skipped_tutorials = new HashSet<string>();

    public override string AsJson() {
        // returns json data for the settings in this module
        DuckDict data = new DuckDict();
        data.SetBool("skip_all_tutorials", skip_all_tutorials);
        data.SetStringList("skipped_tutorials", skipped_tutorials.ToList());
        return data.Jsonify();
    }

    public override void LoadFromJson(string json_str) {
        // sets the settings module from a JSON string
        DuckDict data = JsonParser.ReadAsDuckDict(json_str);
        skip_all_tutorials = UnpackBool(data, "skip_all_tutorials");
        skipped_tutorials = UnpackHashSet(data, "skipped_tutorials");
        this.AllFieldsUpdates();
    }

    private bool UnpackBool(DuckDict data, string field_name) {
        bool? v = data.GetBool(field_name);
        if (v == null) {
            v = false;
        }
        return (bool) v;
    }

    private HashSet<string> UnpackHashSet(DuckDict data, string field_name) {
        List<string> data_out = data.GetStringList(field_name);
        if (data_out == null) { return new HashSet<string>(); }

        return new HashSet<string>(data_out);
    }
}