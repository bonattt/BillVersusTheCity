

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour {
    /* Facade for dealing with Unity AudioMixers */

    private const float MIN_VOLUME = 0.0001f;
    private const float MAX_VOLUME = 1f;
    public static AudioMixerManager inst { get; private set; }
    [SerializeField] private AudioMixer _audio_mixer;

    void Awake() {
        inst = this;
    }

    private static string SoundCategoryToMixerName(SoundCategory category) {
        switch (category) {
            case SoundCategory.master:
                return "Master";

            case SoundCategory.sound_effect:
                return "Master/sound_effects";

            case SoundCategory.music:
                return "Master/music";

            case SoundCategory.menu:
                return "Master/ui";

            default:
                Debug.LogWarning($"un-handled SoundCategory '{category}'");
                return $"{category}";
        }
    }

    private static readonly Dictionary<SoundCategory, string> category_to_volume_fields = new Dictionary<SoundCategory, string>() {
        {SoundCategory.master, "master_volume"},
        {SoundCategory.sound_effect, "sound_effect_volume"},
        {SoundCategory.music, "music_volume"},
        {SoundCategory.menu, "menu_volume"},
    };

    private static string CategoryToVolumeField(SoundCategory category) {
        /* takes a SoundCategory, and returns the string name of the Mixer parameter that sets the volume for that sound category 
            Unity's method for doing this is dumb. These strings represent manually configured parameters mapped to settings in the audio mixer
        */
        return category_to_volume_fields[category];
    }

    public AudioMixer GetAudioMixer() {
        return _audio_mixer;
    }

    public static float PercentToDecibels(float slider_value) {
        /* takes a percent volume (from a UI slider) and converts it to a decibel setting for */
        slider_value = Mathf.Clamp(slider_value, MIN_VOLUME, MAX_VOLUME); // prevents values of 0
        float dB = Mathf.Log10(slider_value) * 20f;
        return dB;
    }

    public void SetMixerVolume(SoundCategory category, float volume) {
        _audio_mixer.SetFloat(CategoryToVolumeField(category), PercentToDecibels(volume));
    }

    public AudioMixerGroup GetMixerGroup(SoundCategory category) {
        string mixer_name = SoundCategoryToMixerName(category);
        AudioMixerGroup[] mixers = _audio_mixer.FindMatchingGroups(mixer_name);
        int count = mixers.Count();
        if (count != 1) {
            if (count == 0) {
                Debug.LogError($"no mixers found matching '{category}'");
                return null;
            } else {
                foreach (AudioMixerGroup m in mixers) {
                    if (mixer_name.Equals(m.name)) {
                        return m;
                    }
                }
                string msg = $"'{count}' Too many mixers found matching '{category}' with no exact matches! returning first mixer: {mixers[0]}";
                Debug.LogError(msg);
            }
        }
        return mixers[0];
    }

    public float GetGroupVolume(SoundCategory category) {
        /* gets the volume setting on a specific audio mixer, (not the total playback volume for that AudioMixerGroup) */
        _audio_mixer.GetFloat(CategoryToVolumeField(category), out float mixer_setting);
        return mixer_setting;
    }

    public float GetEffectiveVolume(SoundCategory category) {
        /* gets the effective volume setting on a specific audio mixer, incorperating parent volume levels */
        float mixer_level = GetGroupVolume(category);
        if (category == SoundCategory.master) {
            return mixer_level; // if it's the master volume, don't apply master to the volume.
        }
        float master_level = GetGroupVolume(SoundCategory.master);
        return mixer_level + master_level;
    }
}