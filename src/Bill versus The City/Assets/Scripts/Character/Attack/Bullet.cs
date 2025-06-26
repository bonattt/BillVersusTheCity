using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IBullet
{
    
    public Transform location { get { return transform; } }
    public IAttackTarget attacker { get; set; }
    public bool is_threat { get => PlayerCharacter.IsPlayer(attacker);  } // TODO --- check if attacker is player
    public IWeapon weapon {
        get => firearm;
        set {
            firearm = (IFirearm)value;
        }
    }
    public IFirearm firearm { get; set; }
    public float attack_damage_min { get; set; }
    public float attack_damage_max { get; set; }
    public float armor_effectiveness { get; set; }
    // public float armor_damage { get; set; }
    public bool ignore_armor { get { return false; }}
    public float final_health_damage { get; set; }
    public float final_armor_damage { get; set; }

    public float time_to_live = 30f;
    private float start_time;
    protected Rigidbody rb; 
    public float damage_falloff_rate = 0f;
    private Vector3 start_position;
    public float damage_falloff {
        get {
            float distance_from_start = Vector3.Distance(start_position, transform.position);
            return damage_falloff_rate * distance_from_start;
        }
    }

    protected static ulong bullet_count = 0;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        start_position = transform.position;
        start_time = Time.time;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        // rb.constraints = RigidbodyConstraints.FreezePositionY;
        gameObject.name = $"bullet {++bullet_count}";
        BulletTracking.inst.TrackNewBullet(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (start_time + time_to_live <= Time.time) {
            Debug.LogWarning($"{gameObject.name} destroyed by time out!!");
            DestroyProjectile();
        }
        Debug.DrawRay(transform.position, rb.velocity, Color.green, Time.deltaTime);
    }

    void OnCollisionEnter(Collision hit) {
        ResolveCollision(hit.gameObject, hit.contacts[0].point);
    }

    void OnTriggerEnter(Collider hit) {
        ResolveCollision(hit.gameObject, hit.gameObject.transform.position);
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        ResolveCollision(hit.gameObject, hit.point);
    }

    public void ResolveCollision(GameObject hit, Vector3 hit_location) {
        IAttackTarget target = hit.GetComponent<IAttackTarget>();
        if (target != null && target == this.attacker) {
            // do nothing, don't collide with the attacker
            // Debug.LogWarning("bullet ignores collision with the one who shot it!");
        } else if (target != null) {
            ResolveAttackHit(hit, hit_location, target);
        } else {
            ResolveAttackMiss(hit, hit_location, target);
        }
    }

    protected virtual void ResolveAttackHit(GameObject hit, Vector3 hit_location, IAttackTarget target) {
        AttackResolver.ResolveAttackHit(this, target, hit_location);
        BulletTracking.inst.TrackHit(this, target, hit_location);
        DestroyProjectile();
    }

    protected virtual void ResolveAttackMiss(GameObject hit, Vector3 hit_location, IAttackTarget target) {
        AttackResolver.AttackMiss(this, hit_location);
        BulletTracking.inst.TrackHit(this, null, hit_location);
        DestroyProjectile();
    }

    protected virtual void DestroyProjectile() {
        Destroy(this.gameObject);
    }

    void OnDestroy() {
        BulletTracking.inst.UnTrackBullet(this);
    }
}
