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
    private HashSet<IAttackTarget> targets_hit;

    public LayerMask blocks_explosion;

    void Start()
    {
        Reset();
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

    public void Reset() {
        // calling Reset allows the explosion to be set to go off again
        targets_hit = new HashSet<IAttackTarget>();
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
        // foreach (Collider c in hits) {
        //     IAttackTarget target = c.gameObject.GetComponentInParent<IAttackTarget>();
        //     if (target != null) {
        //         if (targets_hit.Contains(target)) {
        //             continue; // target already hit, skip
        //         } else {
        //             targets_hit.Add(target);
        //             AttackResolver.ResolveAttackHit(explosion_attack, target, target.GetHitTarget().transform.position);
        //             Debug.LogWarning($"{target} was hit by explosion"); // TODO --- remove debug
        //         }
        //     }
        // }
        foreach (IAttackTarget target in GetExplosionHits()) {
            AttackResolver.ResolveAttackHit(explosion_attack, target, target.GetHitTarget().transform.position);
            Debug.LogWarning($"{target} was hit by explosion"); // TODO --- remove debug
        }
    }

    private IEnumerable<IAttackTarget> GetExplosionHits() {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosion_attack.explosion_radius);
        foreach (Collider c in hits) {
            IAttackTarget target = c.gameObject.GetComponentInParent<IAttackTarget>();
            if (target != null) {
                if (targets_hit.Contains(target)) {
                    continue; // target already hit, skip
                } else if (ExplosionBlocked(target)) {
                    Debug.LogWarning($"Explosion blocked for target {target}!"); // TODO --- remove debug
                    continue; 
                } else {
                    targets_hit.Add(target);
                    yield return target;
                }
            }
        }
    }

    public bool ExplosionBlocked(IAttackTarget target) {
        // returns a false if there is a wall protecting the target from the explosion
        Vector3 to_target = target.GetHitTarget().transform.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, to_target, out hit, explosion_attack.explosion_radius, blocks_explosion)) {
            // if the raycast hit hit a game object sharing a hierarchy with the target, the target is not blocked.
            return !GameObjectBelongsToTarget(hit.collider.gameObject, target);
        }
        // if the raycast hits nothing, the explosion isn't blocked
        return false;
    }

    public bool GameObjectBelongsToTarget(GameObject obj, IAttackTarget target) {
        // takes a GameObject and an IAttackTarget, and returns true if both are part of the same hierarchy.
        // if they are unrelated objects, return false.
        IAttackTarget obj_target = obj.GetComponentInParent<IAttackTarget>();
        if (obj_target == target) { return true; }
        else if (obj_target != null) {
            return false; // if the obj has an AttackTarget, but it's NOT the same target, than the explosion is blocked by something
        } else {
            // if there is no attack target in parents, check children
            obj_target = obj.GetComponentInChildren<IAttackTarget>();
            return obj_target == null || obj_target == target;
        }
    }
}