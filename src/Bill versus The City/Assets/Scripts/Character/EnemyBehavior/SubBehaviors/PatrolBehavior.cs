using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PatrolBehavior : MonoBehaviour, ISubBehavior  {

    public float shooting_rate { get { return 1f; }}
    public List<Transform> patrol_points;
    public bool loop = false;
    private int iteration_step = 1;
    private int current = 0; 

    private const float distance_to_arrival = 0.1f;
    private bool initialized = false;

    private const float COOLDOWN = 2f;
    private float cooldown = 0f;
    private Vector3 destination;
    
    void Start() { 
        initialized = false;
    }
    
    public void SetControllerFlags(EnemyBehavior parent) {
        if (!initialized) {
            SetNewDestination(parent);
            initialized = true;
        }

        parent.controller.ctrl_will_shoot = false;
        float distance = Vector3.Distance(parent.transform.position, destination);
        // waiting before moving towards new destination
        if (cooldown > 0) {
            parent.controller.ctrl_move_mode = MovementTarget.stationary;
            parent.controller.ctrl_aim_mode = AimingTarget.stationary;
            cooldown -= Time.deltaTime;
        } 
        // arrival at latest destination
        else if (distance_to_arrival >= distance) {
            parent.controller.ctrl_move_mode = MovementTarget.stationary;
            parent.controller.ctrl_aim_mode = AimingTarget.stationary;
            SetNewDestination(parent);
        }
        // still traveling to destination
        else {
            parent.controller.ctrl_waypoint = destination;
            parent.controller.ctrl_move_mode = MovementTarget.waypoint;
            parent.controller.ctrl_aim_mode = AimingTarget.movement_direction;
        }
    }

    protected void SetNewDestination(EnemyBehavior parent) {
        parent.controller.ctrl_move_mode = MovementTarget.stationary;
        parent.controller.ctrl_aim_mode = AimingTarget.stationary;
        cooldown = COOLDOWN;
        
        if (current == patrol_points.Count - 1) {
            if (loop) {
                iteration_step = 1;
                current = 0;
            } else {
                iteration_step = -1;
            }
        }
        else if (current == 0) {
            iteration_step = 1;
        }

        current += iteration_step;
        destination = patrol_points[current].position;
    }
}
