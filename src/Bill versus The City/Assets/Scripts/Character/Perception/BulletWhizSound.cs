

using UnityEngine;

[Tooltip("creates a sound enemies can hear for bullet's whizing through a room.")]
public class BulletWhizSound : MonoBehaviour {
    // creates a sound enemies can hear for bullet's whizing through a room.
    // does NOT create a sound effect for the player
    
    [Tooltip("How far away will sound be audible.")]
    public float base_range = 5f;
    [Tooltip("how much will sound heard by enemies alert them.")]
    public float base_alert_rate = 0.5f;
    public float interval_seconds = 0.33f;

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
            CreateMovementSound();
            SamplePosition();
            last_sample_time = Time.time;
        }
    }

    private void CreateMovementSound()
    {
        float distance_traveled = Vector3.Distance(GetSampledPosition(), last_position);
        float range = base_range + distance_traveled;
        float alarm_level = base_alert_rate * distance_traveled * interval_seconds;
        GameSound new_sound = new GameSound(GetSampledPosition(), range, alarm_level);
        new_sound.sound_name = "bullet whiz";
        new_sound.penetrate_walls = false;
        EnemyHearingManager.inst.NewSound(new_sound);
    }

}