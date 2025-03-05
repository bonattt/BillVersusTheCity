using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IBullet
{
    
    public IAttackTarget attacker { get; set; }
    public IWeapon weapon { get; set; }
    public float attack_damage_min { get; set; }
    public float attack_damage_max { get; set; }
    public float armor_effectiveness { get; set; }
    // public float armor_damage { get; set; }
    public bool ignore_armor { get { return false; }}
    public float final_health_damage { get; set; }
    public float final_armor_damage { get; set; }

    public float time_to_live = 30f;
    private float start_time;
    private Rigidbody rb; 
    public float damage_falloff_rate = 0f;
    private Vector3 start_position;
    public float damage_falloff {
        get {
            float distance_from_start = Vector3.Distance(start_position, transform.position);
            return damage_falloff_rate * distance_from_start;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        start_position = transform.position;
        start_time = Time.time;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        // rb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    // Update is called once per frame
    void Update()
    {
        if (start_time + time_to_live <= Time.time) {
            DestroyProjectile();
        }
        Debug.DrawRay(transform.position, rb.velocity, Color.green, Time.deltaTime);
    }

    void OnCollisionEnter(Collision hit) {
        ResolveHit(hit.gameObject, hit.contacts[0].point);
    }

    void OnTriggerEnter(Collider hit) {
        ResolveHit(hit.gameObject, hit.gameObject.transform.position);
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        ResolveHit(hit.gameObject, hit.point);
    }

    public void ResolveHit(GameObject hit, Vector3 hit_location) {
        IAttackTarget target = hit.GetComponent<IAttackTarget>();
        if(target != null && target == this.attacker) {
            // do nothing, don't collide with the attacker
            // Debug.LogWarning("bullet ignores collision with the one who shot it!");
        }
        else if (target != null) {
            AttackResolver.ResolveAttackHit(this, target, hit_location);
            DestroyProjectile();
        } 
        else {
            AttackResolver.AttackMiss(this, hit_location);
            DestroyProjectile();
        }

    }

    private void DestroyProjectile() {
        Destroy(this.gameObject);
    }
}
