using UnityEngine;

public class WonderingBehavior : ISubBehavior  {

    public float shooting_rate { get { return 1f; }}


    public float wonder_distance_max = 7f;
    public float wonder_cooldown_max = 5f;
    public float wonder_cooldown_min = 2f;
    
    // how close does the character need to be to the destination to be considered as arrived
    private const float distance_to_arrival = 0.1f;

    private float cooldown = 0f;
    private Vector3 destination;
    
    public WonderingBehavior(EnemyBehavior parent) { 
        SetNewDestination(parent);
    }
    
    public void SetControllerFlags(EnemyBehavior parent, PlayerMovement player) {

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
        cooldown = Random.Range(wonder_cooldown_min, wonder_cooldown_max);
        destination = NavMeshUtils.GetRandomNavMeshPoint(parent.controller.nav_mesh_agent, wonder_cooldown_max);
    }
}