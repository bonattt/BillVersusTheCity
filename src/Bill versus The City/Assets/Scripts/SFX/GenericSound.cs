using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSound : ISound, ISoundSet {
    public string sound_name  { get; set; }
    public AudioClip clip  { get; set; }

    public float volume { get; set; }

    public SoundCategory default_category { get; set; }

    public GenericSound() {
        sound_name = "new sound";
        volume = 1f;
        default_category = SoundCategory.sound_effect;
    }

    public List<ISound> GetSounds() {
        return new List<ISound>(new ISound[]{this});
    }
}