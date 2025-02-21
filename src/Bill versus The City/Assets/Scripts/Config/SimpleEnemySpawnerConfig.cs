using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Simple Enemy Spawn Config", menuName ="Data/SimpleEnemySpawner")]
public class SimpleEnemySpawnerConfig : ScriptableObject, IEnemySpawnConfig
{
    public float rare_weapon_chance = 0.1f;
    public float armor_chance = 0f;

    public GameObject enemy_prefab;
    public ArmorPlate armor_prefab;
    public List<DetailedWeapon> rare_weapons;

    public GameObject GetPrefab() {
        return enemy_prefab; // null tells the spawner to use default setting
    }
    public IWeapon GetWeapon() {
        if (Random.Range(0f, 1f) < rare_weapon_chance) {
            IWeapon weapon = GetRandomWeapon();
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

    public IWeapon GetRandomWeapon() {
        int i = (int) Random.Range(0, rare_weapons.Count);
        return rare_weapons[i];
    }
}
