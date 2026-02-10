

using UnityEngine;

public class AreaDamageAttack : IAttack
{    public IAttackTarget attacker => null;
    public IWeapon weapon => null;
    public float damage_falloff_rate => 0;
    public DecayFunction damage_falloff_function => DecayFunction.none;
    public Vector3 attack_from { get; set; } // where distance to target should be calculated from

    public float attack_damage => area_damage.damage_rate * area_damage.damage_period_seconds;
    public float attack_damage_min => attack_damage;
    public float attack_damage_max => attack_damage;
    public float armor_effectiveness => 1;
    public bool ignore_armor { get; private set; }
    public float final_health_damage { get; set; }
    public float final_armor_damage { get; set; }

    private AreaDamageRegion area_damage;
    public AreaDamageAttack(AreaDamageRegion area_damage)
    {
        this.area_damage = area_damage;

        this.ignore_armor = false;
        attack_from = area_damage.transform.position;
    }
}