using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSound : ISingleSound, ISounds {
    public string sound_name  { get; set; }
    public AudioClip clip  { get; set; }

    public float volume { get; set; }

    public SoundCategory default_category { get; set; }

    public GenericSound() {
        sound_name = "new sound";
        volume = 1f;
        default_category = SoundCategory.sound_effect;
    }

    public List<ISingleSound> GetSounds() {
        return new List<ISingleSound>(new ISingleSound[]{this});
    }
}