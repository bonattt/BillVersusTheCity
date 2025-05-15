using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class AudioSettings : AbstractSettingsModule {
    
    private float _master_volume = 1f;
    public float master_volume {
        get { return _master_volume; }
        set { 
            float percent = Mathf.Clamp(value, 0, 1); 
            if (percent != value) {
                Debug.LogWarning($"clampped invalid master_volume {value} --> {percent}");
            }
            _master_volume = percent;
            UpdateSubscribers("master_volume");
        }
    }

    private Dictionary<SoundCategory, float> volume_settings = new Dictionary<SoundCategory, float>();

    public float GetVolume(SoundCategory category) {
        return GetVolumeSetting(category) * master_volume;
    }

    public float GetVolumeSetting(SoundCategory category) {
        if (! volume_settings.ContainsKey(category)) {
            volume_settings[category] = 1f;  // initialize default value to volume settings
        }
        return volume_settings[category];
    }

    public void SetVolumeSetting(SoundCategory category, float volume) {
        float _volume = Mathf.Clamp(volume, 0, 1);
        if (_volume != volume) {
            Debug.LogWarning($"snapped invalid volume setting {volume} -> {_volume} for {category}");
        }
        volume_settings[category] = _volume;
        UpdateSubscribers($"{category}");
    }
    
    public override string AsJson() {
        // returns json data for the settings in this module
        DuckDict data = new DuckDict();
        data.SetFloat("master_volume", master_volume);
        foreach (SoundCategory category in volume_settings.Keys) {
            data.SetFloat(CategoryToString(category), volume_settings[category]);
        }

        return data.Jsonify();
    }

    private List<string> _all_fields = null;
    public override List<string> all_fields {
        get {
            if (_all_fields == null) {
                _all_fields = new List<string>(){"master_volume"};
                foreach (SoundCategory category in Enum.GetValues(typeof(SoundCategory))) {
                    _all_fields.Add(CategoryToString(category));
                }
            }
            return _all_fields;
        }
    }
    
    public override void LoadFromJson(DuckDict data) {
        // sets the settings module from a JSON string
        // DuckDict data = JsonParser.ReadAsDuckDict(json_str);
        master_volume = (float) data.GetFloat("master_volume");

        foreach (SoundCategory category in Enum.GetValues(typeof(SoundCategory))) {
            float val;
            string key = CategoryToString(category);
            if (data.ContainsKey(key)) {
                if (data.GetFloat(key) == null) {
                    val = 1f;
                }
                else {
                    val = (float) data.GetFloat(key);
                }
            }
            else {
                val = 1f;
            }
            volume_settings[CategoryFromString(key)] = val;
            
            this.AllFieldsUpdates();
        }
    }

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