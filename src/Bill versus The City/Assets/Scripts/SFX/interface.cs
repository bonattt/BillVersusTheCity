using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ISingleSFXSound {
    public string sound_name { get; set; }
    public AudioClip clip { get; set; }
    public float volume { get; set; }
    public SoundCategory default_category { get; set; }
}


public interface ISFXSounds {
    public List<ISingleSFXSound> GetSounds();

    public ISingleSFXSound GetRandomSound() {
        List<ISingleSFXSound> sounds = GetSounds();
        if (sounds.Count <= 0) {
            return null;
        }
        int i = Random.Range(0, sounds.Count);
        return sounds[i];
    }
}