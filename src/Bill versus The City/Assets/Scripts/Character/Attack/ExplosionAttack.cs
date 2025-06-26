

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Explosion", menuName = "Data/Explosion")]
public class ExplosionAttack : ScriptableObject, IAttack, IWeapon {

    public float explosion_volume = 10f;
    public float explosion_radius;
    public List<GameObject> explosion_effects;

    // implements IAttack
    public IAttackTarget attacker { get; set; }
    public IWeapon weapon { get => this; }
    public float _damage_falloff;
    public float damage_falloff { get => _damage_falloff; }
    public float _attack_damage_min;
    public float attack_damage_min { get => _attack_damage_min; }
    public float _attack_damage_max;
    public float attack_damage_max { get => _attack_damage_max; }
    public float _armor_effectiveness;
    public float armor_effectiveness { get => _armor_effectiveness; }
    public bool _ignore_armor;
    public bool ignore_armor { get => _ignore_armor; }
    public float final_health_damage { get; set; }
    public float final_armor_damage { get; set; }


    // implement IWeapon
    public string _item_id = "explosion";
    public string item_id { get => _item_id; }
    public string _item_name = "explosion";
    public string item_name { get => _item_name; }
    public Sprite _item_icon;
    public Sprite item_icon { get => _item_icon; }
    public float weapon_damage_min { get => attack_damage_min; }
    public float weapon_damage_max { get => attack_damage_max; }
    public string _attack_sound;
    public string attack_sound { get => _attack_sound; }

    
    public void AttackClicked(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker) {
        throw new NotImplementedException("AttackClicked is not implemented for explosion attack!");
    }
    public void AttackHold(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker) {
        throw new NotImplementedException("AttackHold is not implemented for explosion attack!");
    }
    public void AttackReleased(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker) {
        // do nothing
    }
    public IWeapon CopyWeapon() {
        return Instantiate(this);
    }

}