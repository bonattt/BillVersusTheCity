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

    public List<IFirearm> rifles {
        get {
            List<IFirearm> return_value = new List<IFirearm>();
            for (int i = 0; i < _rifles.Count; i++) {
                return_value.Add((IFirearm) _rifles[i]);
            }
            return return_value;
        }
    }
    
    public List<IFirearm> handguns {
        get {
            List<IFirearm> return_value = new List<IFirearm>();
            for (int i = 0; i < _handguns.Count; i++) {
                return_value.Add((IFirearm) _handguns[i]);
            }
            return return_value;
        }
    }
}
