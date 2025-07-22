using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class GamePlaySettings : AbstractSettingsModule {
    public const float MIN_MOUSE_SENSITIVITY = 0.01f;
    public const string MOUSE_SENSITIVITY = "mouse_sensitivity";
    public float mouse_sensitivity {
        get => GetFloat(MOUSE_SENSITIVITY);
        set {
            if (value <= MIN_MOUSE_SENSITIVITY) {
                Debug.LogWarning($"mouse_sensitivity less than minimum {value} --> {MIN_MOUSE_SENSITIVITY}");
                SetFloat(MOUSE_SENSITIVITY, MIN_MOUSE_SENSITIVITY);
            } else {
                SetFloat(MOUSE_SENSITIVITY, value);
            }
            UpdateSubscribers(MOUSE_SENSITIVITY);
        }
    }

    private static readonly List<string> FLOAT_FIELDS = new List<string>(){ MOUSE_SENSITIVITY };
    public override List<string> float_field_names { get => FLOAT_FIELDS; }

    // public override DuckDict AsDuckDict() {
    //     // returns json data for the settings in this module
    //     DuckDict data = new DuckDict();
    //     data.SetFloat(MOUSE_SENSITIVITY, mouse_sensitivity);
    //     return data;
    // }
    // public override void LoadFromJson(DuckDict data) {
    //     // sets the settings module from a JSON string
    //     // DuckDict data = JsonParser.ReadAsDuckDict(json_str);

    //     float? ms = data.GetFloat(MOUSE_SENSITIVITY);
    //     if (ms == null) {
    //         mouse_sensitivity = 1f;
    //     } else {
    //         mouse_sensitivity = (float) ms;
    //     }

    //     this.AllFieldsUpdated();
    // }
    
}