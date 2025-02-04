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

        // TODO --- apply behaviors
        string spawned_at_str = transform.parent == null ? "" : $"at {transform.parent.gameObject.name}";
        Debug.Log($"Spawn new enemy {enemy_ctrl.gameObject.name} spawned by {this.gameObject.name} {spawned_at_str} (spawn time {Time.time})");
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
