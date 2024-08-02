using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Detailed Weapon", menuName ="Data/DetailedWeapon")]
public class DetailedWeapon : ScriptableObject, IWeapon
{
    public string _name;
    // ammo
    public AmmoType _ammo_type;
    public int _ammo_capacity = 30;
    public int _reload_amount = 30;
    public float _reload_time ;

    // rate of fire
    public FiringMode _firing_mode; 
    public float _semi_auto_fire_rate = 0.25f;
    public float _full_auto_fire_rate = 0.1f;

    // accuracy
    public float _aimed_inaccuracy = 0f;
    public float _initial_inaccuracy = 2f; 
    public float _aim_speed = 1f;
    public float _recoil_inaccuracy = 0.5f;
    public float _recoil_shake = 0.1f;

    // attack
    public float _bullet_speed = 35f;
    public float _weapon_damage_min = 30f;
    public float _weapon_damage_max = 50f;
    public float _armor_penetration = 0f;

    public string weapon_name {
        get { return _name; }
    }
    public AmmoType ammo_type { 
        get { return _ammo_type; } 
    }
    public int ammo_capacity { 
        get { return _ammo_capacity; } 
    }
    public int reload_amount { 
        get { return _reload_amount; } 
    }
    public float reload_time { 
        get { return _reload_time; } 
    }
    public int current_ammo { get; set; }

    // rate of fire
    public FiringMode firing_mode { 
        get { return _firing_mode; }
    }
    public bool auto_fire { 
        get { return firing_mode == FiringMode.full_auto; }
    }
    public float semi_auto_fire_rate { 
        get { return _semi_auto_fire_rate; }
    }
    public float full_auto_fire_rate { 
        get { return _full_auto_fire_rate; }
    }

    // accuracy
    public float aimed_inaccuracy { 
        get { return _aimed_inaccuracy; }
    }
    public float initial_inaccuracy { 
        get { return _initial_inaccuracy; }
    }
    public float aim_speed { 
        get { return _aim_speed; }
    }
    public float recoil_inaccuracy { 
        get { return _recoil_inaccuracy; }
    }
    public float recoil_shake { 
        get { return _recoil_shake; }
    }
    
    
    public float bullet_speed { 
        get { return _bullet_speed; }
    }
    public float weapon_damage_min { 
        get { return _weapon_damage_min; }
    }
    public float weapon_damage_max { 
        get { return _weapon_damage_max; }
    }
    public float armor_penetration { 
        get { return _armor_penetration; }
    }
    public float _armor_damage;
    public float armor_damage { 
        get { return _armor_damage; }
    }

    public override string ToString() {
        return $"DetailedWeapon<{_name}>";
    }
}
