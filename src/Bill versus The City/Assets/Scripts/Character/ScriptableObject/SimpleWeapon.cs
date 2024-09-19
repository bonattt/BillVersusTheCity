using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Simple Weapon", menuName ="Data/SimpleWeapon")]
public class SimpleWeapon : ScriptableObject, IWeapon
{
    public string _name;
    // ammo
    public AmmoType _ammo_type;
    public int _ammo_capacity = 17;
    // public int _reload_amount;
    public float _reload_time = 1.5f;

    // rate of fire
    public FiringMode _firing_mode; 
    public float _fire_rate = 0.25f;
    private int _n_shots = 1;

    // accuracy
    private float _aimed_inaccuracy = 1f;
    private float _initial_inaccuracy = 8f; 
    private float _time_to_aim = 1f;
    private float _recoil_inaccuracy = 10f;
    private float _recoil_max = 12f;
    private float _recoil_recovery = 30f;
    private float _recoil_shake = 0f;
    private float _aim_zoom = 0.8f;
    private float _aim_move_speed = 0.5f;
    private float _max_zoom_range = 5f;

    private float _bullet_speed = 35f;
    public float weapon_damage = 40f;
    private float _armor_penetration = 0f;

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
        get { return _ammo_capacity; } 
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
        get { return _fire_rate; }
    }
    public float full_auto_fire_rate { 
        get { return _fire_rate; }
    }
    public int n_shots {
        get { return _n_shots; }
    }

    // accuracy
    public float aimed_inaccuracy { 
        get { return _aimed_inaccuracy; }
    }
    public float initial_inaccuracy { 
        get { return _initial_inaccuracy; }
    }
    public float time_to_aim { 
        get { return _time_to_aim; }
    }
    public float recoil_inaccuracy { 
        get { return _recoil_inaccuracy; }
    }
    public float recoil_max { 
        get { return _recoil_max; }
    }
    public float recoil_recovery { 
        get { return _recoil_recovery; }
    }
    public float recoil_shake { 
        get { return _recoil_shake; }
    }
    // aiming
    public float aim_zoom {
        get { return _aim_zoom; }
    }
    public float aim_move_speed { 
        get { return _aim_move_speed; }
    }
    public float max_zoom_range { 
        get { return _max_zoom_range; }
    }
    
    public float bullet_speed { 
        get { return _bullet_speed; }
    }
    public float weapon_damage_min { 
        get { return weapon_damage - 5; }
    }
    public float weapon_damage_max { 
        get { return weapon_damage + 5; }
    }
    public float armor_penetration { 
        get { return _armor_penetration; }
    }

    public override string ToString() {
        return $"SimpleWeapon<{_name}>";
    }
}
