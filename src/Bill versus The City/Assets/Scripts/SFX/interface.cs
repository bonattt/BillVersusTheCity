using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ISound {
    public string sound_name { get; set; }
    public AudioClip clip { get; set; }
    public float volume { get; set; }
    public SoundCategory default_category { get; set; }
}


public interface ISoundSet {
    public List<ISound> GetSounds();

    public ISound GetRandomSound() {
        List<ISound> sounds = GetSounds();
        int i = Random.Range(0, sounds.Count);
        return sounds[i];
    }
}