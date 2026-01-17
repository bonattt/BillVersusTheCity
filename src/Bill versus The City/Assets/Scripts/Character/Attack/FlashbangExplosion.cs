using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashbangExplosion : AbstractExplosion
{
    // an IExplosion which spawns additional explosions

    public float flashbang_intensity = 10f;

    [SerializeField]
    private float _explosion_radius;
    public override float explosion_radius => _explosion_radius;
    public override void Explode()
    {
        foreach (IAttackTarget target in GetExplosionHits()) {
            Vector3 hit_position = ((MonoBehaviour) target).transform.position;
            float distance = PhysicsUtils.FlatDistance(hit_position, raycast_from.position);
            float hit_intensity = flashbang_intensity * (explosion_radius - distance);
            target.FlashBangHit(hit_intensity);
        }
        MakeGameSound();
        SpawnExplosionEffects();
        MakeSoundEffect();
        if (destroy_on_explode) {
            Destroy(gameObject);
        } 
    }
}
