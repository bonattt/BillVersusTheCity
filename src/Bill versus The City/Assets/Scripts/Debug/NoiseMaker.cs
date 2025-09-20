

using UnityEngine;

public class NoiseMaker : MonoBehaviour
{
    // debug class that makes noises at an interval

    public float interval = 2f;
    public float sound_alert_level = 1f;
    public float sound_range = 5f;
    public float spawn_radius = 2f;
    private float last_sound_time = 0f;

    void Update()
    {
        if (last_sound_time + interval <= Time.time)
        {
            last_sound_time = Time.time;
            SpawnSound();
        }
    }

    public void SpawnSound()
    {
        Vector3 direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * Random.Range(0, spawn_radius);
        Vector3 origin = transform.position + direction;
        GameSound new_sound = new GameSound(origin, sound_range, sound_alert_level);
        new_sound.sound_name = "Test Noisemaker";
        EnemyHearingManager.inst.NewSound(new_sound);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawCube(transform.position, new Vector3(0.75f, 0.75f, 0.75f));
    }
}