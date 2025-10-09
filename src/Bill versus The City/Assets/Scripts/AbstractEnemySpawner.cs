using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IEnemySpawnConfig {
    public GameObject GetPrefab() {
        return null; // null tells the spawner to use default setting
    }
    public IFirearm GetWeapon() {
        return null; // null tells the spawner to use default setting
    }
    public IArmor GetArmor() {
        return null;  // null tells the spawner to use default setting
    }
}

public interface ISpawnPoint {
    public Vector3 GetSpawnPosition();
}

public abstract class AbstractEnemySpawner : MonoBehaviour, ISpawnPoint
{
    // base class for spawning enemies, which implements all spawner logic, but never triggers a spawn.
    // sub-classes must simply invoke SpawnEnemy under whatever conditions the sub-class wants to implement
    public GameObject enemy_prefab;
    public Transform spawn_location;
    public ScriptableObject init_spawner_config;
    private IEnemySpawnConfig spawner_config;

    public bool override_default_behavior = false;
    public BehaviorMode default_behavior = BehaviorMode.searching;
    public Transform initial_move_target;

    public List<MonoBehaviour> init_spawn_points;
    protected List<ISpawnPoint> spawn_points = new List<ISpawnPoint>();

    public string base_enemy_name = "Enemy";
    private int spawns_count = 0;
    
    protected virtual void Start()
    {
        InitializeSpawner();
    }

    public string GetNextEnemyName() {
        // 
        spawns_count += 1;
        return $"{base_enemy_name} ({spawns_count})";
    }

    protected virtual void InitializeSpawnPoints() {
        spawn_points = new List<ISpawnPoint>();
        if (init_spawn_points == null || init_spawn_points.Count == 0) {
            spawn_points.Add(this);
            return;
        }
        foreach(MonoBehaviour b in init_spawn_points) {
            try {
                ISpawnPoint p = (ISpawnPoint) b;
                spawn_points.Add(p);
            } catch (InvalidCastException) {
                Debug.LogWarning($"behavior {b} cannot be cast to ISpawnPoint!");
            }
        }
    }

    protected virtual void InitializeSpawner() {
        if (spawn_location == null) { spawn_location = transform; }
        spawner_config = (IEnemySpawnConfig) init_spawner_config;
        InitializeSpawnPoints();
    }

    public virtual GameObject SpawnEnemy() {
        GameObject prefab = GetPrefab();
        GameObject enemy = Instantiate(prefab);
        enemy.name = GetNextEnemyName();

        NavMeshAgentMovement enemy_ctrl = enemy.GetComponent<NavMeshAgentMovement>();
        enemy_ctrl.TeleportTo(GetSpawnPoint().GetSpawnPosition());
        IFirearm weapon = GetWeapon();
        if (weapon != null) {
            enemy_ctrl.current_firearm = weapon;
        }

        IArmor armor = GetArmor();
        if (armor != null) {
            enemy_ctrl.GetStatus().ApplyNewArmor(armor);
        }

        ConfigureEnemyBehavior(enemy_ctrl.GetComponent<EnemyBehavior>());
        // TODO --- apply behaviors // --- I think this is handled?
        LogSpawn(enemy_ctrl);
        return enemy;
    }

    public bool log_spawns = false;
    private void LogSpawn(NavMeshAgentMovement enemy_ctrl) {
        // logs that a new enemy was spawned
        if (log_spawns) {
            string spawned_at_str = transform.parent == null ? "" : $"at {transform.parent.gameObject.name}";
        }

    }

    public virtual void ConfigureEnemyBehavior(EnemyBehavior spawned_enemy) {
        // configures the behavior of a newly spawned enemy
        if (override_default_behavior) {
            spawned_enemy.default_behavior = default_behavior;
        }
        Vector3 initial_move_position;
        if (initial_move_target != null) {
            initial_move_position = initial_move_target.position;
        } else {
            initial_move_position = new Vector3(float.NaN, float.NaN, float.NaN);
        }
        spawned_enemy.initial_movement_target = initial_move_position;
        if (EnemyBehavior.ValidVectorTarget(initial_move_position)) {
            spawned_enemy.use_initial_movement_target = true;
        } else {
            spawned_enemy.use_initial_movement_target = false;
        }
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

    public virtual ISpawnPoint GetSpawnPoint() {
        return this;
    }
    
    public virtual Vector3 GetSpawnPosition() {
        // implements ISpawnPoint, if no spawn points are provided, spawner spawns at it's own location
        return this.transform.position;
    }

    public IFirearm GetWeapon() {
        return spawner_config != null ? spawner_config.GetWeapon() : null;
    }

    public IArmor GetArmor() {
        return spawner_config != null ? spawner_config.GetArmor() : null;
    }
}
