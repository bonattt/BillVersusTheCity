using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public IWeapon current_weapon;
    public IAttackTarget attacker = null;

    public Transform shoot_point; // bullets start here

    public GameObject bullet_prefab;

    // how fast bullets move when fired
    public float bullet_speed = 35f;
    // how long must pass from the previous shot before you can shoot again
    public float shot_cooldown = 0.1f; 
    
    // tracks when the last shot was fired, for handling rate-of-fire
    private float _last_shot_at = 0f;

    public void FireAttack(Vector3 attack_direction) {
        // fires an attack with the current weapon
        if (_last_shot_at + shot_cooldown <= Time.time) {
            _FireAttack(attack_direction);
        }
    }

    private void _FireAttack(Vector3 attack_direction) {
        _last_shot_at = Time.time;
        GameObject bullet_obj = Instantiate(bullet_prefab) as GameObject;

        bullet_obj.transform.position = shoot_point.position;
        Vector3 velocity = attack_direction.normalized * bullet_speed;
        bullet_obj.GetComponent<Rigidbody>().velocity = velocity;

        Bullet bullet = bullet_obj.GetComponent<Bullet>();
        bullet.attack_damage_max = 50f;
        bullet.attack_damage_min = 10f;
        bullet.attacker = attacker;
    }
}
