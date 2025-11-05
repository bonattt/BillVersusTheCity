
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemiesEvent : AbstractInteractionGameEvent {

    public ScriptableObject init_spawner_config;
    protected IEnemySpawnConfig spawner_config;
    public List<AbstractEnemySpawner> enemy_spawners;

    protected override void Effect() {
        SpawnEnemies();
    }

    protected override void Start() {
        base.Start();
        if (init_spawner_config != null) {
            spawner_config = (IEnemySpawnConfig)init_spawner_config;
            ApplySpawnConfigToChildren();
        }
    }

    protected void ApplySpawnConfigToChildren() {
        foreach (AbstractEnemySpawner spawn in enemy_spawners) {
            spawn.spawner_config = spawner_config;
        }
    }

    public List<GameObject> SpawnEnemies() {
        List<GameObject> enemies_spawned = new List<GameObject>();
        foreach (AbstractEnemySpawner spawn in enemy_spawners) {
            enemies_spawned.Add(spawn.SpawnEnemy());
        }
        return enemies_spawned;
    }
}
