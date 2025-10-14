
using System;
using UnityEngine;

public class PoliceTimer : MonoBehaviour, IGlobalSoundsObserver {

    public LevelConfig level_config;
    [Tooltip("Once the police are alerted, the player will have this many seconds to complete the level.")]
    public float seconds = 180;
    private float alerted_at = -1f;
    public bool police_alerted { get; private set; }

    public float seconds_remaining {
        get {
            if (!police_alerted) {
                return seconds;
            }
            return seconds - (Time.time - alerted_at);
        }
    }

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
    }

    public PoliceTimerDebugger debug;
    void Update() {
        CheckTimer();
        UpdateDebug();
    }

    private void CheckTimer() {
        if (seconds_remaining < 0f) {
            TriggerTimer();
        }
    }

    private void TriggerTimer() {
        Debug.LogWarning("TODO --- implement trigger timer");
        level_config.FailLevel();
    }
    
    void UpdateDebug() {
        debug.police_alerted = police_alerted;
        debug.alerted_at = alerted_at;
        debug.seconds_remaining = seconds_remaining;
    }
}

[Serializable]
public class PoliceTimerDebugger {
    public bool police_alerted = false;
    public float alerted_at;
    public float seconds_remaining;
}
