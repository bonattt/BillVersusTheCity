

using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesSoftKiller : MonoBehaviour
{
    // class that detatches and stops ParticleSystems and gives them a time-to-live
    // so they can fade naturally instead of winking out suddenly when the parent object is destroyed

    [Tooltip("Once duration expires, particles will have this many seconds to fade out naturally before the ParticleSystem is destroyed.")]
    public float particle_wind_down_buffer_time = 5f;

    [SerializeField]
    protected List<ParticleSystem> particle_systems = new List<ParticleSystem>();

    protected List<ParticlesSoftKiller> children = new List<ParticlesSoftKiller>();
    public void AddChild(ParticlesSoftKiller child) => children.Add(child);
    public void RemoveChild(ParticlesSoftKiller child) => children.Remove(child);

    public void SoftKill() {
        foreach (ParticleSystem p in particle_systems) {
            SoftKillOne(p);
        }
        foreach (ParticlesSoftKiller c in children) {
            c.SoftKill();
        }
        Destroy(gameObject);
    }

    protected void SoftKillOne(ParticleSystem particles) {
        var particle_settings = particles.main;
        particle_settings.loop = false;
        particle_settings.playOnAwake = false;
        particles.Stop();
        particles.transform.parent = GarbageBin.transform;
        Destroy(particles.gameObject, particle_wind_down_buffer_time);
    }

    public void AddParticleSystems(IEnumerable<ParticleSystem> new_systems) {
        foreach (ParticleSystem p in new_systems) {
            particle_systems.Add(p);
        }
    }
}