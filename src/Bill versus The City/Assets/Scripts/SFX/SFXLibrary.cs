
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SFXLibrary
{

    public static AudioClip LoadAudioClip(string path) {
        return Resources.Load<AudioClip>(path);
    }


    public static ISoundSet LoadSound(string path) {
        ISoundSet sound = (ISoundSet) Resources.Load<ScriptableObject>(path);
        if (sound != null) {
            return sound;
        }
        AudioClip clip = LoadAudioClip(path);
        if (clip == null) { return null; }
        GenericSound new_sound = new GenericSound();
        new_sound.sound_name = path;
        new_sound.volume = 1f;
        new_sound.clip = clip;
        new_sound.default_category = SoundCategory.sound_effect;

        if (new_sound == null) {
            Debug.LogWarning($"Failed to load sound {path}");
        }
        return new_sound;

        // AudioClip clip = Resources.Load<AudioClip>(path);
        
        // AdjustedSound new_sound = new AdjustedSound();
        // new_sound.name = $"{path}";
        // new_sound.clip = clip;
        // new_sound.volume = 1;
        // new_sound.default_category = SoundCategory.sound_effect;

        // return new_sound;
    }
}
