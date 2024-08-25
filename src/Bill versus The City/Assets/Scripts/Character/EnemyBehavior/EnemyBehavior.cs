using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public PlayerMovement player;
    public EnemyController controller;
    
    void Start()
    {
        if (controller == null) {
            controller = GetComponent<EnemyController>();
        }
    }

    void Update()
    {
        controller.ctrl_target = player.transform;
        controller.ctrl_waypoint = new Vector3(0, 0, 0);
        controller.ctrl_will_shoot = true;
        controller.ctrl_move_mode = MovementTarget.waypoint;
        if (controller.seeing_target) {
            controller.ctrl_aim_mode = AimingTarget.target;
        }
        else {
            Debug.Log("don't aim at unseen target");
            controller.ctrl_aim_mode = AimingTarget.stationary;
        }
    }
}
