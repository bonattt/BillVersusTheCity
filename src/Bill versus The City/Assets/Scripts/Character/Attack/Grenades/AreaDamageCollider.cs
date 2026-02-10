

using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamageCollider : MonoBehaviour, IAreaEffectPartial
{
    /* represents a smaller instance area among many areas within a larger AreaDamageRegion */

    public IAreaEffectRegion parent { get; set; }

    [Tooltip("Affects method of scaling the area-damage effect. This may impact interaction with other scripts. (EG. Particle System scaling)")]
    [SerializeField] private AreaDamageScalingMode _scaling_mode = AreaDamageScalingMode.transform;
    public AreaDamageScalingMode scaling_mode {
        get => _scaling_mode;
        set {
            _scaling_mode = value;
            _UpdateScaling();
        }
    }
    // Static HashSet used to prevent 2 instances of overlapping AoE from stacking damage
    // private static HashSet<IAttackTarget> already_hit = new HashSet<IAttackTarget>();

    public float damage_rate = 20f;

    private float _area_radius;

    public float sub_area_radius
    {
        get => _area_radius;
        set
        {
            _area_radius = value;
            _UpdateScaling();
        }
    }

    private Transform scaling_transform => transform;

    private void _UpdateScaling() {
        switch (scaling_mode) {
            case AreaDamageScalingMode.transform:
                scaling_transform.localScale = new Vector3(sub_area_radius, sub_area_radius, sub_area_radius);
                break;

            default:
                Debug.LogError($"unhandled scaling mode '{scaling_mode}'");
                break;
        }
    }
    
    // private float _area_effect_duration;
    // public float area_effect_duration { 
    //     get => _area_effect_duration;
    //     set {
    //         _area_effect_duration = value;
    //     }
    // }
    
    public LayerMask blocks_propegation { get; set; }

    private ParticlesSoftKiller _particle_soft_kill;
    // private ParticlesSoftKiller particle_soft_kill {
    //     get {
    //         if (_particle_soft_kill == null) {
    //             _particle_soft_kill = GetComponent<ParticlesSoftKiller>();
    //             if (_particle_soft_kill == null) {
    //                 // if GetComponent doesn't find the script, add it manually.
    //                 _AddParticlesSoftKiller();
    //             }
    //         }
    //         return _particle_soft_kill;
    //     }
    // }
    void Start() {
        _AddParticlesSoftKiller();
    }

    private void _AddParticlesSoftKiller() {
        _particle_soft_kill = gameObject.AddComponent<ParticlesSoftKiller>();
        ParticleSystem[] all_particles = GetComponentsInChildren<ParticleSystem>();
        if (all_particles != null) {
            _particle_soft_kill.AddParticleSystems(all_particles);
        }
    }

    void OnDestroy() {
        _particle_soft_kill.SoftKill();
    }

    void Update() {
        foreach (Collider c in GetCurrentOverlap()) {
            IAttackTarget t = c.gameObject.GetComponent<IAttackTarget>();
            if (IsValidHit(t, c)) {
                parent.TargetInArea(t, Time.time);
            }
        }
        // already_hit_reset = false;
        UpdateDebug();
    }

    private bool IsValidHit(IAttackTarget target, Collider collider) {
        return target != null && !IsBlockedByWall(collider); // && !already_hit.Contains(target);
    }

    // private bool already_hit_reset = false;
    // void LateUpdate() {
    //     if (!already_hit_reset) {
    //         already_hit = new HashSet<IAttackTarget>();
    //     }
    //     already_hit_reset = true;
    // }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sub_area_radius);
    }

    private const float raycast_height = 0.5f;
    private bool IsBlockedByWall(Collider c) {
        Vector3 start = PhysicsUtils.PositionAtHeight(transform.position, raycast_height);
        Vector3 end = PhysicsUtils.PositionAtHeight(c.transform.position, raycast_height);
        Vector3 direction = end - start;
        RaycastHit hit;
        if (Physics.Raycast(start, direction.normalized, out hit, direction.magnitude, blocks_propegation)) {
            return true;
        }
        return false;
    }

    public Collider[] GetCurrentOverlap() {
        Vector3 collider_center = transform.position;
        return Physics.OverlapSphere(collider_center, sub_area_radius);
    }

    // private void DealAreaDamageToTarget(IAttackTarget target) {
    //     already_hit.Add(target);
    //     AttackResolver.ResolveDamageOverTime(target, damage_rate, Time.deltaTime);
    // }


    public AreaDamageColliderDebugger debug;
    private void UpdateDebug() {
        # if UNITY_EDITOR
        debug.blocks_propegation = blocks_propegation;
        # endif
    }
}

[Serializable]
public class AreaDamageColliderDebugger {
    // public List<string> tracked_targets;
    public LayerMask blocks_propegation;
}

