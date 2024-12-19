using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

 public class EnemyController : CharCtrl
{
    public float shoot_inaccuracy = 1f;
    public LayerMask obstacleMask;

    public bool saw_target { get { return perception.saw_target_last_frame; } } // saw the target last frame
    public bool seeing_target { get { return perception.seeing_target; }  } // seeing the target last frame

    public bool drop_weapon = false;

    public float ctrl_shooting_rate = 0.75f;
    private EnemyPerception perception;

    public GameObject weapon_pickup_prefab;


    //////////////////////////////
    /// Behavior controls ////////
    //////////////////////////////
    public CharCtrl ctrl_target;
    public bool ctrl_will_shoot = true;  // set by behavior, determines if the character will shoot if able
    public Vector3 ctrl_waypoint;  // arbitrary movement-target setable by behaviors
    public MovementTarget ctrl_move_mode = MovementTarget.stationary; // used by Behavior to instruct the controller how to move
    public AimingTarget ctrl_aim_mode = AimingTarget.target; // used by Behavior to instruct the controller how to aim

    public NavMeshAgent nav_mesh_agent;

    // void OnDestroy() {
    //     // DO nothing
    // }

    public override void SetupCharacter() {
        base.SetupCharacter();
        perception = GetComponent<EnemyPerception>();
        EnemiesManager.inst.AddEnemy(this);
    }

    protected override void Move() {
        LookWithAction();
        if (this.is_hit_stunned) {
            nav_mesh_agent.SetDestination(transform.position);
        }
        else {
            nav_mesh_agent.speed = this.movement_speed;
            nav_mesh_agent.SetDestination(MoveVector());
        }
    }

    public override Vector3 MoveVector() {
        switch (ctrl_move_mode) {
            case MovementTarget.stationary:
                return transform.position;

            case MovementTarget.target:
                if (ctrl_target != null) {
                    return ctrl_target.transform.position;
                }
                break;

            case MovementTarget.waypoint:
                if (ctrl_waypoint != null) {
                    return ctrl_waypoint;
                }
                break;

            default:
                Debug.LogWarning($"movement for {ctrl_move_mode} is not implemented!");
                break;
        }
        return new Vector3(0f, 0f, 0f); // don't move
    }

    public override bool AttackInput() {
        // Debug.Log($"{Time.time} >= {this.last_attack_time} + {ctrl_shooting_rate}: {Time.time >= (this.last_attack_time + ctrl_shooting_rate)}");
        if (seeing_target) {
            if (saw_target) {
                return Time.time >= (this.last_attack_time + ctrl_shooting_rate);
            }
            else {
                // start countdown to shoot once target is seen
                this.last_attack_time = Time.time;
            }
        }
        return false;
    }

    public override Vector3 ShootVector() {
        return ShootTarget() - attack_controller.shoot_point.position; //  VectorFromLookTarget(ShootTarget());
    }

    private Vector3 ShootTarget() {
        // float rand_x = Random.Range(-shoot_inaccuracy, shoot_inaccuracy);
        // float rand_z = Random.Range(-shoot_inaccuracy, shoot_inaccuracy);
        // Vector3 rand = new Vector3(rand_x, 0, rand_z);
        // return LookTarget() + rand;
        return ctrl_target.GetAimTarget().position;
    }
    
    public override Vector3 LookTarget() {
        switch (ctrl_aim_mode) {
            case AimingTarget.stationary:
                return new Vector3(0f, 0f, 0f);

            case AimingTarget.target:
                if (ctrl_target != null) {
                    return ctrl_target.GetAimTarget().position;
                }
                break;

            case AimingTarget.waypoint:
                if (ctrl_target != null) {
                    return ctrl_waypoint;
                }
                break;

            case AimingTarget.movement_direction:
                return nav_mesh_agent.velocity + transform.position;

            default:
                Debug.LogWarning($"aiming for {ctrl_aim_mode} is not implemented!");
                break;
        }
        // TODO --- default case should make aiming not change, rather than pusing it to 
        return new Vector3(0f, 0f, 0f);  
    }

    protected override void CharacterDeath() {
        SpawnWeaponPickup();
        Destroy(gameObject);
        EnemiesManager.inst.KillEnemy(this);
        MetricSystem.instance.IncrimentMetric("enemies_killed", 1);
    }

    protected void SpawnWeaponPickup() {
        // spawns a weapon pickup where the enemy is standing
        if (!drop_weapon) {
            return;
        }
        if (weapon_pickup_prefab == null || attack_controller.current_weapon == null) {
            Debug.Log("no weapon to drop as pickup!");
            return;
        }

        GameObject obj = Instantiate(weapon_pickup_prefab);
        obj.transform.position = transform.position;
        WeaponPickupInteraction pickup = obj.GetComponent<WeaponPickupInteraction>();
        pickup.pickup_weapon = attack_controller.current_weapon;
        if (pickup.pickup_weapon.current_ammo == 0) {
            pickup.pickup_weapon.current_ammo = pickup.pickup_weapon.ammo_capacity;
        }
    }

    /////////////////////////////
    /// DEBUG FIELDS ////////////
    /////////////////////////////
    
    public bool debug_seeing_target, debug_saw_target;
    public Vector3 debug_move_vector, debug_look_target;

    protected override void SetDebugData() {
        base.SetDebugData();
        debug_seeing_target = seeing_target;
        debug_saw_target = saw_target;
        debug_move_vector = MoveVector();
        debug_look_target = LookTarget();
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