using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Armor", menuName ="Data/Armor")]
public class ArmorPlate : ScriptableObject, IArmor
{
    public string _item_name;
    public Sprite _item_icon;
    public float _armor_max_durability;
    public float _armor_hardness;
    public float _armor_protection;
    
    public string item_name { get { return _item_name; } }
    public Sprite item_icon { get { return _item_icon; } }

    private float _difficulty_multiplier = 1f;
    public float difficulty_multiplier { 
        get { return _difficulty_multiplier; }
    }
    
    public void SetDiffcultyMultiplier(float new_multiplier) {
        // sets the difficulty multipler, and updates affected fields, removing the old multiplier and adding the new one
        _armor_max_durability = _armor_max_durability * new_multiplier / difficulty_multiplier;
        _armor_durability = _armor_durability * new_multiplier / difficulty_multiplier;
        _difficulty_multiplier = new_multiplier;
    }
    
    public void SettingsUpdated(ISettingsModule updated, string field) {
        // right now, `CharacterStatus` is subscribing to the settings 
        // so UI updates can be guaranteed to go out AFTER changes are applied
        throw new InvalidOperationException("not implemented");
    }

    public float armor_max_durability { 
        get { return _armor_max_durability; }
    }

    // reduces overall damage
    public float armor_hardness { 
        get { return _armor_hardness; }
    }
    
    // percentage of damage dealt to the wearer of the armor
    public float armor_protection { 
        get { return _armor_protection; }
    }

    // current HP of the armor
    private float? _armor_durability = null; 
    public float armor_durability { 
        get {
            if (_armor_durability == null) {
                // initialize nullable float armor durability
                _armor_durability = armor_max_durability;
            }
            return (float) _armor_durability;
        }
        set {
            if (value > armor_max_durability) {
                _armor_durability = armor_max_durability;
            }
            else if (value < 0) {
                _armor_durability = 0f;
            }
            else {
                _armor_durability = value;
            }
        }
    }

}