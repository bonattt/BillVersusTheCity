using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IEnemySpawnConfig {
    public GameObject GetPrefab() {
        return null; // null tells the spawner to use default setting
    }
    public IWeapon GetWeapon() {
        return null; // null tells the spawner to use default setting
    }
    public IArmor GetArmor() {
        return null;  // null tells the spawner to use default setting
    }
}

public abstract class AbstractEnemySpawner : MonoBehaviour
{
    // base class for spawning enemies, which implements all spawner logic, but never triggers a spawn.
    // sub-classes must simply invoke SpawnEnemy under whatever conditions the sub-class wants to implement
    public GameObject enemy_prefab;
    public Transform spawn_location;
    public ScriptableObject init_spawner_config;
    private IEnemySpawnConfig spawner_config;

    public bool override_default_behavior = false;
    public BehaviorMode default_behavior = BehaviorMode.searching;
    public Vector3 initial_move_target = new Vector3(float.NaN, float.NaN, float.NaN);
    
    void Start()
    {
        InitializeSpawner();
    }

    protected virtual void InitializeSpawner() {
        if (spawn_location == null) { spawn_location = transform; }
        spawner_config = (IEnemySpawnConfig) init_spawner_config;
    }

    public void SpawnEnemy() {
        GameObject prefab = GetPrefab();
        GameObject enemy = Instantiate(prefab);

        enemy.transform.position = spawn_location.position;

        EnemyController enemy_ctrl = enemy.GetComponent<EnemyController>();
        IWeapon weapon = GetWeapon();
        if (weapon != null) {
            enemy_ctrl.current_weapon = weapon;
        }
        
        IArmor armor = GetArmor();
        if (armor != null) {
            enemy_ctrl.GetStatus().ApplyNewArmor(armor);
        }

        ConfigureEnemyBehavior(enemy_ctrl.GetComponent<EnemyBehavior>());
        // TODO --- apply behaviors
        string spawned_at_str = transform.parent == null ? "" : $"at {transform.parent.gameObject.name}";
        Debug.Log($"Spawn new enemy {enemy_ctrl.gameObject.name} spawned by {this.gameObject.name} {spawned_at_str} (spawn time {Time.time})");
    }

    public virtual void ConfigureEnemyBehavior(EnemyBehavior spawned_enemy) {
        // configures the behavior of a newly spawned enemy
        if (override_default_behavior) {
            spawned_enemy.default_behavior = default_behavior;
        }

        spawned_enemy.initial_movement_target = initial_move_target;
        if (EnemyBehavior.ValidVectorTarget(initial_move_target)) {
            spawned_enemy.use_initial_movement_target = true;
        } else {
            spawned_enemy.use_initial_movement_target = false;
            
        }
    }
    
    private void ConfigureSearchBehavior(EnemyBehavior spawned_enemy) {
        SearchingBehavior search_behavior;
    }

    public GameObject GetPrefab() {
        GameObject result = null;
        if (spawner_config != null)  {
            result = spawner_config.GetPrefab();
        }
        if (result == null) {
            result = enemy_prefab;
        }
        return result;
    }

    public IWeapon GetWeapon() {
        return spawner_config != null ? spawner_config.GetWeapon() : null;
    }

    public IArmor GetArmor() {
        return spawner_config != null ? spawner_config.GetArmor() : null;
    }
}
