
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemiesEvent : AbstractInteractionGameEvent {

    public List<AbstractEnemySpawner> enemy_spawners;

    protected override void Effect() {
        SpawnEnemies();
    }
    public List<GameObject> SpawnEnemies() {
        List<GameObject> enemies_spawned = new List<GameObject>();
        foreach (AbstractEnemySpawner spawn in enemy_spawners) {
            enemies_spawned.Add(spawn.SpawnEnemy());
        }
        return enemies_spawned;
    }
}