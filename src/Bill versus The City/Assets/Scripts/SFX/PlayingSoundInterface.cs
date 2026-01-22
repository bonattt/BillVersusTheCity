
using UnityEngine;
using UnityEngine.Serialization;

public class PlayingSoundInterface : MonoBehaviour, ISettingsObserver, IPlayingSound {
    // interface layer that manages instances of AudioSources 
    
    
    [FormerlySerializedAs("audio_source")]
    [SerializeField] private AudioSource _audio_source;

    ///////// implement IPlayingSound /////////
    public ISingleSFXSound sound_playing { get; set; }
    public ISFXSounds source_sounds { get; set; }
    public SoundCategory sound_category { get; set; }
    public AudioSource audio_source => _audio_source;

    public float volume_setting { get; set; } // 1-0 volume setting, which will be multiplied against the volume of `sound_playing` before it is applied to the audio_source
    public bool loop { 
        get => audio_source.loop = loop;
        set {
            if (_started) {
                Debug.LogError($"cannot update loop once sound is playing!  sound: '{sound_playing.sound_name}'");
                return;
            }
            audio_source.loop = value;
        } 
    }
    private bool _started = false;
    private bool _stopped = false;
    public void StartPlayback(ISFXSounds sound_to_play, bool loop) {
        source_sounds = sound_to_play;
        StartPlayback(sound_to_play.GetRandomSound(), loop);
    }
    public void StartPlayback(ISingleSFXSound sound_to_play, bool loop) {
        if (_stopped) {
            Debug.LogError($"AudioSource should not be started once it has already been stopped. Create a new sound! sound: '{sound_playing.sound_name}'");
            return;
        }
        if (_started) { 
            Debug.LogWarning($"StartPlayback called more than once! sound_name: '{sound_playing.sound_name}'"); 
            return; 
        }
        sound_playing = sound_to_play;
        sound_category = sound_to_play.default_category;
        this.loop = loop;
        _started = true;
        AudioClip clip = sound_to_play.clip;
        audio_source.clip = clip;
        audio_source.Play();
        UpdateVolume();
        if (loop) {
            // do nothing. Looping sounds clean themselves up once stopped.
        } else {
            Destroy(gameObject, sound_playing.clip.length); // clean up once sound is finished
        }
        GameSettings.inst.audio_settings.Subscribe(this);
    }

    public void StopPlayback() {
        // manually stop playing a sound. Normally only used for looping sounds; 
        //   non-looping sounds should clean themselves up automatically when they finish
        audio_source.Stop();
        _stopped = true;
        Destroy(gameObject);
    }
    //////// END IPlayingSound implementation ///////////
    
    void OnDestroy() {
        GameSettings.inst.audio_settings.Unsubscribe(this);
    }
    
    public void SettingsUpdated(ISettingsModule updated, string field) {
        UpdateVolume();
    }

    private void UpdateVolume() {
        if (_stopped) { return; }
        float new_volume = SFXSystem.GetVolume(sound_playing);
        Debug.LogWarning($"{gameObject.name}: volume updated {audio_source.volume} --> {new_volume}"); // TODO --- remove debug
        audio_source.volume = new_volume;
        
    }
}