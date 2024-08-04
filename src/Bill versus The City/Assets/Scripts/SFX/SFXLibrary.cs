
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SFXLibrary
{

    public static ISoundSet LoadSoundSet(string name) {
        return (ISoundSet) Resources.Load<ScriptableObject>(name);
    }

    public static ISound LoadSound(string path) {
        return (ISound) Resources.Load<ScriptableObject>(path);

        // AudioClip clip = Resources.Load<AudioClip>(path);
        
        // AdjustedSound new_sound = new AdjustedSound();
        // new_sound.name = $"{path}";
        // new_sound.clip = clip;
        // new_sound.volume = 1;
        // new_sound.default_category = SoundCategory.sound_effect;

        // return new_sound;
    }
}
