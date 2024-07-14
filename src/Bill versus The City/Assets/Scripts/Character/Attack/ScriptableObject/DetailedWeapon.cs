using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Detailed Weapon", menuName ="Data/DetailedWeapon")]
public class DetailedWeapon : ScriptableObject, IWeapon
{
    // ammo
    public AmmoType _ammo_type;
    public int _ammo_capacity;
    public int _reload_amount;
    public float _reload_time;

    // rate of fire
    public FiringMode _firing_mode; 
    public float _semi_auto_fire_rate;
    public float _full_auto_fire_rate;

    // accuracy
    public float _aimed_inaccuracy;
    public float _initial_inaccuracy;
    public float _aim_speed;
    public float _recoil_inaccuracy;
    public float _recoil_shake;

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

    // rate of fire
    public FiringMode firing_mode { 
        get { return _firing_mode; }
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
}
