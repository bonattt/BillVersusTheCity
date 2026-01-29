

using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamage : MonoBehaviour, IAreaEffect
{
    /* deals damage in an area */
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
    private static HashSet<IAttackTarget> already_hit = new HashSet<IAttackTarget>();

    public float damage_rate = 20f;

    private HashSet<IAttackTarget> targets_in_area = new HashSet<IAttackTarget>();
    private HashSet<IAttackTarget> remove_targets = new HashSet<IAttackTarget>();

    // private SphereCollider _collider;
    // public SphereCollider collider_ {
    //     get {
    //         if (_collider == null) {
    //             _collider = GetComponent<SphereCollider>();
    //         }
    //         return _collider;
    //     }
    // }
    public SphereCollider collider_;

    private float _area_radius;

    public float area_radius
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
                scaling_transform.localScale = new Vector3(area_radius, area_radius, area_radius);
                collider_.radius = 1f;
                break;

            case AreaDamageScalingMode.collider:
                scaling_transform.localScale = new Vector3(1f, 1f, 1f);
                collider_.radius = area_radius;
                break;

            default:
                Debug.LogError($"unhandled scaling mode '{scaling_mode}'");
                break;
        }
    }
    
    private float _area_effect_duration;
    public float area_effect_duration { 
        get => _area_effect_duration;
        set {
            _area_effect_duration = value;
            if (kill_timer != null) {
                kill_timer.duration = value;
            }
        }
    }
    
    public LayerMask blocks_propegation { get; set; }

    private ParticlesSoftKillTimer kill_timer;

    void Start() {
        // on start, set anything already inside the area as tracked
        targets_in_area = new HashSet<IAttackTarget>();
        foreach (Collider c in GetCurrentOverlap()) {
            TryAddingCollider(c);
        }
        AddDuration();
    }

    private void AddDuration() {
        kill_timer = gameObject.AddComponent<ParticlesSoftKillTimer>();
        kill_timer.duration = area_effect_duration;
        ParticleSystem[] all_particles = GetComponentsInChildren<ParticleSystem>();
        if (all_particles != null) {
            kill_timer.AddParticleSystems(all_particles);
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
        if (t != null && !IsBlockedByWall(c)) {
            targets_in_area.Add(t);
        }
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

    private void TryRemovingCollider(Collider c) {
        IAttackTarget t = c.gameObject.GetComponent<IAttackTarget>();
        if (t != null) {
            targets_in_area.Remove(t);
        }
    }

    public float scaled_radius => transform.localScale.x * collider_.radius;

    public Collider[] GetCurrentOverlap() {
        Vector3 collider_center = collider_.center + transform.position;
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
        debug.blocks_propegation = blocks_propegation;
        # endif
    }
}

[Serializable]
public class AreaDamageDebugger {
    public List<string> tracked_targets;
    public LayerMask blocks_propegation;
}


public enum AreaDamageScalingMode {
    collider,
    transform,
}