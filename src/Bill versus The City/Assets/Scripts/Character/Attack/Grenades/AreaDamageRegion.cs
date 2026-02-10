

using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamageRegion : MonoBehaviour, IAreaEffectRegion
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

    private ParticlesSoftKillTimer _kill_timer;
    protected ParticlesSoftKillTimer kill_timer {
        get {
            if (_kill_timer == null) {
                AddKillTimer();
            }
            return _kill_timer;
        }
    }
    private float _destroy_after;

    private void AddKillTimer() {
        _kill_timer = gameObject.AddComponent<ParticlesSoftKillTimer>();
        _kill_timer.duration = area_effect_duration;
        ParticleSystem[] all_particles = GetComponentsInChildren<ParticleSystem>();
        if (all_particles != null) {
            _kill_timer.AddParticleSystems(all_particles);
        }
        _destroy_after = Time.time + area_effect_duration;
        // Destroy(gameObject, area_effect_duration + 0.1f); 
    }

    private List<IAreaEffectPartial> children = new List<IAreaEffectPartial>();
    public void AddChild(IAreaEffectPartial new_child) {
        children.Add(new_child);
        new_child.parent = this;
        ParticlesSoftKiller killer = ((MonoBehaviour) new_child).GetComponent<ParticlesSoftKiller>(); 
        kill_timer.AddChild(killer);
    }

    public void RemoveChild(IAreaEffectPartial new_child) {
        children.Remove(new_child);
    }

    void Start() {
        AddKillTimer();
    }

    void Update() {
        if (_destroy_after <= Time.time) {
            kill_timer.SoftKill(); // must detatch particles before Destroying
            Destroy(gameObject);
            return;
        }
        // foreach (Collider c in GetCurrentOverlap()) {
        //     IAttackTarget t = c.gameObject.GetComponent<IAttackTarget>();
        //     if (IsValidHit(t, c)) {
        //         DealAreaDamageToTarget(t);
        //     }
        // }
        Debug.LogWarning("TODO --- update damage on any tracked targets");
        UpdateDebug();
    }

    public void TargetInArea(IAttackTarget t, float at_time) {
        Debug.LogWarning("TODO: implement TargetInArea");
    }

    private bool IsValidHit(IAttackTarget target, Collider collider) {
        return target != null && !already_hit.Contains(target) && !IsBlockedByWall(collider);
    }

    void LateUpdate() {
        
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, area_radius);
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
        return Physics.OverlapSphere(collider_center, area_radius);
    }

    private void DealAreaDamageToTarget(IAttackTarget target) {
        already_hit.Add(target);
        AttackResolver.ResolveDamageOverTime(target, damage_rate, Time.deltaTime);
    }


    public AreaDamageRegionDebugger debug = new AreaDamageRegionDebugger();
    private void UpdateDebug() {
        # if UNITY_EDITOR
        debug.blocks_propegation = blocks_propegation;
        # endif
    }
}

[Serializable]
public class AreaDamageRegionDebugger {
    // public List<string> tracked_targets;
    public LayerMask blocks_propegation;
}
