using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class AudioSettings : ISettingsModule {
    
    private float _master_volume = 1f;
    public float master_volume {
        get { return _master_volume; }
        set { 
            _master_volume = Mathf.Clamp(value, 0, 1); 
            UpdateSubscribers("master_volume");
        }
    }

    private Dictionary<SoundCategory, float> volume_settings = new Dictionary<SoundCategory, float>();

    private List<ISettingsObserver> subscribers = new List<ISettingsObserver>();

    public void Subscribe(ISettingsObserver sub) => subscribers.Add(sub);
    public void Unsubscribe(ISettingsObserver sub) => subscribers.Remove(sub);

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

    public void UpdateSubscribers(string field) {
        foreach(ISettingsObserver sub in subscribers) {
            sub.SettingsUpdated(this, field);
        }
    }
}