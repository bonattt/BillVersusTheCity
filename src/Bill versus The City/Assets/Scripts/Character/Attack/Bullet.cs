using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IBullet
{
    
    public IAttackTarget attacker { get; set; }
    public float attack_damage_min { get; set; }
    public float attack_damage_max { get; set; }
    public float armor_damage { get; set; }

    public float time_to_live = 30f;
    private float start_time;
    private Rigidbody rb; 

    // Start is called before the first frame update
    void Start()
    {
        start_time = Time.time;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    // Update is called once per frame
    void Update()
    {
        if (start_time + time_to_live <= Time.time) {
            DestroyProjectile();
        }
    }

    void OnCollisionEnter(Collision hit) {
        Debug.Log($"OnCollisionEnter({hit.gameObject})");
        ResolveHit(hit.gameObject, hit.contacts[0].point);
    }

    void OnTriggerEnter(Collider hit) {
        Debug.Log($"OnTriggerEnter({hit.gameObject})");
        ResolveHit(hit.gameObject, hit.gameObject.transform.position);
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        Debug.Log($"OnControllerColliderHit({hit.gameObject})");
        ResolveHit(hit.gameObject, hit.gameObject.transform.position);
    }

    public void ResolveHit(GameObject hit, Vector3 hit_location) {
        IAttackTarget target = hit.GetComponent<IAttackTarget>();
        if(target != null && target == this.attacker) {
            // do nothing, don't collide with the attacker
            Debug.LogWarning("bullet ignores collision with the one who shot it!");
        }
        else if (target != null) {
            AttackResolver.ResolveAttackHit(this, target, hit_location);
            DestroyProjectile();
        } 
        else {
            Debug.Log("hit non-target");
            DestroyProjectile();
        }

    }

    private void DestroyProjectile() {
        Destroy(this.gameObject);
    }
}
