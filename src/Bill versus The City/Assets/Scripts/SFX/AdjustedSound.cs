using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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
    
    [Tooltip("If set, this will override the default audio mixer for the sound's SoundCategory.")]
    public AudioMixerGroup _override_mixer_group = null;
    public AudioMixerGroup GetMixerGroup(SoundCategory category) {
        if (_override_mixer_group == null) {
            return AudioMixerManager.inst.GetMixerGroup(category);
        }
        return _override_mixer_group;
        
    }
    
    public List<ISingleSFXSound> GetSounds() {
        return new List<ISingleSFXSound>(new ISingleSFXSound[]{this});
    }
}