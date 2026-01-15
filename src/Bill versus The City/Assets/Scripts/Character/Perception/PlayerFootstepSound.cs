

using UnityEngine;

public class PlayerFootstepSound : MonoBehaviour {
    [Tooltip("How far away will footsteps be audible.")]
    public float base_range = 5f;
    [Tooltip("how much will footsteps heard by enemies alert them.")]
    public float base_alert_rate = 0.1f;
    public float interval_seconds = 0.33f;
    [Tooltip("if the player traveled less than this value during a sampling, no sound is created. (value is adjusted based on `interval_seconds`)")]
    public float distance_threshold_seconds = 0.1f;

    // this caps the distance that can be sampled; this is to avoid making a really loud sound when the player is teleported to a new floor.
    private float max_travel_distance = 12f; // TODO --- this should be more dynamic; right now the player moves 5.81 per .33 interval, I doubled that

    [Tooltip("This script calculates a max allowed distance to avoid creating massive sounds when the player is teleported. The cap is player max speed, times the safety margin.")]
    public float max_distance_safety_margin = 1.1f;

    [Tooltip("if disabled, the player will make no sound if their movement exceeds the max movement threshold (eg. teleportation).")]
    public bool make_sound_over_movement_threshold = false;

    public float distance_threshold {
        get {
            // adjusts distance_threshold_seconds to reflect travel-distance per interval, instead of travel-distance per second
            return distance_threshold_seconds * interval_seconds;
        }
    }

    [Tooltip("The effects of player's speed is multiplied by this amount.")]
    public float speed_factor = 2f;

    private float last_sample_time = 0f;
    private Vector3 last_position;

    protected void SamplePosition() {
        last_position = GetSampledPosition();
    }
    public Vector3 GetSampledPosition() {
        return new Vector3(transform.position.x, 0f, transform.position.z);
    }

    private void SetMaxSpeed() {
        ManualCharacterMovement player = GetComponent<ManualCharacterMovement>();
        float max_player_distance = player.max_movement_speed * interval_seconds;
        max_travel_distance = max_distance_safety_margin * max_player_distance;
    }

    void Start() {
        SetMaxSpeed();
        SamplePosition();
    }

    void Update() {
        if (last_sample_time + interval_seconds <= Time.time) {
            CreateFootstepSound();
            SamplePosition();
            last_sample_time = Time.time;
        }
    }

    private void CreateFootstepSound() {
        float distance_traveled = Vector3.Distance(GetSampledPosition(), last_position);
        if (distance_traveled < distance_threshold) {
            // no sound while not moving
            return;
        } else if (distance_traveled >= max_travel_distance) {
            if (!make_sound_over_movement_threshold) {
                Debug.Log($"skipped making sound because distance '{distance_traveled}' exceeded maximum '{max_travel_distance}'");
                return;
            }
            Debug.LogWarning("creating max footstep sound!");
            // TODO --- demote these from warning logs
        }
        float range = base_range + distance_traveled;
        float alarm_level = base_alert_rate * distance_traveled * interval_seconds;
        GameSound new_sound = new GameSound(GetSampledPosition(), range, alarm_level);
        new_sound.alerts_police = false;
        new_sound.sound_name = "footstep";
        EnemyHearingManager.inst.NewSound(new_sound);
    }
}