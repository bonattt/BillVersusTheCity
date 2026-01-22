
using UnityEngine;

public class FlashbangDazeParticles : MonoBehaviour {
    [SerializeField] private ParticleSystem particles;
    public EnemyPerception perception;
    public Transform follow;

    private const float particle_rate_blind = 10f;
    private const float particle_rate_dazed = 1f;
    private bool stopped = false;

    void Start() {
        UpdateParticles();
        FollowTarget();
    }

    void Update() {
        UpdateParticles();
        FollowTarget();
    }

    private void FollowTarget() {
        if (follow == null) { return; } // target destroyed
        transform.position = follow.position;
        transform.rotation = follow.rotation;
    }

    private void UpdateParticles() {
        if (stopped) { return; }
        if (perception.is_flashbang_blinded) {
            SetParticlesBlind();
        }
        else if (perception.is_flashbang_dazed) {
            SetParticlesDaze();
        }
        else {
            SetParticlesNone();
        }
    }

    private void SetParticlesBlind() { 
        var emission = particles.emission;
        emission.rateOverTime = particle_rate_blind;
    }
    private void SetParticlesDaze() {
        var emission = particles.emission;
        emission.rateOverTime = particle_rate_dazed;
    }
    private void SetParticlesNone() {
        particles.Stop();
        Destroy(gameObject, 25f); // give particles time to disappear naturally, before cleaning up the particle system.
    }

    

    private enum FlashBangState {
        none,
        daze,
        blind,
    }
}