using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Bullet Effect", menuName ="Data/BulletEffect")]
public class BulletEffect : ScriptableObject {
    // configs about the terminal balistics of a weapon, mostly 
    // equivalent to the cartridge used.
    
    public string _name;
    // ammo
    public AmmoType ammo_type;

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
    

    public override string ToString() {
        return $"BulletEffect<{_name}>";
    }
}
