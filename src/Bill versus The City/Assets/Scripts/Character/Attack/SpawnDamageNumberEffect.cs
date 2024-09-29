using UnityEngine;

public class SpawnDamageNumberEffect : SpawnPrefabEffect {
    
    public const string DAMAGE_NUMBER_PREFAB = "DamageNumber";

    private bool use_armor_damage = false;
    private Color text_color = new Color(1f, 0.3f, 0.3f);
    private Color outline_color = new Color(0.55f, 0f, 0f);

    public SpawnDamageNumberEffect () : base(DAMAGE_NUMBER_PREFAB) {
        // do nothing. use default values.
    }
    public SpawnDamageNumberEffect (bool use_armor_damage, Color text_color, Color outline_color) : base(DAMAGE_NUMBER_PREFAB) {
        this.use_armor_damage = use_armor_damage;
        this.text_color = text_color;
        this.outline_color = outline_color;
    }

    protected virtual float GetDamageValue(IAttack attack) {
        if (use_armor_damage) {
            return attack.final_armor_damage;
        }
        return attack.final_health_damage;
    }

    protected override void ConfigureDamageEffect(GameObject spawned_prefab, GameObject hit_target,
            Vector3 hit_location, IAttack attack) {
        // hook to extend this class and setup the spawned prefab. By default, do nothing
        float damage = GetDamageValue(attack);
        string damage_value;
        if (this.use_armor_damage && attack.final_armor_damage <= 0f) {
            // don't spawn armor damage numbers for unarmored targets
            damage_value = "";
        } else {
            damage_value = $"{Mathf.Round(damage)}";
        }
        DamageNumberController effect = spawned_prefab.GetComponent<DamageNumberController>();
        effect.damage_value = damage_value;
        effect.text_color = text_color;
        effect.outline_color = outline_color;
    }

}