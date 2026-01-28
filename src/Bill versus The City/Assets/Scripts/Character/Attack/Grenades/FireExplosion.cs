using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExplosion : AbstractExplosion
{
    // an IExplosion which spawns a damaging area
    [Tooltip("Prefab grenade spawned when this explosion explodes.")]
    public GameObject flame_prefab;


    public override ExplosionAttack explosion_attack => null;
    public override string attack_sound_path => "gunshot_sound";
    public override IEnumerable<GameObject> explosion_effects => new List<GameObject>();

    public float damage_per_second = 50f;
    public float burn_duration_seconds = 8f;

    [SerializeField] private float flame_radius;
    public override float explosion_radius => flame_radius;
    
    [SerializeField] private float _explosion_volume = 1f;
    public override float explosion_volume => _explosion_volume;
    public override void Explode()
    {
        SpawnFlame();
        SpawnExplosionEffects();
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


    protected GameObject SpawnFlame() {
        Debug.LogWarning("SpawnFlame START");
        GameObject flame_spawner = new GameObject();
        flame_spawner.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        AreaEffectSpawner spawner_script = flame_spawner.AddComponent<AreaEffectSpawner>();
        spawner_script.area_radius = flame_radius;
        spawner_script.area_effect_duration = burn_duration_seconds;
        spawner_script.spawn_on_start = false;
        spawner_script.effect_prefab = flame_prefab;
        spawner_script.blocks_propegation = blocks_explosion;
        spawner_script.SpawnAsRoot();


        // GameObject flame_area = Instantiate(flame_prefab);
        // flame_area.transform.position = transform.position;
        // flame_area.transform.localScale = new Vector3(explosion_radius, 1, explosion_radius);

        // AreaDamage damage = flame_area.GetComponentInChildren<AreaDamage>();
        // damage.damage_rate = damage_per_second;

        // ParticlesSoftKillTimer kill_timer = flame_area.AddComponent<ParticlesSoftKillTimer>();
        // kill_timer.duration = burn_duration_seconds;
        // ParticleSystem[] all_particles = flame_area.GetComponentsInChildren<ParticleSystem>();
        // if (all_particles != null) {
        //     kill_timer.AddParticleSystems(all_particles);
        // }
        
        Debug.LogWarning("SpawnFlame END");
        return flame_spawner;
    }
}