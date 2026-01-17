using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Bullet {
    public DamageExplosion explosion;

    protected override void Start() {
        base.Start();
        gameObject.name = $"rocket {bullet_count}"; // bullet count incremented in `base.Start()`
    }

    // protected override void ResolveAttackHit(GameObject hit, Vector3 hit_location, IAttackTarget target) { 
    //     base.ResolveAttackHit(hit, hit_location, target);
    //     explosion.Explode();
    // }
    // protected override void ResolveAttackMiss(GameObject hit, Vector3 hit_location, IAttackTarget target) {
    //     base.ResolveAttackMiss(hit, hit_location, target);
    //     explosion.Explode();
    // }

    protected override void DestroyProjectile() {
        base.DestroyProjectile();
        explosion.Explode();
    }
}
