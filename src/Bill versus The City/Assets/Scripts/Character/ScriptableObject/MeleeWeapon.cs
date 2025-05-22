


using UnityEngine;

[CreateAssetMenu(fileName ="New Melee Weapon", menuName ="Data/MeleeWeapon")]
public class MeleeWeapon : ScriptableObject, IMeleeWeapon 
{
    
    public string _item_id;
    public string item_id { get => _item_id; }

    public string _item_name;
    public string item_name { get => _item_name; }

    public Sprite _item_icon;
    public Sprite item_icon { get => _item_icon; }

    public float _weapon_damage_min;
    public float weapon_damage_min { get => _weapon_damage_min; }

    public float _weapon_damage_max;
    public float weapon_damage_max { get => _weapon_damage_max; }

    public float _armor_effectiveness;
    public float armor_effectiveness { get => _armor_effectiveness; }

    public string _attack_sound;
    public string attack_sound { get => _attack_sound; }


    public float _attack_reach;
    public float attack_reach { get => _attack_reach; }

    public float _attack_windup;
    public float attack_windup { get => _attack_windup; }

    public float _attack_duration;
    public float attack_duration { get => _attack_duration; }

    public float _attack_recovery;
    public float attack_recovery { get => _attack_recovery; }

    public float _attack_cooldown;
    public float attack_cooldown { get => _attack_cooldown; }

    public IWeapon CopyWeapon()
    {
        return Instantiate(this);
    }
}