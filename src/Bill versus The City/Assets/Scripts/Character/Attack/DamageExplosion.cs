using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DamageExplosion : AbstractExplosion
{
    public override float explosion_radius { get => explosion_attack.explosion_radius; }
    public override void Explode()
    {
        if (has_exploded) { return; } // only explode once
        has_exploded = true;
        DealExplosionDamage();
        MakeGameSound();
        SpawnExplosionEffects();
        MakeSoundEffect();
        if (destroy_on_explode) {
            Destroy(gameObject);
        } 
    }
}