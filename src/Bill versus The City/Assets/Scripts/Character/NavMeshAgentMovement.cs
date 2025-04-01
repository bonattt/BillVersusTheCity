using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

 public class NavMeshAgentMovement : CharCtrl
{
    public float shoot_inaccuracy = 1f;
    public LayerMask obstacleMask;

    public bool saw_target { get { return perception.saw_target_last_frame; } } // saw the target last frame
    public bool seeing_target { get { return perception.seeing_target; }  } // seeing the target last frame

    public bool drop_weapon = false;
    public bool drop_ammo = false;
    public bool use_full_auto = false;

    private EnemyPerception perception;
    private EnemyBehavior behavior;

    public GameObject ammo_pickup_prefab, weapon_pickup_prefab;

    public override bool is_sprinting {
        get {
            return behavior.ctrl_sprint;
        }
    }

    public NavMeshAgent nav_mesh_agent;

    public override void Start() {
        base.Start();
        perception = GetComponent<EnemyPerception>();
        behavior = GetComponent<EnemyBehavior>();
        EnemiesManager.inst.AddEnemy(this);
    }
    
    void Update() {
        UpdateReload();
    }

    public override void SetPosition(Vector3 new_position) {
        nav_mesh_agent.Warp(new_position);
    }
    
    public Vector3 debug_look_direction;
    public override void MoveCharacter(Vector3 move_target, Vector3 look_direction, bool sprint=false, bool crouch=false) {
        // TODO --- crouch not implemented 
        SetCharacterLookDirection(look_direction);
        // Debug.DrawRay(transform.position + Vector3.up, look_direction, Color.yellow);
        debug_look_direction = look_direction;

        if (is_hit_stunned) {
            nav_mesh_agent.SetDestination(transform.position);
        } else if (move_target != nav_mesh_agent.destination) {
            nav_mesh_agent.SetDestination(move_target);
        }

    }

    public Vector3 MoveDirection() {
        return (nav_mesh_agent.nextPosition - transform.position).normalized;
    }
    
    public override Vector3 GetVelocity() {
        return MoveDirection() * movement_speed;
    }

    public Vector3 GetMoveDestination() {
        switch (behavior.ctrl_move_mode) {
            case MovementTarget.stationary:
                return transform.position;

            case MovementTarget.target:
                if (behavior.ctrl_target != null) {
                    return behavior.ctrl_target.transform.position;
                }
                break;

            case MovementTarget.waypoint:
                if (behavior.ctrl_waypoint != null) {
                    return behavior.ctrl_waypoint;
                }
                break;

            default:
                Debug.LogWarning($"movement for {behavior.ctrl_move_mode} is not implemented!");
                break;
        }
        Debug.LogWarning("Don't move (not implemented correctly!)");
        return new Vector3(0f, 0f, 0f); // don't move
    }

    public override Vector3 GetShootVector() {
        return ShootTarget() - attack_controller.shoot_point.position; //  DirectionFromLookTarget(ShootTarget());
    }

    private Vector3 ShootTarget() {
        return behavior.ctrl_target.GetAimTarget().position;
    }
    
    public Vector3 GetLookTarget() {
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

    private void DisableCollision() {
        GetComponent<CapsuleCollider>().enabled = false;
    }
    public override void DelayedOnDeath(ICharacterStatus status) {
        base.DelayedOnDeath(status);
        SpawnAmmoPickup();
        SpawnWeaponPickup();
        EnemiesManager.inst.KillEnemy(this);
    }

    public override void OnDeathCleanup(ICharacterStatus status) {
        base.OnDeathCleanup(status);
        Destroy(gameObject);
    }

    protected void SpawnAmmoPickup() {
        // spawns a weapon pickup where the enemy is standing
        if (!drop_ammo) {
            return;
        }
        if (ammo_pickup_prefab == null || attack_controller.current_weapon == null) {
            Debug.LogWarning("no weapon to drop ammo from!");
            return;
        }

        GameObject obj = Instantiate(ammo_pickup_prefab);
        obj.transform.position = transform.position + new Vector3(0f, 0f, 0.25f);
        AmmoPickupInteraction pickup = obj.GetComponent<AmmoPickupInteraction>();
        pickup.ammo_type = attack_controller.current_weapon.ammo_type;
        pickup.ammo_amount = attack_controller.current_weapon.ammo_drop_size;
    }

    protected void SpawnWeaponPickup() {
        // spawns a weapon pickup where the enemy is standing
        if (!drop_weapon) {
            return;
        }
        if (weapon_pickup_prefab == null || attack_controller.current_weapon == null) {
            Debug.LogWarning("no weapon to drop as pickup!");
            return;
        }

        GameObject obj = Instantiate(weapon_pickup_prefab);
        obj.transform.position = transform.position + new Vector3(0f, 0f, -0.25f);;
        WeaponPickupInteraction pickup = obj.GetComponent<WeaponPickupInteraction>();
        pickup.pickup_weapon = attack_controller.current_weapon;
    }
}

public enum MovementTarget {
    stationary,
    target,
    waypoint, 
    random
}

public enum AimingTarget {
    stationary,
    target,
    waypoint,
    movement_direction  // TODO --- not implemented
}