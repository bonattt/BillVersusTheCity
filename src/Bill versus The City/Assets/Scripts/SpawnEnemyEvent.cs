using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnEnemyEvent : AbstractEnemySpawner, IGameEventEffect, IInteractionEffect {

    [Tooltip("if true, the spawner will spawn enemies at all spawn points on the first tick. Otherwise, countdowns will be started from the first tick")]
    public bool spawn_on_start = true;
    public bool effect_completed { get; private set; }

    protected override void Start() {
        base.Start();
        if (spawn_on_start) {
            SpawnEnemy();
        }
        effect_completed = false;
    }

    public void ActivateEffect() {
        SpawnEnemy();
        effect_completed = true;
    }

    public void Interact(GameObject actor) {
        SpawnEnemy();
    }

    public override GameObject SpawnEnemy() {
        GameObject spawned = base.SpawnEnemy();
        return spawned;
    }

    //////////////////////////////////////
    ///// implement IGameEventEffect /////
    //////////////////////////////////////

}
