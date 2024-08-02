using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Armor", menuName ="Data/Armor")]
public class ArmorPlate : ScriptableObject, IArmor
{
    
    public float _armor_max_durability;
    public float _armor_hardness;
    public float _armor_protection;
    
    public float armor_max_durability { 
        get {
            return _armor_max_durability;
        }
    }

    // reduces overall damage
    public float armor_hardness { 
        get {
            return _armor_hardness;
        }
    }
    
    // percentage of damage dealt to the wearer of the armor
    public float armor_protection { 
        get {
            return _armor_protection;
        }
    }

    // current HP of the armor
    private float? _armor_durability = null; 
    public float armor_durability { 
        get {
            if (_armor_durability == null) {
                _armor_durability = armor_max_durability;
            }
            return (float) armor_durability;
        }
        set {
            if (value > armor_max_durability) {
                _armor_durability = armor_max_durability;
            }
            else if (value < 0) {
                _armor_durability = 0f;
            }
        }
    }

}