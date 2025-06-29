


using System;
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

    
    public void AttackClicked(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker) {
        FireAttack(attack_direction, attack_start_point, inaccuracy, attacker);
    }
    public void AttackHold(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker) {
        FireAttack(attack_direction, attack_start_point, inaccuracy, attacker);
    }

    public void AttackReleased(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker) {
        // do nothing
    }

    private MeleeAttack current_attack = null;
    public void FireAttack(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker) {
        // Debug.LogError($"MeleeWeapon.FireAttack");
        // if (current_attack == null) current_attack = new MeleeAttack();
        // // else Debug.LogError("attack made while current attack isn't nulled yet!");
        // current_attack.melee_weapon = this;
        // current_attack.ignore_armor = false;
        // current_attack.hit_targets.Add(attacker); // prevent from hitting self with melee attack

        // AttackResolver.AttackStart(current_attack, attack_direction, attack_start_point, is_melee_attack: true);
    }

    public IWeapon CopyWeapon() => CopyMeleeWeapon();
    public IMeleeWeapon CopyMeleeWeapon() {
        return Instantiate(this);
    }
}