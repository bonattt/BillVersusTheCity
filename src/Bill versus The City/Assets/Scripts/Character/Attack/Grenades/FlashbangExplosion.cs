using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashbangExplosion : AbstractExplosion
{
    // an IExplosion which spawns additional explosions

    public float flashbang_intensity = 10f;

    [SerializeField] private float _explosion_radius;
    public override float explosion_radius => _explosion_radius;
    public override ExplosionAttack explosion_attack => null;
    public override string attack_sound_path => "gunshot_sound";
    public override IEnumerable<GameObject> explosion_effects {
        get {
            if (_explosion_effects == null) {
                InitExplosionEffects();
            }
            return _explosion_effects;
        }
    }
    [SerializeField] private float _explosion_volume;
    public override float explosion_volume => _explosion_volume;

    private static List<GameObject> _explosion_effects = null;

    private void InitExplosionEffects() {
        _explosion_effects = new List<GameObject>();
        GameObject effect_prefab = Resources.Load<GameObject>("FlashBangEffect");
        _explosion_effects.Add(effect_prefab);
    }
    public override void Explode()
    {
        foreach (IAttackTarget target in GetExplosionHits()) {
            Vector3 hit_position = ((MonoBehaviour) target).transform.position;
            float distance = PhysicsUtils.FlatDistance(hit_position, raycast_from.position);
            float hit_intensity = flashbang_intensity * (explosion_radius - distance);
            target.FlashBangHit(transform.position, hit_intensity);
        }
        MakeGameSound();
        SpawnExplosionEffects();
        MakeSoundEffect();
        if (destroy_on_explode) {
            Destroy(gameObject);
        } 
    }
}
