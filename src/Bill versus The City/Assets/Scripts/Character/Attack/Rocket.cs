using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Bullet {
    public Explosion explosion;

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
        Debug.LogError("pause!"); // TODO --- remove debug
        base.DestroyProjectile();
        explosion.Explode();
    }
}
