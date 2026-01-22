using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Sound", menuName ="Data/Sound")]
public class AdjustedSound : ScriptableObject, ISingleSFXSound, ISFXSounds
{
    public string _sound_name = "new sound";
    public AudioClip _clip;

    public float _volume = 1f;

    public SoundCategory _default_category = SoundCategory.sound_effect;
    
    public string sound_name { 
        get => _sound_name;
        set {
            _sound_name = value;
        }
    }
    public AudioClip clip {
        get =>_clip;
        set {
            _clip = value;
        }
    }
    public float volume { 
        get => _volume; 
        set {
            _volume = value;
        }
    }
    public SoundCategory default_category { 
        get => _default_category;
        set {
            _default_category = value;
        }
    }
    
    public List<ISingleSFXSound> GetSounds() {
        return new List<ISingleSFXSound>(new ISingleSFXSound[]{this});
    }
}