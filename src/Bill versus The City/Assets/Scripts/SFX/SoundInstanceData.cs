

using System;
using UnityEngine;

public class SoundInstanceData : MonoBehaviour
{
    /* This class tracks data about a sound fed into an AudioSource, 
       so the origianl sound and it's data can be recovered from the AudioSource's GameObject, if needed
     */

    public ISingleSFXSound sound;
    public ISFXSounds sound_set;
    public SoundCategory category;
    public AudioSource audio_source;

# if UNITY_EDITOR

    public string sound_name;
    public string sound_set_name;
    [Tooltip("Returns the volume on the audioSource (in 0-1 percent)")]
    public float debug__source_volume = -1;
    [Tooltip("Returns the effective volume based on the mixer and it's parents. (in dB's)")]
    public float debug__effective_mixer_volume = -1;

    [Tooltip("returns the volume setting on JUST the mixer specified by the SoundCategory. (is dB's)")]
    public float debug__exact_mixer_volume = -1;

    void Update() {
        sound_name = sound != null ? sound.sound_name : "NULL";

        if (sound_set == null) {
            sound_set_name = "no sound set";
        } else {
            try {
                sound_set_name = $"{((ScriptableObject) sound_set).name}";
            } catch (InvalidCastException) {
                sound_set_name = "InvalidCastException";
            }
        }

        if (audio_source != null) {
            debug__source_volume = audio_source.volume;
        }
        debug__effective_mixer_volume = AudioMixerManager.inst.GetEffectiveVolume(category);
        debug__exact_mixer_volume = AudioMixerManager.inst.GetGroupVolume(category);
    }
# endif
}