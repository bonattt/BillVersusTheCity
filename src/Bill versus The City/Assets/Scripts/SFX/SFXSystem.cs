using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXSystem : MonoBehaviour
{

    public AudioSource sfx_object;
    private static SFXSystem _instance;
    public static SFXSystem instance {
        get {
            return _instance;
        }
    }

    void Awake() {
        _instance = this;
    }

    public void PlaySound(ISoundSet sound, Vector3 target) {
        PlaySound(sound.GetRandomSound(), target);
    }

    public void PlaySound(ISound sound, Vector3 target) {
        PlaySound(sound.clip, target, sound.volume);
    }

    public void PlaySound(AudioClip audio_clip, Vector3 target, float volume) {
        // based on tutoral at `https://www.youtube.com/watch?v=DU7cgVsU2rM`
        Debug.Log(sfx_object);
        AudioSource audio_source = Instantiate(sfx_object, target, Quaternion.identity);
        audio_source.clip = audio_clip;
        audio_source.volume = volume;
        audio_source.Play();

        // destroy when sound finishes  
        float clip_length = audio_source.clip.length;
        Destroy(audio_source, clip_length);

    }

    public void PlayRandomClip(AudioClip[] audio_clips, Vector3 target, float volume) {
        int rand = Random.Range(0, audio_clips.Length);
        PlaySound(audio_clips[rand], target, volume);
    }

    public void PlayRandomClip(ISoundSet sound_set, Vector3 target) {
        PlaySound(sound_set.GetRandomSound(), target);
    }
}

public enum SoundCategory {
    sound_effect,
    music,
    menu
}