using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DamageExplosion : AbstractExplosion
{
    [FormerlySerializedAs("explosion_attack")]
    [SerializeField]
    private ExplosionAttack _explosion_attack;
    public override ExplosionAttack explosion_attack => _explosion_attack;
    public override string attack_sound_path => explosion_attack.attack_sound;
    public override IEnumerable<GameObject> explosion_effects => explosion_attack.explosion_effects;
    public override float explosion_radius { get => explosion_attack.explosion_radius; }
    public override float explosion_volume { get => explosion_attack.explosion_volume; }

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