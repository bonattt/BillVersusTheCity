using UnityEngine;

public class SpawnPrefabEffect : IAttackHitEffect, 
        IAttackShootEffect, IAttackMissEffect {
    
    public GameObject prefab;
    public string prefab_path;
    public Vector3 offset = new Vector3(0f, 0f, 0f);

    public SpawnPrefabEffect(string prefab_path) {
        this.prefab_path = prefab_path; 
    }
    
    public void DisplayDamageEffect(GameObject hit_target,
            Vector3 hit_location, IAttack attack) {
        GameObject spawned_prefab = SpawnPrefab(hit_location);
        ConfigureDamageEffect(spawned_prefab, hit_target, hit_location, attack.final_damage);
    }

    public void DisplayEffect(Vector3 hit_location, IAttack attack) {
        SpawnPrefab(hit_location);
    }

    private GameObject SpawnPrefab(Vector3 point) {
        if (prefab == null) {
            prefab = (GameObject) Resources.Load(prefab_path);
        }
        GameObject effect = GameObject.Instantiate<GameObject>(prefab);
        effect.transform.position = point;
        return effect;
    }

    protected virtual void ConfigureDamageEffect(GameObject spawned_prefab, GameObject hit_target,
            Vector3 hit_location, float damage) {
        // hook to extend this class and setup the spawned prefab for a damage effect. By default, do nothing
    }

    protected virtual void ConfigureEffect(GameObject spawned_prefab) {
        // hook to extend this class and setup the spawned prefab location effect. By default, do nothing
    }

}