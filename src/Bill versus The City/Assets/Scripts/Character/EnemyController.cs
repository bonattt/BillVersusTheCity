using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

 public class EnemyController : CharCtrl
{
    public float shoot_inaccuracy = 1f;
    public LayerMask obstacleMask;

    [SerializeField]
    public bool saw_target { get; protected set; } // saw the target last frame
    
    [SerializeField]
    public bool seeing_target { get; protected set; } // seeing the target last frame

    public float shooting_rate = 0.75f;

    //////////////////////////////
    /// Behavior controls ////////
    //////////////////////////////
    public Transform ctrl_target;
    public bool ctrl_will_shoot = true;  // set by behavior, determines if the character will shoot if able
    public Vector3 ctrl_waypoint;  // arbitrary movement-target setable by behaviors
    public MovementTarget ctrl_move_mode = MovementTarget.stationary; // used by Behavior to instruct the controller how to move
    public AimingTarget ctrl_aim_mode = AimingTarget.target; // used by Behavior to instruct the controller how to aim

    public NavMeshAgent nav_mesh_agent;

    public override void SetupCharacter() {
        base.SetupCharacter();
        saw_target = false;
        seeing_target = false;
    }

    protected override void Move() {
        LookWithAction();
        if (this.is_hit_stunned) {
            nav_mesh_agent.SetDestination(transform.position);
        }
        else {
            nav_mesh_agent.SetDestination(MoveVector());
        }
    }

    public override Vector3 MoveVector() {
        switch (ctrl_move_mode) {
            case MovementTarget.stationary:
                return transform.position;

            case MovementTarget.target:
                if (ctrl_target != null) {
                    return ctrl_target.position;
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

    public override Vector3 LookTarget() {
        switch (ctrl_aim_mode) {
            case AimingTarget.stationary:
                return new Vector3(0f, 0f, 0f);

            case AimingTarget.target:
                if (ctrl_target != null) {
                    return ctrl_target.position;
                }
                break;

            case AimingTarget.waypoint:
                if (ctrl_target != null) {
                    return ctrl_waypoint;
                }
                break;

            default:
                Debug.LogWarning($"aiming for {ctrl_aim_mode} is not implemented!");
                break;
        }
        // TODO --- default case should make aiming not change, rather than pusing it to 
        return new Vector3(0f, 0f, 0f);  
    }

    public override bool AttackInput() {
        // Debug.Log($"{Time.time} >= {this.last_attack_time} + {shooting_rate}: {Time.time >= (this.last_attack_time + shooting_rate)}");
        if (seeing_target) {
            if (saw_target) {
                return Time.time >= (this.last_attack_time + shooting_rate);
            }
            else {
                // start countdown to shoot once target is seen
                this.last_attack_time = Time.time;
            }
        }
        return false;
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

    public bool LineOfSightToTarget() {
        if (ctrl_target == null) {
            Debug.LogWarning("no target!");
            return false;
        }
        RaycastHit hit;
        Vector3 offset = new Vector3(0f, 0.5f, 0f);
        Vector3 start = transform.position + offset;
        Vector3 end = ctrl_target.position + offset;
        Vector3 direction = end - start;
        Debug.DrawRay(start, direction, Color.red);
        if (Physics.Raycast(start, direction, out hit, direction.magnitude, obstacleMask)) {
            bool los_to_target = hit.transform == ctrl_target;
            // Debug.Log($"seeing {hit}");
            return los_to_target;
        }
        Debug.Log("I SEE NOTHING!");
        return false;
    }

    protected override void PreUpdate() {
        seeing_target = LineOfSightToTarget();
    }
    
    protected override void PostUpdate() {
        saw_target = LineOfSightToTarget();
    }    

    /////////////////////////////
    /// DEBUG FIELDS ////////////
    /////////////////////////////
    
    public bool debug_seeing_target, debug_saw_target;

    protected override void SetDebugData() {
        base.SetDebugData();
        debug_seeing_target = seeing_target;
        debug_saw_target = saw_target;
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
    waypoint
}