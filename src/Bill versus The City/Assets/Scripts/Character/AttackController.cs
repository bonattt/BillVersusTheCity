using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public IWeapon current_weapon;

    public Transform shoot_point; // bullets start here

    public GameObject bullet_prefab;

    public float bullet_speed = 35f;

    public void FireAttack(Vector3 attack_direction) {
        // fires an attack with the current weapon
        GameObject bullet = Instantiate(bullet_prefab) as GameObject;

        bullet.transform.position = shoot_point.position;
        Vector3 velocity = attack_direction.normalized * bullet_speed;
        bullet.GetComponent<Rigidbody>().velocity = velocity;
    }
}
