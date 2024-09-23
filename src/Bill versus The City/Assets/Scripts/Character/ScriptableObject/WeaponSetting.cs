using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Weapon Setting", menuName ="Data/WeaponSetting")]
public class WeaponSetting : ScriptableObject
{
    // Configs for a weapon equivalent to the weapon's action or
    //  firing mode
    public string _name;
    public FiringMode _firing_mode; 
    public float _fire_rate = 0.1f;

    // rate of fire
    public FiringMode firing_mode { 
        get { return _firing_mode; }
    }
    public bool auto_fire { 
        get { return firing_mode == FiringMode.full_auto; }
    }
    public float fire_rate { 
        get { return _fire_rate; }
    }

    public override string ToString() {
        return $"WeaponSetting<{_name}>";
    }
}
