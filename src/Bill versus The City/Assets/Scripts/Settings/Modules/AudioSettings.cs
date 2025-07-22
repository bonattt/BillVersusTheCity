using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class AudioSettings : AbstractSettingsModule {
    
    private const string MASTER_VOLUME = "master_volume";
    public float master_volume {
        get { return GetFloat(MASTER_VOLUME); }
        set {
            // float percent = Mathf.Clamp(value, 0, 1);
            // if (percent != value) {
            //     Debug.LogWarning($"clampped invalid master_volume {value} --> {percent}");
            // }
            // _master_volume = percent;
            // UpdateSubscribers("master_volume");
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
        // float _volume = Mathf.Clamp(volume, 0, 1);
        // if (_volume != volume) {
        //     Debug.LogWarning($"snapped invalid volume setting {volume} -> {_volume} for {category}");
        // }
        // volume_settings[category] = _volume;
        // UpdateSubscribers($"{category}");
        SetFloat(CategoryToString(category), volume);
    }

    // public override DuckDict AsDuckDict()
    // {
    //     // returns json data for the settings in this module
    //     DuckDict data = new DuckDict();
    //     data.SetFloat("master_volume", master_volume);
    //     foreach (SoundCategory category in volume_settings.Keys)
    //     {
    //         data.SetFloat(CategoryToString(category), volume_settings[category]);
    //     }
    //     return data;
    // }

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

    // public override void LoadFromJson(DuckDict data, bool update_subscribers = true) {
    //     // sets the settings module from a JSON string
    //     // DuckDict data = JsonParser.ReadAsDuckDict(json_str);
    //     master_volume = (float)data.GetFloat("master_volume");

    //     foreach (SoundCategory category in Enum.GetValues(typeof(SoundCategory))) {
    //         float val;
    //         string key = CategoryToString(category);
    //         if (data.ContainsKey(key)) {
    //             if (data.GetFloat(key) == null) {
    //                 val = 1f;
    //             } else {
    //                 val = (float)data.GetFloat(key);
    //             }
    //         } else {
    //             val = 1f;
    //         }
    //         volume_settings[CategoryFromString(key)] = val;

    //     }
    //     if (update_subscribers) {
    //         this.AllFieldsUpdated();
    //     }
    // }

    private static SoundCategory CategoryFromString(string category) {
        switch(category) {
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