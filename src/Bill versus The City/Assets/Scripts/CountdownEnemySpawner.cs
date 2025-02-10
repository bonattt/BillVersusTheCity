using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CountdownEnemySpawner : AbstractEnemySpawner
{
    public float max_time_to_spawn_seconds = 45;
    public float min_time_to_spawn_seconds = 20f;

    [Tooltip("if true, the spawner will spawn enemies at all spawn points on the first tick. Otherwise, countdowns will be started from the first tick")]
    public bool spawn_on_start = true;
    
    
    [SerializeField]
    private float next_spawn_at = -1f; // cached next spawn time

    private Dictionary<ISpawnPoint, float> spawn_times;
    protected override void InitializeSpawnPoints() {
        base.InitializeSpawnPoints();
        spawn_times = new Dictionary<ISpawnPoint, float>();
        foreach (ISpawnPoint p in spawn_points) {
            if (spawn_on_start) {
                spawn_times[p] = -1f;
            } else {
                spawn_times[p] = GetRandomSpawnTime();
            }
        }
    }

    public float FindNextSpawnTime() {
        (ISpawnPoint p, float time) = GetNextSpawnAndTime();
        return time;
    }

    private (ISpawnPoint, float) GetNextSpawnAndTime() {
        float soonest_spawn_time = float.PositiveInfinity;
        ISpawnPoint soonest_point = null;
        foreach(ISpawnPoint p in spawn_times.Keys) {
            if (spawn_times[p] < soonest_spawn_time) {
                soonest_spawn_time = spawn_times[p];
                soonest_point = p;
            } 
        }

        if (float.IsInfinity(soonest_spawn_time)) {
            Debug.LogWarning("soonest spawn time is infinit!");
        }
        return (soonest_point, soonest_spawn_time);
    }

    private float GetRandomSpawnTime() {
        // gets a random next spawn time
        return Time.time + Random.Range(min_time_to_spawn_seconds, max_time_to_spawn_seconds);
    }
    
    private void UpdateNextSpawnAt() {
        // next_spawn_at = FindNextSpawnTime();
        spawn_times[cached_spawn_point] = GetRandomSpawnTime();
        (cached_spawn_point, next_spawn_at) = GetNextSpawnAndTime();
    }

    private ISpawnPoint cached_spawn_point = null;
    public override ISpawnPoint GetSpawnPoint()
    {
        return cached_spawn_point;
    }

    void Update()
    {
        if (Time.time > next_spawn_at) {
            (cached_spawn_point, next_spawn_at) = GetNextSpawnAndTime();
            SpawnEnemy();
            UpdateNextSpawnAt();
        }
    }
}
