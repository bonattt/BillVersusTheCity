using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSettingsUpdater : MonoBehaviour, ISettingsObserver {

    public AudioSource audio_source;
    private float _volume_cache;
    private float _setting_cache;
    // Start is called before the first frame update
    void Start() {
        GameSettings.inst.audio_settings.Subscribe(this);
    }

    // Update is called once per frame
    void Update() {

    }

    void OnDestroy() {
        GameSettings.inst.audio_settings.Unsubscribe(this);
    }

    public void SettingsUpdated(ISettingsModule updated, string field) {
        // TODO --- 
    }
}
