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
            Vector3 hit_location, float damage) {
        SpawnPrefab(hit_location);
    }

    public void DisplayEffect(Vector3 hit_location) {
        SpawnPrefab(hit_location);
    }

    private void SpawnPrefab(Vector3 point) {
        if (prefab == null) {
            prefab = (GameObject) Resources.Load(prefab_path);
        }
        GameObject effect = GameObject.Instantiate<GameObject>(prefab);
        effect.transform.position = point;
    }

}