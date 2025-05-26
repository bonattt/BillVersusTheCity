using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public AudioSource PlaySound(ISFXSounds sound, Vector3 target) {
        if (sound == null) {
            Debug.LogWarning("empty sound effect");
            return null;
        }
        return PlaySound(sound.GetRandomSound(), target);
    }

    public AudioSource PlaySound(ISingleSFXSound sound, Vector3 target) {
        if (sound == null) {
            Debug.LogWarning("empty sound effect");
            return null;
        }
        return PlaySound(sound.clip, target, GetVolume(sound));
    }

    public AudioSource PlaySound(AudioClip audio_clip, Vector3 target, float volume) {
        if (audio_clip == null) {
            Debug.LogWarning("empty sound effect");
            return null;
        }
        AudioSource audio_source = CreatePlayer(audio_clip, target, volume);
        audio_source.Play();
        // destroy when sound finishes  
        float clip_length = audio_source.clip.length;
        Destroy(audio_source.gameObject, clip_length);
        return audio_source;
    }

    private AudioSource CreatePlayer(AudioClip audio_clip, Vector3 target, float volume) {
        // based on tutoral at `https://www.youtube.com/watch?v=DU7cgVsU2rM`
        AudioSource audio_source = Instantiate(sfx_object_prefab, target, Quaternion.identity);
        audio_source.clip = audio_clip;
        audio_source.volume = volume;

        return audio_source;
        
    }

    public void PlayRandomClip(AudioClip[] audio_clips, Vector3 target, float volume) {
        int rand = Random.Range(0, audio_clips.Length);
        PlaySound(audio_clips[rand], target, volume);
    }

    public void PlayRandomClip(ISFXSounds sound_set, Vector3 target) {
        PlaySound(sound_set.GetRandomSound(), target);
    }

    public static float GetVolume(ISingleSFXSound sound) {
        // gets the volume level (0-1 percent) by applying the master volume, category volume, and clip volume
        AudioSettings settings = GameSettings.inst.audio_settings;
        return sound.volume * settings.GetVolume(sound.default_category);
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
        if (music_player == null) { return; } // no live music to update
        if (current_music == null) {
            Debug.LogError("Cannot update music volume; music player exists, but current music is not set!");
            return;
        }
        music_player.volume = GetVolume(current_music);
    }
}

public enum SoundCategory {
    sound_effect,
    music,
    menu
}