

using UnityEngine;

public class PlayerFootstepSound : MonoBehaviour
{
    [Tooltip("How far away will footsteps be audible.")]
    public float base_range = 1f;
    [Tooltip("how much will footsteps heard by enemies alert them.")]
    public float base_alert_rate = 0.1f;
    public float interval_seconds = 0.33f;
    [Tooltip("if the player traveled less than this value during a sampling, no sound is created. (value is adjusted based on `interval_seconds`)")]
    public float distance_threshold_seconds = 0.1f;

    public float distance_threshold
    {
        get {
            // adjusts distance_threshold_seconds to reflect travel-distance per interval, instead of travel-distance per second
            return distance_threshold_seconds * interval_seconds;
        }
    }

    [Tooltip("The effects of player's speed is multiplied by this amount.")]
    public float speed_factor = 2f;

    private float last_sample_time = 0f;
    private Vector3 last_position;

    protected void SamplePosition()
    {
        last_position = GetSampledPosition();
    }
    public Vector3 GetSampledPosition()
    {
        return new Vector3(transform.position.x, 0f, transform.position.z);
    }

    void Start()
    {
        SamplePosition();
    }

    void Update()
    {
        if (last_sample_time + interval_seconds <= Time.time)
        {
            CreateFootstepSound();
            SamplePosition();
            last_sample_time = Time.time;
        }
    }

    private void CreateFootstepSound()
    {
        float distance_traveled = Vector3.Distance(GetSampledPosition(), last_position);
        if (distance_traveled < distance_threshold)
        {
            Debug.Log("no sound while not moving");
            Debug.LogWarning("no sound while not moving"); // TODO --- remove debug
            return;
        }
        float range = base_range + distance_traveled;
        float alarm_level = base_alert_rate * distance_traveled;
        GameSound new_sound = new GameSound(GetSampledPosition(), range, alarm_level);
        EnemyHearingManager.inst.NewSound(new_sound);
    }
    

}