using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

 public class NavMeshAgentMovement : SimpleNavMeshAgentMovement
{
    public ManagedEnemyState managed_enemy_state;
    public float shoot_inaccuracy = 1f;
    public LayerMask obstacleMask;

    public bool saw_target { get { return perception.saw_target_last_frame; } } // saw the target last frame
    public bool seeing_target { get { return perception.seeing_target; }  } // seeing the target last frame

    public bool use_full_auto = false;

    private EnemyPerception perception;
    private EnemyBehavior behavior;

    public override void Start() {
        base.Start();
        perception = GetComponent<EnemyPerception>();
        behavior = GetComponent<EnemyBehavior>();
        EnemiesManager.inst.AddEnemy(this);
    }
    
    void Update() {
        UpdateReload();
    }

    // public Vector3 debug__look_direction;
    // public override void MoveCharacter(Vector3 move_target, Vector3 look_direction, bool sprint = false, bool crouch = false) {
    //     // TODO --- crouch not implemented 
    //     SetCharacterLookDirection(look_direction);
    //     // Debug.DrawRay(transform.position + Vector3.up, look_direction, Color.yellow);
    //     debug__look_direction = look_direction;

    //     if (crouch) { Debug.LogWarning("enemy crouch not implemented!"); }
    //     _is_sprinting = sprint;
    //     if (sprint) {
    //         nav_mesh_agent.speed = walk_speed * sprint_multiplier;
    //     } else {
    //         nav_mesh_agent.speed = walk_speed;
    //     }

    //     if (is_hit_stunned) {
    //         nav_mesh_agent.SetDestination(transform.position);
    //     } else if (move_target != nav_mesh_agent.destination) {
    //         nav_mesh_agent.SetDestination(move_target);
    //     }
    //     HandleAnimation();
    // }

    // public Vector3 GetMoveDestination() {
    //     switch (behavior.ctrl_move_mode) {
    //         case MovementTarget.stationary:
    //             return transform.position;

    //         case MovementTarget.target:
    //             if (behavior.ctrl_target != null) {
    //                 return behavior.ctrl_target.transform.position;
    //             }
    //             break;

    //         case MovementTarget.waypoint:
    //             if (behavior.ctrl_waypoint != null) {
    //                 return behavior.ctrl_waypoint;
    //             }
    //             break;

    //         default:
    //             Debug.LogWarning($"movement for {behavior.ctrl_move_mode} is not implemented!");
    //             break;
    //     }
    //     Debug.LogWarning("Don't move (not implemented correctly!)");
    //     return new Vector3(0f, 0f, 0f); // don't move
    // }

    public override Vector3 GetShootVector() {
        return ShootTarget() - attack_controller.shoot_from.position; //  DirectionFromLookTarget(ShootTarget());
    }

    private Vector3 ShootTarget() {
        return behavior.ctrl_target.GetAimTarget().position;
    }
    
    public override Vector3 GetLookTarget() {
        switch (behavior.ctrl_aim_mode) {
            case AimingTarget.stationary:
                return new Vector3(0f, 0f, 0f);

            case AimingTarget.target:
                if (behavior.ctrl_target != null) {
                    return behavior.ctrl_target.GetAimTarget().position;
                }
                break;

            case AimingTarget.waypoint:
                if (behavior.ctrl_target != null) {
                    return behavior.ctrl_waypoint;
                }
                break;

            case AimingTarget.movement_direction:
                return nav_mesh_agent.velocity + transform.position;

            default:
                Debug.LogWarning($"aiming for {behavior.ctrl_aim_mode} is not implemented!");
                break;
        }
        // TODO --- default case should make aiming not change, rather than pusing it to 
        // return new Vector3(0f, 0f, 0f);  
        return transform.forward + transform.position;
    }

    public override void OnDeath(ICharacterStatus status) {
        base.OnDeath(status);
        DisableCollision();
        MetricSystem.instance.IncrimentMetric("enemies_killed", 1);
        behavior.Kill();
        nav_mesh_agent.enabled = false;
    }

    public override void DelayedOnDeath(ICharacterStatus status) {
        base.DelayedOnDeath(status);
        // SpawnAmmoPickup();
        // SpawnWeaponPickup();
        EnemiesManager.inst.KillEnemy(this);
    }

    public override void OnDeathCleanup(ICharacterStatus status) {
        base.OnDeathCleanup(status);
        Destroy(gameObject);
    }

}

public enum MovementTarget {
    stationary,
    target,
    waypoint, 
    random
}
