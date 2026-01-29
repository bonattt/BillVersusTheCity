using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class AbstractExplosion : MonoBehaviour, IExplosion
{
    public bool explode_on_start = false;
    [FormerlySerializedAs("destroy_on_explode")]
    [SerializeField]
    private bool _destroy_on_explode = true;
    public bool destroy_on_explode {
        get => _destroy_on_explode;
        set {
            _destroy_on_explode = value;
        }
    }
    private bool _has_exploded = false;
    public bool has_exploded { 
        get => _has_exploded;
        protected set {
            _has_exploded = value;
        }
    }
    public Transform raycast_from;
    [Tooltip("set to any non-NaN float to lock the Y-position of `raycast_from` to this value.")]
    public float raycast_fixed_height = 0.9f;

    public bool effect_completed { get => has_exploded; }
    protected HashSet<IAttackTarget> targets_hit;

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
    }

    public abstract ExplosionAttack explosion_attack { get; }
    public abstract string attack_sound_path { get; }
    public abstract IEnumerable<GameObject> explosion_effects { get; }
    public abstract float explosion_volume { get; }
    public abstract float explosion_radius { get; }
    public abstract void Explode();

    public void MakeGameSound() {
        // triggers game sound that enemies can hear
        float volume = explosion_volume * 2;
        GameSound new_sound = new GameSound(transform.position, range:volume, alarm_level:volume);
        new_sound.sound_name = "Explosion";
        new_sound.alerts_police = true;
        EnemyHearingManager.inst.NewSound(new_sound);
    }

    public void MakeSoundEffect() {
        Debug.LogWarning($"MakeSoundEffect('{attack_sound_path}')"); // TODO --- remove debug
        ISFXSounds explosion_sfx = SFXLibrary.LoadSound(attack_sound_path);
        SFXSystem.inst.PlaySound(explosion_sfx, transform.position);
    }

    public void Reset() {
        // calling Reset allows the explosion to be set to go off again
        targets_hit = new HashSet<IAttackTarget>();
    }

    protected void SpawnExplosionEffects()
    {
        foreach (GameObject prefab in explosion_effects) {
            GameObject effect = Instantiate(prefab);
            effect.transform.position = transform.position;

            IExplosionEffect effect_script = effect.GetComponent<IExplosionEffect>();
            if (effect_script != null) {
                effect_script.ConfigureFromExplosion(this);
            }
        }
    }
    protected void DealExplosionDamage()
    {
        explosion_attack.attack_from = GetRaycastStart();
        foreach (IAttackTarget target in GetExplosionHits()) {
            AttackResolver.ResolveAttackHit(explosion_attack, target, target.GetHitTarget().transform.position);
        }
    }

    public Vector3 GetRaycastStart() {
        Vector3 v;
        if (raycast_from == null) {
            v = transform.position;
        } else {
            v = raycast_from.position;
        }

        if (!float.IsNaN(raycast_fixed_height)) {
            v = new Vector3(v.x, raycast_fixed_height, v.z);
        }
        return v;
    }

    protected IEnumerable<IAttackTarget> GetExplosionHits() {
        Vector3 raycast_start = GetRaycastStart();
        Collider[] hits = Physics.OverlapSphere(raycast_start, explosion_radius);
        foreach (Collider c in hits) {
            IAttackTarget target = c.gameObject.GetComponentInParent<IAttackTarget>();
            if (target != null) {
                if (targets_hit.Contains(target)) {
                    continue; // target already hit, skip
                } else if (ExplosionBlocked(target)) {
                    Debug.DrawLine(raycast_start, target.GetAimTarget().position, Color.blue, 1f);
                    continue;
                } else {
                    Debug.DrawLine(raycast_start, target.GetAimTarget().position, Color.red, 1f);
                    targets_hit.Add(target);
                    yield return target;
                }
            }
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetRaycastStart(), explosion_radius);
    }

    public bool ExplosionBlocked(IAttackTarget target) {
        // returns a false if there is a wall protecting the target from the explosion
        Vector3 raycast_start = GetRaycastStart();
        Vector3 target_position = target.GetAimTarget().transform.position;
        Debug.DrawLine(raycast_start, target_position, Color.yellow, 1f);
        Vector3 to_target = target_position - raycast_start;
        RaycastHit hit;
        float raycast_length = Vector3.Distance(raycast_start, target_position);
        if (Physics.Raycast(raycast_start, to_target, out hit, raycast_length, blocks_explosion)) {
            // if the raycast hit hit a game object sharing a hierarchy with the target, the target is not blocked.
            return true; // raycast layerMask should not include damageable targets...
        }
        // if the raycast hits nothing, the explosion isn't blocked
        return false;
    }

    public bool GameObjectBelongsToTarget(GameObject obj, IAttackTarget target) {
        // takes a GameObject and an IAttackTarget, and returns true if both are part of the same hierarchy.
        // if they are unrelated objects, return false.
        IAttackTarget obj_target = obj.GetComponentInParent<IAttackTarget>();
        if (obj_target == target) { 
            return true; 
        }
        else if (obj_target != null) {
            return false; // if the obj has an AttackTarget, but it's NOT the same target, than the explosion is blocked by something
        } else {
            // if there is no attack target in parents, check children
            obj_target = obj.GetComponentInChildren<IAttackTarget>();
            return obj_target == target;
        }
    }
}