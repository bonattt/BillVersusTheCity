using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Bullet Attributes", menuName ="Data/BulletAttributes")]
public class BulletAttributes : ScriptableObject {
    // configs about the terminal balistics of a weapon, mostly 
    // equivalent to the cartridge used.
    
    public string _name;
    // ammo
    public AmmoType ammo_type;
    public int ammo_drop_size = 5;

    // rate of fire
    public int n_shots = 1;

    // accuracy
    public float aimed_inaccuracy = 0f;
    public float initial_inaccuracy = 2f; 

    // attack
    public float bullet_speed = 35f;
    public float weapon_damage_min = 30f;
    public float weapon_damage_max = 50f;
    public float armor_effectiveness = 1f;
    public float damage_falloff_rate = 0.25f;
    public GameObject bullet_prefab;
    

    public override string ToString() {
        return $"BulletEffect<{_name}>";
    }
}
