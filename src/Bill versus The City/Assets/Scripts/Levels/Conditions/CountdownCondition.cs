
using System;
using System.Collections.Generic;

using UnityEngine;


public class CountdownCondition : AbstractLevelCondition, ITimer {
    
    [SerializeField]
    private bool _paused = false;
    public bool paused { 
        get {return _paused; } 
        set { _paused = value; }
    }
    public float start_time_seconds = 60 * 5; // 5 minute default
    private float _countdown = 0f;
    public float remaining_time { get { return _countdown; }}
    // public int remaining_minutes { get { return (int) (_countdown / 60); }}
    // public int remaining_seconds_remainder { get { return (int) (_countdown - (remaining_minutes * 60)); }} // returns the seconds left in the current minute
    // public int remaining_seconds_full { get { return (int) _countdown; }}
    
    // public bool is_active { get; set; }
    // public bool was_triggered { get; set; }

    // public List<MonoBehaviour> init_effects;
    // public List<IGameEventEffect> effects { get; private set; }

    protected override void Start() {
        base.Start();
        _countdown = start_time_seconds;
    }

    void Update() {
        if (!paused) {
            _countdown -= Time.deltaTime;
        }
        UpdateDebugData();
    }

    public override bool ConditionMet() {
        return _countdown <= 0f;
    }

    //////////////////// DEBUG FIELDS ///////////////////////
    public float debug_remaining_time;
    public int debug_remaining_minutes, debug_remaining_seconds_remainder, debug_remaining_seconds_full;
    private void UpdateDebugData() {
        debug_remaining_time = remaining_time;
        debug_remaining_minutes = ((ITimer) this).remaining_minutes;
        debug_remaining_seconds_remainder = ((ITimer) this).remaining_seconds_remainder;
        debug_remaining_seconds_full = ((ITimer) this).remaining_seconds_full;
    }
}