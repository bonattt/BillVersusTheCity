

using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamage : MonoBehaviour
{
    // deals damage in an area

    // Static HashSet used to prevent 2 instances of overlapping AoE from stacking damage
    private static HashSet<IAttackTarget> already_hit = new HashSet<IAttackTarget>();

    public float damage_rate = 20f;

    private HashSet<IAttackTarget> targets_in_area = new HashSet<IAttackTarget>();
    private HashSet<IAttackTarget> remove_targets = new HashSet<IAttackTarget>();

    private SphereCollider _collider;
    public SphereCollider collider_ {
        get {
            if (_collider == null) {
                _collider = GetComponent<SphereCollider>();
            }
            return _collider;
        }
    }

    void Start() {
        // on start, set anything already inside the area as tracked
        targets_in_area = new HashSet<IAttackTarget>();
        foreach (Collider c in GetCurrentOverlap()) {
            TryAddingCollider(c);
        }
    }

    void Update() {
        foreach (IAttackTarget t in targets_in_area) {
            DealAreaDamageToTarget(t);
        }
        if (remove_targets.Count > 0) {
            // remove targets that are already dead,
            foreach(IAttackTarget t in remove_targets) {
                targets_in_area.Remove(t);
            }
            remove_targets = new HashSet<IAttackTarget>();
        }
        UpdateDebug();
    }

    void OnTriggerEnter(Collider c) {
        TryAddingCollider(c);
    }

    void OnTriggerExit(Collider c) {
        TryRemovingCollider(c);
    }

    void OnDrawGizmos() {
        
        Vector3 collider_center = collider_.center + transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(collider_center, scaled_radius);
    }

    private void TryAddingCollider(Collider c) {
        IAttackTarget t = c.gameObject.GetComponent<IAttackTarget>();
        if (t != null) {
            targets_in_area.Add(t);
        }
    }

    private void TryRemovingCollider(Collider c) {
        IAttackTarget t = c.gameObject.GetComponent<IAttackTarget>();
        if (t != null) {
            targets_in_area.Remove(t);
        }
    }

    public float scaled_radius => transform.localScale.x * collider_.radius;

    public Collider[] GetCurrentOverlap() {
        Vector3 collider_center = collider_.center + transform.position;
        Debug.LogWarning($"Physics.OverlapSphere(position: {collider_center}, radius: {collider_.radius})"); // TODO --- remove debug
        return Physics.OverlapSphere(collider_center, scaled_radius);
    }

    private void DealAreaDamageToTarget(IAttackTarget target) {
        if (target.GetStatus().health <= 0) {
            remove_targets.Add(target); // remove dead targets
            return;
        }
        if (already_hit.Contains(target)) { return; } // skip already damaged targets;

        AttackResolver.ResolveDamageOverTime(target, damage_rate, Time.deltaTime);
    }


    public AreaDamageDebugger debug;
    private void UpdateDebug() {
        # if UNITY_EDITOR
        debug.tracked_targets = new List<string>();
        foreach (IAttackTarget t in targets_in_area) {
            debug.tracked_targets.Add($"{t}");
        }
        # endif
    }
}

[Serializable]
public class AreaDamageDebugger {
    public List<string> tracked_targets;
}