
using System;
using UnityEngine;

public class PoliceTimer : MonoBehaviour, IGlobalSoundsObserver, ITimer {

    public LevelConfig level_config;
    [Tooltip("Once the police are alerted, the player will have this many seconds to complete the level.")]
    public float seconds = 180;
    private float alerted_at = -1f;
    public bool police_alerted { get; private set; }
    public bool has_triggered { get; private set; }
    public TimerUIController ui;
    ////////////////////////////////
    //////// ITimer fields /////////
    ////////////////////////////////
    public float remaining_time {
        get {
            if (!police_alerted) {
                return seconds;
            }
            return seconds - (Time.time - alerted_at);
        }
    }
    public bool paused { get => !police_alerted; }
    public int remaining_minutes { get { return (int) (Mathf.Round(remaining_time) / 60); }}

    public int remaining_seconds_remainder { get { return (int) (Mathf.Round(remaining_time) - (remaining_minutes * 60)); }}
    public int remaining_seconds_full { get { return (int) Mathf.Round(remaining_time); }}
    ////////////////////////////////

    void Start() {
        Initialize();
        EnemyHearingManager.inst.Subscribe(this);
    }

    void OnDestroy() {
        EnemyHearingManager.inst.Unsubscribe(this);
    }

    public void UpdateSound(ISound sound) {
        if (sound.alerts_police && !police_alerted) {
            AlertPolice();
        }
    }

    public void AlertPolice() {
        if (alerted_at > 0) {
            Debug.LogWarning($"police alerted while their `alerted_at` was already set to '{alerted_at}'");
        }
        police_alerted = true;
        alerted_at = Time.time;
    }

    private void Initialize() {
        police_alerted = false;
        has_triggered = false;
        ui.AttachTimer(this);
    }

    public PoliceTimerDebugger debug;
    void Update() {
        CheckTimer();
        UpdateDebug();
    }

    private void CheckTimer() {
        if (remaining_time < 0f && !has_triggered) {
            TriggerTimer();
        }
    }

    private void TriggerTimer() {
        has_triggered = true;
        level_config.FailLevel();
    }
    
    void UpdateDebug() {
        debug.police_alerted = police_alerted;
        debug.alerted_at = alerted_at;
        debug.remaining_time = remaining_time;
        debug.has_triggered = has_triggered;
    }
}

[Serializable]
public class PoliceTimerDebugger {
    public bool police_alerted = false;
    public bool has_triggered = false;
    public float alerted_at;
    public float remaining_time;
}
