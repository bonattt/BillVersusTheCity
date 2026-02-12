using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public interface ISingleSFXSound {
    public string sound_name { get; set; }
    public AudioClip clip { get; set; }
    public float volume { get; set; }
    public SoundCategory default_category { get; set; }
    public AudioMixerGroup GetMixerGroup() => GetMixerGroup(default_category);
    public AudioMixerGroup GetMixerGroup(SoundCategory category);
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


public interface IPlayingSound {
    // interface for a scripts that manage an instance of a sound being played
    public ISingleSFXSound sound_playing { get; set; }
    public ISFXSounds source_sounds { get; set; }
    public SoundCategory sound_category { get; set; }
    public AudioSource audio_source { get; }

    public float volume_setting { get; set; } // 1-0 volume setting, which will be multiplied against the volume of `sound_playing` before it is applied to the audio_source
    public bool loop { get; } 
    public void StartPlayback(ISingleSFXSound sound, bool loop);
    public void StartPlayback(ISFXSounds sound, bool loop);
    public void StopPlayback();

}