using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Simple Enemy Spawn Config", menuName = "Data/SimpleEnemySpawner")]
public class SimpleEnemySpawnerConfig : ScriptableObject, IEnemySpawnConfig {

    public EnemyDrop drop_config = EnemyDrop.default_drop;
    public float rare_weapon_chance = 0.1f;
    public float armor_chance = 0f;

    public GameObject enemy_prefab;
    public ArmorPlate armor_prefab;
    public List<DetailedWeapon> rare_weapons;

    public bool? drop_ammo {
        get {
            if (drop_config == EnemyDrop.none) {
                return false;
            }
            else if (drop_config == EnemyDrop.default_drop) {
                return null;
            }
            else if (drop_config == EnemyDrop.ammo) {
                return true;
            }
            return false;
        }
    }

    public bool? drop_weapon {
        get {
            if (drop_config == EnemyDrop.none) {
                return false;
            }
            else if (drop_config == EnemyDrop.default_drop) {
                return null;
            }
            else if (drop_config == EnemyDrop.weapon) {
                return true;
            }
            return false;
        }
    }

    public GameObject GetPrefab() {
        return enemy_prefab; // null tells the spawner to use default setting
    }
    public IFirearm GetWeapon() {
        if (Random.Range(0f, 1f) < rare_weapon_chance) {
            IFirearm weapon = GetRandomWeapon();
            return weapon;
        }
        return null; // null tells the spawner to use default setting
    }
    public IArmor GetArmor() {
        if (Random.Range(0f, 1f) < armor_chance) {
            return armor_prefab;
        }
        return null;  // null tells the spawner to use default setting
    }

    public IFirearm GetRandomWeapon() {
        int i = (int)Random.Range(0, rare_weapons.Count);
        return rare_weapons[i];
    }
}


public enum EnemyDrop {
    default_drop, // leaves the prefab's drop settings unchanged
    none,
    ammo,
    weapon
}
