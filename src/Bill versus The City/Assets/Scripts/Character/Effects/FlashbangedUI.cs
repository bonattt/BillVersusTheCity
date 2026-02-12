

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FlashbangedUI : MonoBehaviour {
    
    public UIDocument ui_doc;
    private float _flashbanged_at = float.NegativeInfinity;
    public float blind_until { get; private set; }
    public float dazed_until { get; private set; }

    private VisualElement panel;

    public bool active = true;

    public string tinnitus_sound_path = "WeaponEffects/flashbang_tinnitus";
    private ISFXSounds _sound = null;
    private ISFXSounds sound {
        get {
            if (_sound == null) {
                _sound = SFXLibrary.LoadSound(tinnitus_sound_path);
            }
            return _sound;
        }
    }
    private AudioSource tinnitus_sound = null;


    public static FlashbangedUI inst { get; private set; }
    void Awake() {
        inst = this;
    }

    void Start() {
        blind_until = float.NegativeInfinity; // set initial value to prevent screen being white for 1 frame on game start
        dazed_until = float.NegativeInfinity; // set initial value to prevent screen being white for 1 frame on game start
        Init();
        UpdateOpacity();
    }

    void Update() {
        // if (active) {
            UpdateOpacity(); // blind effect
            UpdateTinnitusVolume(); // tinnitus effect
        // }
        UpdateDebug();
    }

    private void Init() {
        panel = ui_doc.rootVisualElement.Q<VisualElement>("flashbang");

    }

    private void UpdateOpacity() {
        Color c =  new Color(1f, 1f, 1f, GetBlindPercent());
        panel.style.backgroundColor = c;
    }

    private void UpdateTinnitusVolume() {
        if (tinnitus_sound == null) { 
            Debug.LogWarning($"cannot update volume, no tinnitus sound is set!!");
            return;
        }
        float percent =  GetDeafPercent();
        if (percent <= 0f) {
            tinnitus_sound.Stop();
        } else {
            tinnitus_sound.volume = percent;
        }
    }

    private float GetDeafPercent() {
        if(!active) { return 0f; }
        if (dazed_until <= Time.time) { return 0f; }
        
        float time_recovering = Time.time - _flashbanged_at; // seconds after blind starts fading
        float period = dazed_until - _flashbanged_at; // time it takes to go from fully opaque at the midpoint to fully transparent

        float percent = (period - time_recovering) / period;
        return percent;
    }

    private float GetBlindPercent() {
        if(!active) { return 0f; }
        if (blind_until >= Time.time) { return 1f; }
        if (dazed_until <= Time.time) { return 0f; }
        
        float time_recovering = Time.time - blind_until; // seconds after blind starts fading
        float period = dazed_until - blind_until; // time it takes to go from fully opaque at the midpoint to fully transparent

        float percent = (period - time_recovering) / period;
        return percent;
    }

    public void FlashbangUntil(float blind_duration) {
        Debug.LogWarning($"blind duration: {blind_duration}");
        _flashbanged_at = Time.time;
        blind_until = Time.time + blind_duration;
        dazed_until = Time.time + (blind_duration * 3);
        StartFlashbangSound();
    }

    private void StartFlashbangSound() {
        if (tinnitus_sound != null) {
            Debug.LogWarning("clearing old tinnitus_sound");
            tinnitus_sound.Stop();
        }
        tinnitus_sound = SFXSystem.inst.PlaySound(sound, transform.position, loop:true);
    }


    public FlashbangedUIDebugger debug;
    private void UpdateDebug() {
        debug.percent_blind = GetBlindPercent();
        debug.percent_deaf = GetDeafPercent();
        debug.blind_until = blind_until;
        debug.dazed_until = dazed_until;
        debug.blind_remaining = Mathf.Max(0, blind_until - Time.time);
        debug.daze_remaining =  Mathf.Max(0, dazed_until - Time.time);
        debug.tinnitus_volume = tinnitus_sound == null ? -1 : tinnitus_sound.volume;
    }

}

[Serializable]
public class FlashbangedUIDebugger {
    [Header("audio")]
    public float tinnitus_volume;
    [Header("durations")]
    public float percent_blind; // only place header once
    public float percent_deaf, blind_until, dazed_until, blind_remaining, daze_remaining;
}