

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

    [Tooltip("How long does it take to deal damage once.")]
    public float damage_period_seconds = 0.25f;

    [Tooltip("How much damage is dealt each period.")]
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
        UpdateDebug();
    }

    private Dictionary<IAttackTarget, AreaDamageTracking> tracked_targets = new Dictionary<IAttackTarget, AreaDamageTracking>();
    public void TargetInArea(IAttackTarget t, float at_time) {
        if (tracked_targets.ContainsKey(t)) {
            tracked_targets[t] = tracked_targets[t].UpdateMostRecentHit(Time.time);
        } else {
            tracked_targets[t] = new AreaDamageTracking(t, first_hit_at:Time.time);
        }
    }

    // private bool IsValidHit(IAttackTarget target, Collider collider) {
    //     return target != null && !already_hit.Contains(target) && !IsBlockedByWall(collider);
    // }

    void LateUpdate() {
        if (gameObject == null) { return; } // destroyed during `Update`, so do nothing
        if (Time.timeScale == 0) { 
            Debug.LogWarning("TODO: find a better way to avoid damage while paused");
            return; 
        }
        // run damage resolution in LateUpdate to guarantee colliders all resolve before resolving damage
        ResolveDamageOverTime();
    }

    private void ResolveDamageOverTime() {
        Dictionary<IAttackTarget, float> targets_damaged = new Dictionary<IAttackTarget, float>();
        HashSet<IAttackTarget> targets_removed = new HashSet<IAttackTarget>();
        foreach (AreaDamageTracking item in tracked_targets.Values) {
            if (float.IsNaN(item.last_resolved_at)) {
                // deal damage immediately on the first frame a target enters the area
                targets_damaged[item.target] = Time.time;
                DamageTarget(item.target);
                Debug.LogWarning($"deal first instance of damage immediately! '{item.target}'"); // TODO --- remove debug
                continue;
            }
            else if (Time.time >= item.last_resolved_at + damage_period_seconds) {
                // enough time has passed to deal damage
                if (item.most_recent_hit_at + (damage_period_seconds/4) <= Time.time) {
                    // target hasn't been tracked recently, so they have left the area since last damage instance
                    targets_removed.Add(item.target);
                    Debug.LogWarning($"un-track target: '{item.target}'"); // TODO --- remove debug
                } else {
                    Debug.LogWarning($"damage target: '{item.target}', {item.last_resolved_at} => {item.last_resolved_at + damage_period_seconds} (at {Time.time})"); // TODO --- remove debug
                    targets_damaged[item.target] = item.last_resolved_at + damage_period_seconds;
                    DamageTarget(item.target);
                }
                continue;
            }
            // else { continue; } // unnecessary
        }

        foreach (IAttackTarget t in targets_removed) {
            Debug.LogWarning($"removed {t}");
            tracked_targets.Remove(t);
        }
        foreach (IAttackTarget t in targets_damaged.Keys) {
            Debug.LogWarning($"update resolved to '{targets_damaged[t]}' (time:{Time.time} + period:{damage_period_seconds}) for '{t}'"); // TODO --- remove debug
            tracked_targets[t] = tracked_targets[t].UpdateResolvedAt(targets_damaged[t]);
        }
    }

    private void DamageTarget(IAttackTarget target) {
        Vector3 target_position = ((MonoBehaviour) target).transform.position;
        AttackResolver.ResolveAttackHit(GetAttack(), target, target_position);
    }

    private IAttack GetAttack() {
        return new AreaDamageAttack(this);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, area_radius);
    }

    // private const float raycast_height = 0.5f;
    // private bool IsBlockedByWall(Collider c) {
    //     Vector3 start = PhysicsUtils.PositionAtHeight(transform.position, raycast_height);
    //     Vector3 end = PhysicsUtils.PositionAtHeight(c.transform.position, raycast_height);
    //     Vector3 direction = end - start;
    //     RaycastHit hit;
    //     if (Physics.Raycast(start, direction.normalized, out hit, direction.magnitude, blocks_propegation)) {
    //         return true;
    //     }
    //     return false;
    // }

    // public Collider[] GetCurrentOverlap() {
    //     Vector3 collider_center = transform.position;
    //     return Physics.OverlapSphere(collider_center, area_radius);
    // }

    public AreaDamageRegionDebugger debug = new AreaDamageRegionDebugger();
    private void UpdateDebug() {
        # if UNITY_EDITOR
        debug.blocks_propegation = blocks_propegation;
        debug.tracked_targets = new List<string>();
        foreach (AreaDamageTracking item in tracked_targets.Values) {
            string target_name;
            try {
                target_name = ((MonoBehaviour) item.target).gameObject.name;
            } catch (InvalidCastException) {
                target_name = $"{item.target}";
            }
            debug.tracked_targets.Add($"{target_name}: (last_resolved; {item.last_resolved_at}, first_hit; {item.first_hit_at} recent_hit; {item.most_recent_hit_at})");
        }
        # endif
    }
}

public struct AreaDamageTracking {
    public IAttackTarget target;
    public float first_hit_at;
    public float most_recent_hit_at;
    public float last_resolved_at;

    public AreaDamageTracking(IAttackTarget target, float first_hit_at)
        : this(target, first_hit_at, first_hit_at) { /* do nothing */ }
    public AreaDamageTracking(IAttackTarget target, float first_hit_at, float most_recent_hit_at) 
        : this(target, first_hit_at, most_recent_hit_at, float.NaN) { /* do nothing */ }
    public AreaDamageTracking(IAttackTarget target, float first_hit_at, float most_recent_hit_at, float last_resolved_at) {
        this.target = target;
        this.first_hit_at = first_hit_at;
        this.most_recent_hit_at = most_recent_hit_at;
        this.last_resolved_at = last_resolved_at; // not never resolved yet
    }

    public AreaDamageTracking UpdateMostRecentHit(float new_most_recent) {
        return new AreaDamageTracking(this.target, this.first_hit_at, new_most_recent, this.last_resolved_at);
    }

    public AreaDamageTracking UpdateResolvedAt(float new_resolved_at) {
        return new AreaDamageTracking(this.target, this.first_hit_at, this.most_recent_hit_at, new_resolved_at);
    }
}

[Serializable]
public class AreaDamageRegionDebugger {
    // public List<string> tracked_targets;
    public LayerMask blocks_propegation;
    public List<string> tracked_targets;
}
