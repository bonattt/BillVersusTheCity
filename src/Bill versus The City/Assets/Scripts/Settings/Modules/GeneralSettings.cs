using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GeneralSettings : AbstractSettingsModule {
    
    private const string SKIP_ALL_TUTORIALS = "skip_all_tutorials";
    public bool skip_all_tutorials {
        get => GetBool(SKIP_ALL_TUTORIALS);
        set {
            SetBool(SKIP_ALL_TUTORIALS, value);
        }
    }

    public override List<string> bool_field_names { get { return new List<string>() { "skip_all_tutorials" }; } }

    private static readonly List<string> OTHER_FIELDS = new List<string>() {
        "skipped_tutorials",
    };
    public override List<string> other_field_names { get => OTHER_FIELDS; }
    public HashSet<string> skipped_tutorials = new HashSet<string>();

    protected override void InitializeMinMaxAndDefaults() {
        bool_fields_default[SKIP_ALL_TUTORIALS] = false;
    }

    public override DuckDict AsDuckDict() {
        // returns json data for the settings in this module
        DuckDict data = base.AsDuckDict();
        data.SetStringList("skipped_tutorials", skipped_tutorials.ToList());
        return data;
    }

    public override void LoadFromJson(DuckDict data, bool update_subscribers = true) {
        // sets the settings module from a JSON string
        skipped_tutorials = UnpackHashSet(data, "skipped_tutorials");
        base.LoadFromJson(data, update_subscribers);
    }

    public override void RestoreToDefaults() {
        skip_all_tutorials = false;
        skipped_tutorials = new HashSet<string>();
        base.RestoreToDefaults();
    }

    private HashSet<string> UnpackHashSet(DuckDict data, string field_name) {
        List<string> data_out = data.GetStringList(field_name);
        if (data_out == null) { return new HashSet<string>(); }

        return new HashSet<string>(data_out);
    }
}