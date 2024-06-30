using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class EnemyController : CharCtrl
{
    public float shoot_inaccuracy = 1f;
    public Transform target;

    public float shooting_rate = 0.75f;

    public override Vector3 MoveVector() {
        return new Vector3(0f, 0f, 0f);
    }

    public override Vector3 LookTarget() {
        if (target == null) {
            return new Vector3(0, 0, 0);
        }
        return target.position;
    }

    public override bool AttackInput() {
        // Debug.Log($"{Time.time} >= {this.last_attack_time} + {shooting_rate}: {Time.time >= (this.last_attack_time + shooting_rate)}");
        return Time.time >= (this.last_attack_time + shooting_rate);
    }

    public override Vector3 ShootVector() {
        return VectorFromLookTarget(ShootTarget());
    }

    private Vector3 ShootTarget() {
        float rand_x = Random.Range(-shoot_inaccuracy, shoot_inaccuracy);
        float rand_z = Random.Range(-shoot_inaccuracy, shoot_inaccuracy);
        Vector3 rand = new Vector3(rand_x, 0, rand_z);
        return LookTarget() + rand;
    }

    protected override void CharacterDeath() {
        base.CharacterDeath();
        Destroy(gameObject);
    }
}
