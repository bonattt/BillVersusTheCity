

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
        UpdateEmission();
        # if UNITY_EDITOR
        UpdateDebug();
        # endif
    }

    void Update() {
        if (!update_at_runtime) { return; } // only run scaling at runtime of this bool is checked.
        UpdateEmission();
        
        # if UNITY_EDITOR
        UpdateDebug();
        # endif
    }

    private void UpdateEmission() {
        current_volume = GetTransformVolume();
        rate_over_time = base_rate_over_time * current_volume;
        max_particles = (int) (base_max_particles * current_volume);
    }


    private bool baseline_set = false;
    public void SetBaseline() {
        Debug.LogWarning("SetBaseline"); // TODO --- remove debug
        // stores the initial values of the particle system's emission and volume, so it can be scaled dynamically
        // method is IDEMPOTENT, so it can be called before Start, when spawning this from a script (the standard way to use this)
        if (baseline_set) { return; }  // don't overwrite the baseline if it was already set
        baseline_set = true;
        current_volume = IdentityVolume(); // assumes the initial config of the particle system are set for particles at a scale of <1,1,1>
        base_rate_over_time = rate_over_time / current_volume;
        base_max_particles = (int) (max_particles / current_volume);
    }
    
    public static float IdentityVolume() {
        // returns the volume when the object is not scaled (scale.<x, y, z> = <1, 1, 1>)
        float radius_squared = 1f/4f; // 1/2 squared, for r^2 * Pi
        return radius_squared * Mathf.PI;
    }

    public float GetTransformVolume() {
        // gets the scaled volume of the transform. Assumes the shape is a circle
        // TODO --- this algorithm can be dramatically improved
        float radius = transform.localScale.x / 2;
        return radius * radius * Mathf.PI;

    }

# if UNITY_EDITOR
    public ParticleEmissionByScaleDebugger debug;
    private void UpdateDebug() {
        debug.base_rate_over_time = base_rate_over_time; 
        debug.base_max_particles = base_max_particles; 
        debug.rate_over_time = rate_over_time; 
        debug.max_particles = max_particles; 
        debug.current_volume = GetTransformVolume();
        debug.identity_volume = IdentityVolume();
    }
#endif
}

# if UNITY_EDITOR
[Serializable]
public class ParticleEmissionByScaleDebugger {
    public float base_rate_over_time, base_max_particles, rate_over_time, max_particles, current_volume, identity_volume;
}
#endif
