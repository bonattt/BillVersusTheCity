using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TriggeredEnemySpawner : AbstractEnemySpawner, IGameEventEffect, IInteractionEffect {

    [Tooltip("if true, the spawner will spawn enemies at all spawn points on the first tick. Otherwise, countdowns will be started from the first tick")]
    public bool spawn_on_start = true;

    protected override void Start() {
        base.Start();
        if (spawn_on_start) {
            SpawnEnemy(); 
        }
    }

    public void ActivateEffect() {
        SpawnEnemy();
    }

    public void Interact(GameObject actor) {
        SpawnEnemy();
    }

}
