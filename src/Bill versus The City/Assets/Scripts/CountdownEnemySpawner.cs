using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CountdownEnemySpawner : AbstractEnemySpawner
{
    public float max_time_to_spawn_seconds = 45;
    public float min_time_to_spawn_seconds = 20f;
    [SerializeField]
    private float next_spawn_at = -1f;
    
    private void SetNextSpawnAt() {
        next_spawn_at = Time.time + Random.Range(min_time_to_spawn_seconds, max_time_to_spawn_seconds);
    }

    void Update()
    {
        if (Time.time > next_spawn_at) {
            SpawnEnemy();
            SetNextSpawnAt();
        }
    }
}
