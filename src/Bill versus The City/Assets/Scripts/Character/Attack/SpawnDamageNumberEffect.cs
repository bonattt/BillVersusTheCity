using UnityEngine;

public class SpawnDamageNumberEffect : SpawnPrefabEffect {
    
    public const string DAMAGE_NUMBER_PREFAB = "DamageNumber";

    public SpawnDamageNumberEffect () : base(DAMAGE_NUMBER_PREFAB) {
        // do nothing
    }

    protected override void ConfigureDamageEffect(GameObject spawned_prefab, GameObject hit_target,
            Vector3 hit_location, float damage) {
        // hook to extend this class and setup the spawned prefab. By default, do nothing
        DamageNumberEffect effect = spawned_prefab.GetComponent<DamageNumberEffect>();
        effect.damage_amount = Mathf.Round(damage);
        Debug.Log($"spawn damage number {damage}");
    }

}