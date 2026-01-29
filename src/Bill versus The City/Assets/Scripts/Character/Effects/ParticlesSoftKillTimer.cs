

using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesSoftKillTimer : MonoBehaviour
{
    // class that sets a time to live on a particle effect, but detatches and stops the particles before destruction, \
    // so they can fade naturally instead of winking out suddenly

    [Tooltip("time before this object destroys itself")]
    public float duration = 5f;
    [Tooltip("Once duration expires, particles will have this many seconds to fade out naturally before the ParticleSystem is destroyed.")]
    public float particle_wind_down_buffer_time = 5f;

    public List<ParticleSystem> particle_systems = new List<ParticleSystem>();

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

    private void SoftKill() {
        foreach (ParticleSystem p in particle_systems) {
            SoftKillOne(p);
        }
        Destroy(gameObject);
    }

    private void SoftKillOne(ParticleSystem particles) {
        particles.Stop();
        particles.transform.parent = GarbageBin.transform;
        Destroy(particles.gameObject, particle_wind_down_buffer_time);
    }

    public void AddParticleSystems(IEnumerable<ParticleSystem> new_systems) {
        foreach (ParticleSystem p in new_systems) {
            particle_systems.Add(p);
        }
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