
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemiesEvent : MonoBehaviour, IGameEventEffect, IInteractionEffect {

    public List<AbstractEnemySpawner> enemy_spawners;
    public void Interact(GameObject actor) {
        SpawnEnemies();
    }

    public void ActivateEffect() {
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