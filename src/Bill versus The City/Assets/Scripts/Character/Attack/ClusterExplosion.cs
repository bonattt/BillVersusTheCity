using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterExplosion : AbstractExplosion
{
    // an IExplosion which spawns additional explosions

    [Tooltip("Prefab grenade spawned when this explosion explodes.")]
    public GameObject explosive_prefab;

    [Tooltip("Number of secondary explosives spawned by the grenade")]
    public int n_explosions = 12;
    
    [Tooltip("Initial velocity of spawned secondary explosives.")]
    public float spawn_force_min = 0.8f;
    public float spawn_force_max = 1.2f;
    private float spawn_start_offset = 0.1f; // starts the spawned grenade this far from the origin already

    [Tooltip("Fuse duration on spawned secondary explosives.")]
    public float secondary_fuse_seconds_min = 0.9f;
    public float secondary_fuse_seconds_max = 1.1f;

    public override ExplosionAttack explosion_attack => null;
    public override string attack_sound_path => "gunshot_sound";
    public override IEnumerable<GameObject> explosion_effects => new List<GameObject>();

    [SerializeField] private float _explosion_radius;
    public override float explosion_radius => _explosion_radius;
    
    [SerializeField] private float _explosion_volume;
    public override float explosion_volume => _explosion_volume;
    public override void Explode()
    {
        foreach (Vector3 direction in GetDirections(n_explosions)) {
            SpawnSecondaryExplosive(transform.position, direction);
        }
        if (destroy_on_explode) {
            Destroy(gameObject);
        }
    }

    public static IEnumerable<Vector3> GetDirections(int n) {
        // iterable method to return vectors pointing in N directions
        for (int k = 0; k < n; k++) {
            yield return GetDirection(n, k);
        }
    }


    private static Vector3 GetDirection(int n, int k) {
        // n: length of sequence
        // k: sequence index
        float angle = 2 * Mathf.PI * k / n;
        return new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
    }


    protected GameObject SpawnSecondaryExplosive(Vector3 origin, Vector3 direction) {
        GameObject secondary = Instantiate(explosive_prefab);
        GrenadeFuse fuse = secondary.GetComponent<GrenadeFuse>();
        if (fuse == null) {
            Debug.LogError($"cannot set fuse length on secondary explosive {secondary.name} with no fuse!");
        } else {
            fuse.fuse_length_seconds = Random.Range(secondary_fuse_seconds_min, secondary_fuse_seconds_max);
            fuse.start_fuse_on_start = true;
        }
        direction = direction.normalized;
        secondary.transform.position = origin + (direction * spawn_start_offset);
        Rigidbody rb = secondary.GetComponent<Rigidbody>();
        if (rb == null) {
            Debug.LogError($"cannot configure Rigidboyd on {secondary.name}, component is missing!"); 
        } else {
            float magnitude = Random.Range(spawn_force_min, spawn_force_max);
            rb.AddForce(direction * magnitude, ForceMode.VelocityChange);
        }
        GrenadeFuseUI fuse_ui = secondary.GetComponentInChildren<GrenadeFuseUI>();
        if (fuse_ui == null) {
            Debug.LogError($"cannot set fuse UI for {secondary.name}, GrenadeFuseUI component is missing!");
        } else {
            fuse_ui.SetVisibility(false); // never show fuse UI for cluster bomb secondaries.
        }
        return secondary;
    }
}