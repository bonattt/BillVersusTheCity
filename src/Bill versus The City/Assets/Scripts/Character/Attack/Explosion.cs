using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour, IGameEventEffect
{
    public bool explode_on_start = true;
    public bool destroy_on_explode = true;
    private bool _exploded = false;
    public ExplosionAttack explosion_attack;

    private bool _effect_completed = false;
    public bool effect_completed { get => _effect_completed; }

    void Start()
    {
        if (explode_on_start)
        {
            Explode();
        }
    }

    public void ActivateEffect()
    {
        Explode();
        _effect_completed = true;
    }


    public void Explode()
    {
        if (_exploded) { return; } // only explode once
        _exploded = true;
        SpawnExplosionEffects();
        DealExplosionDamage();
        if (destroy_on_explode) {
            Debug.LogWarning("destroy on explode!"); // TODO --- remove debug
            Destroy(gameObject);
        } 
        ///
        else { Debug.LogWarning("DON'T destroy"); }// TODO --- remove debug
        
    }

    private void SpawnExplosionEffects()
    {
        foreach (GameObject prefab in explosion_attack.explosion_effects)
        {
            GameObject effect = Instantiate(prefab);
            effect.transform.position = transform.position;
        }
    }
    private void DealExplosionDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosion_attack.explosion_radius);
        HashSet<IAttackTarget> targets_hit = new HashSet<IAttackTarget>();
        foreach (Collider c in hits) {
            IAttackTarget target = c.gameObject.GetComponentInParent<IAttackTarget>();
            if (target != null) {
                if (targets_hit.Contains(target)) {
                    continue; // target already hit, skip
                } else {
                    targets_hit.Add(target);
                    AttackResolver.ResolveAttackHit(explosion_attack, target, target.GetHitTarget().transform.position);
                    Debug.LogWarning($"{target} was hit by explosion"); // TODO --- remove debug
                }
            }
        }
    }
}