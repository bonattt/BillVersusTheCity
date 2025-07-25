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

    private static readonly List<string> FLOAT_FIELDS = new List<string>() { MOUSE_SENSITIVITY };
    public override List<string> float_field_names { get => FLOAT_FIELDS; }


    protected override void InitializeMinMaxAndDefaults() {
        float_fields_default[MOUSE_SENSITIVITY] = 1f;
        float_fields_min[MOUSE_SENSITIVITY] = 0.1f;
        float_fields_max[MOUSE_SENSITIVITY] = 5f;
    }
    
    // public override void RestoreToDefaults() {
    //     base.RestoreToDefaults();
    // }

}