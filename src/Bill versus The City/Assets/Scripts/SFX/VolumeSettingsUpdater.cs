
using UnityEngine;

public class VolumeSettingsUpdater : MonoBehaviour, ISettingsObserver {
    
    public SoundCategory sound_category;
    public AudioSource audio_source;


    void Start() {
        GameSettings.inst.audio_settings.Subscribe(this);
    }

    void OnDestroy() {
        GameSettings.inst.audio_settings.Unsubscribe(this);
    }
    
    public void SettingsUpdated(ISettingsModule updated, string field) {
        float new_volume = GameSettings.inst.audio_settings.GetVolume(sound_category);
        Debug.LogWarning($"{gameObject.name}: volume updated {audio_source.volume} --> {new_volume}"); // TODO --- remove debug
        audio_source.volume = new_volume;
        MenuManager.inst.FrameUnpause();
    }
}