

using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesSoftKillTimer : ParticlesSoftKiller
{
    /* extends ParticlesSoftKiller to automatically trigger on a timer */

    [Tooltip("time before this object destroys itself")]
    public float duration = 5f;

    private float destroy_after = float.PositiveInfinity;


    void Start() {
        destroy_after = Time.time + duration;
    }

    void Update() {
        if (Time.time >= destroy_after) {
            SoftKill();
        }
        UpdateDebug();
    }

    # if UNITY_EDITOR
    public ParticlesSoftKillTimerDebugger debug = new ParticlesSoftKillTimerDebugger();
    # endif
    private void UpdateDebug() {
        # if UNITY_EDITOR
        debug.time_remaining = destroy_after - Time.time;
        debug.particle_sys_count = particle_systems.Count;
        # endif
    }
}

# if UNITY_EDITOR
[Serializable]
public class ParticlesSoftKillTimerDebugger {
    public int particle_sys_count = -1;
    public float time_remaining = float.PositiveInfinity;   
}
# endif