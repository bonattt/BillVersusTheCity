using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.PlayerLoop;

public class SFXSystem : MonoBehaviour, ISettingsObserver
{ 
    public AudioSource sfx_object_prefab;
    private AudioSource music_player;
    private ISingleSFXSound current_music;
    private static SFXSystem _inst = null;
    public static SFXSystem inst {
        get {
            return _inst;
        }
    }

    void Awake() {
        _inst = this;
    }

    void Start() {
        GameSettings.inst.audio_settings.Subscribe(this);
    }

    public void PlayMusic(ISFXSounds sounds) {
        // overloads _PlayMusic
        PlayMusic(sounds, PlayerCharacter.inst.player_transform);
    }
    
    public void PlayMusic(ISingleSFXSound sound) {
        // overloads _PlayMusic
        PlayMusic(sound, PlayerCharacter.inst.player_transform);
    }

    public void PlayMusic(ISFXSounds sound, Transform target) {
        // overloads _PlayMusic
        if (sound == null) {
            Debug.LogWarning("empty music argument");
            return;
        }
        PlayMusic(sound.GetRandomSound(), target);
    }
    public void PlayMusic(ISingleSFXSound sound, Transform target) {
        // overloads _PlayMusic
        if (sound == null) {
            Debug.LogWarning("empty music argument");
            return;
        }
        _PlayMusic(sound, target);
    }

    private void _PlayMusic(ISingleSFXSound sound, Transform target) {
        // Plays a single music track at a time, replacing the previous music track if one was playing
        CleanUpMusicPlayer();
        Vector3 start_pos;
        if (target != null) {
             start_pos = target.position;
        } else {
            start_pos = new Vector3(0, 0, 0);
        }
        music_player = CreatePlayer(sound.clip, start_pos, GetVolume(sound));
        music_player.Play();
        music_player.loop = true;
        current_music = sound;

        if (target != null) {
            music_player.transform.parent = target;
        }
    }

    public void StopMusic() {
        CleanUpMusicPlayer();
    }

    private void CleanUpMusicPlayer() {
        current_music = null;
        if (music_player != null) {
            music_player.Stop();
            Destroy(music_player.gameObject);
        }
    }

    public AudioSource PlaySound(ISFXSounds sound, Vector3 target, bool loop=false) {
        if (sound == null) {
            Debug.LogWarning("empty sound effect");
            return null;
        }
        AudioSource audio_source =  PlaySound(sound.GetRandomSound(), target, loop:loop);
        SoundInstanceData instance_data = audio_source.gameObject.GetComponent<SoundInstanceData>();
        instance_data.sound_set = sound;
        return audio_source;
    }

    public AudioSource PlaySound(ISingleSFXSound sound, Vector3 target, bool loop=false) {
        if (sound == null) {
            Debug.LogWarning("empty sound effect");
            return null;
        }
        AudioClip audio_clip = sound.clip;
        SoundCategory sound_category = sound.default_category;

        float volume = GetVolume(sound);
        if (audio_clip == null) {
            Debug.LogWarning("empty sound effect");
            return null;
        }
        AudioSource audio_source = CreatePlayer(audio_clip, target, volume);
        // PlayingSoundInterface instance_manager = audio_source.gameObject.GetComponent<PlayingSoundInterface>();
        // instance_manager.StartPlayback(sound, loop);
        // instance_manager.sound_category = sound_category;

        SoundInstanceData instance_data = audio_source.gameObject.AddComponent<SoundInstanceData>();
        instance_data.sound = sound;
        instance_data.audio_source = audio_source;
        instance_data.category = sound_category;

        AudioMixerGroup mixer = sound.GetMixerGroup(sound_category);
        audio_source.outputAudioMixerGroup = mixer;
        audio_source.Play();
        return audio_source;
    }

    private AudioSource CreatePlayer(AudioClip audio_clip, Vector3 target, float volume) {
        AudioSource audio_source = Instantiate(sfx_object_prefab, target, Quaternion.identity);
        audio_source.clip = audio_clip;
        audio_source.volume = volume;

        return audio_source;
        
    }

    // public void PlayRandomClip(AudioClip[] audio_clips, Vector3 target, float volume) {
    //     int rand = Random.Range(0, audio_clips.Length);
    //     PlaySound(audio_clips[rand], SoundCategory.sound_effect, target, volume);
    // }

    public void PlayRandomClip(ISFXSounds sound_set, Vector3 target) {
        PlaySound(sound_set.GetRandomSound(), target);
    }

    public static float GetVolume(ISingleSFXSound sound) {
        return sound.volume; // * settings.GetVolume(sound.default_category); // volume now handled with a unity AudioMixer
    }

    public void SettingsUpdated(ISettingsModule updated, string field) {
        switch(updated) {
            case AudioSettings audio_settings:
                Debug.Log("audio settings updated!");
                UpdateMusicVolume();
                break;
            
            default:
                // do nothing, non-audio settings
                return;
        }
    }

    private void UpdateMusicVolume() {
        // updates the volume of the currently playing music track to the current volume settings
        if (music_player == null) {
            return;
        } // no live music to update
        if (current_music == null) {
            Debug.LogError("Cannot update music volume; music player exists, but current music is not set!");
            return;
        }
    }
}

public enum SoundCategory {
    master,
    sound_effect,
    music,
    menu
}