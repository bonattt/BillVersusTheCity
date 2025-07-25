using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class AudioSettings : AbstractSettingsModule {
    
    private const string MASTER_VOLUME = "master_volume";
    public float master_volume {
        get { return GetFloat(MASTER_VOLUME); }
        set {
            SetFloat(MASTER_VOLUME, value);
        }
    }

    public float GetVolume(SoundCategory category) {
        return GetVolumeSetting(category) * master_volume;
    }

    public float GetVolumeSetting(SoundCategory category) {
        string field_name = CategoryToString(category);
        if (!float_fields.ContainsKey(field_name)) {
            SetFloat(field_name, 1f);  // initialize default value to volume settings
            Debug.LogWarning("this should probably be implemented with dynamic default values");
        }
        return GetFloat(field_name);
    }

    public void SetVolumeSetting(SoundCategory category, float volume) {
        SetFloat(CategoryToString(category), volume);
    }


    private List<string> _volume_fields = null;
    public override List<string> float_field_names {
        get {
            if (_volume_fields == null) {
                _volume_fields = new List<string>(){MASTER_VOLUME};
                foreach (SoundCategory category in Enum.GetValues(typeof(SoundCategory))) {
                    _volume_fields.Add(CategoryToString(category));
                }
            }
            return _volume_fields;
        }
    }

    private const float MAX_VOLUME = 1f;
    private const float MIN_VOLUME = 0f;
    protected override void InitializeMinMaxAndDefaults() {
        float_fields_max = new Dictionary<string, float>();
        float_fields_min = new Dictionary<string, float>();
        foreach (string f in float_field_names) {
            float_fields_default[f] = MAX_VOLUME;
            float_fields_max[f] = MAX_VOLUME;
            float_fields_min[f] = MIN_VOLUME;
        }
    }
    
    // public override void RestoreToDefaults() {
    //     base.RestoreToDefaults();
    // }

    private static SoundCategory CategoryFromString(string category) {
        switch (category) {
            case "sound_effect":
                return SoundCategory.sound_effect;
            case "music":
                return SoundCategory.music;
            case "menu":
                return SoundCategory.menu;

            default:
                Debug.LogError($"unknown sound category string '{category}'!");
                return SoundCategory.sound_effect;
        }
    }

    private static string CategoryToString(SoundCategory category) {
        return $"{category}";
    }
}