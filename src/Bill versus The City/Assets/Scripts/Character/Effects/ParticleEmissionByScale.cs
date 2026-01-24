

using System;
using UnityEngine;

public class ParticleEmissionByScale : MonoBehaviour
{
    // Scales the emission rate of a particle system to match the Transform's volume,
    // allowing particle effect to have a flexible area without changing particle size.

    public ParticleSystem particle_sys;
    public bool update_at_runtime = false;

    private float base_rate_over_time;
    private int base_max_particles;

    private float current_volume = 0;

    public float rate_over_time {
        get => particle_sys.emission.rateOverTime.constant;
        set {
            var emission = particle_sys.emission;
            emission.rateOverTime = value;
        }
    }
    public int max_particles {
        get => particle_sys.main.maxParticles;
        set {
            var main = particle_sys.main;
            main.maxParticles = value;
        }
    }

    void Start() {
        SetBaseline();
    }

    void Update() {
        if (!update_at_runtime) { return; } // only run scaling at runtime of this bool is checked.
        UpdateEmission();
    }

    private void UpdateEmission() {
        current_volume = GetTransformVolume();
        rate_over_time = base_rate_over_time * current_volume;
        max_particles = (int) (base_max_particles * current_volume);
    }


    private void SetBaseline() {
        // stores the initial values of the particle system's emission and volume, so it can be scaled dynamically
        current_volume = GetTransformVolume();
        base_rate_over_time = rate_over_time / current_volume;
        base_max_particles = (int) (max_particles / current_volume);
    }


    public float GetTransformVolume() {
        // gets the scaled volume of the transform. Assumes the shape is a circle
        // TODO --- this algorithm can be dramatically improved
        float radius = transform.localScale.x / 2;
        return radius * radius * Mathf.PI;

    }

    public ParticleEmissionByScaleDebugger debug;
    private void UpdateDebug() {
        debug.base_rate_over_time = base_rate_over_time; 
        debug.base_max_particles = base_max_particles; 
        debug.rate_over_time = rate_over_time; 
        debug.max_particles = max_particles; 
        debug.current_volume = GetTransformVolume();
    }


}

[Serializable]
public class ParticleEmissionByScaleDebugger {
    public float base_rate_over_time, base_max_particles, rate_over_time, max_particles, current_volume;
}
