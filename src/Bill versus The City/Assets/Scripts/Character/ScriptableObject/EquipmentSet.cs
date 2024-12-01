using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Equipment Set", menuName ="Data/Equipment Set")]
public class EquipmentSet : ScriptableObject
{
    // Scriptable Ojbect that stores lists of categorized weapons. Used for configuring what weapons the Player has access to
    public string config_name;
    
    [SerializeField]
    // scriptable object list so items can be assigned in the inspector
    private List<ScriptableObject> _rifles, _handguns;

    public List<IWeapon> rifles {
        get {
            List<IWeapon> return_value = new List<IWeapon>();
            for (int i = 0; i < _rifles.Count; i++) {
                return_value.Add((IWeapon) _rifles[i]);
            }
            return return_value;
        }
    }
    
    public List<IWeapon> handguns {
        get {
            List<IWeapon> return_value = new List<IWeapon>();
            for (int i = 0; i < _handguns.Count; i++) {
                return_value.Add((IWeapon) _handguns[i]);
            }
            return return_value;
        }
    }
}
