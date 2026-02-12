using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GenericSound : ISingleSFXSound, ISFXSounds {
    public string sound_name  { get; set; }
    public AudioClip clip  { get; set; }

    public float volume { get; set; }

    public SoundCategory default_category { get; set; }

    [Tooltip("If set, this will override the default audio mixer for the sound's SoundCategory.")]
    public AudioMixerGroup _override_mixer_group = null;
    public AudioMixerGroup GetMixerGroup(SoundCategory category) {
        if (_override_mixer_group == null) {
            return AudioMixerManager.inst.GetMixerGroup(category);
        }
        return _override_mixer_group;
    }

    public GenericSound() {
        sound_name = "new sound";
        volume = 1f;
        default_category = SoundCategory.sound_effect;
    }

    public List<ISingleSFXSound> GetSounds() {
        return new List<ISingleSFXSound>(new ISingleSFXSound[]{this});
    }
}