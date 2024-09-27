using UnityEngine;

public class SpawnDamageNumberEffect : SpawnPrefabEffect {
    
    public const string DAMAGE_NUMBER_PREFAB = "DamageNumber";

    public SpawnDamageNumberEffect () : base(DAMAGE_NUMBER_PREFAB) {
        // do nothing
    }

    protected override void ConfigureDamageEffect(GameObject spawned_prefab, GameObject hit_target,
            Vector3 hit_location, float damage) {
        // hook to extend this class and setup the spawned prefab. By default, do nothing
        DamageNumberController effect = spawned_prefab.GetComponent<DamageNumberController>();
        effect.damage_amount = Mathf.Round(damage);
    }

}