using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

 public class SimpleNavMeshAgentMovement : CharCtrl
{
    protected bool _is_sprinting = false; // set in MoveCharacter
    public override bool is_sprinting
    {
        get
        {
            return _is_sprinting;
        }
    }

    public NavMeshAgent nav_mesh_agent;

    [Tooltip("if true, movement is temporarily overriden by choreography")]
    public bool choreography_movement = false;

    public override void SetPosition(Vector3 new_position) {
        nav_mesh_agent.Warp(new_position);
    }
    
    public Vector3 debug__look_direction;
    public override void MoveCharacter(Vector3 move_target, Vector3 look_direction, bool sprint = false, bool crouch = false) {
        // TODO --- crouch not implemented 
        SetCharacterLookDirection(look_direction);
        // Debug.DrawRay(transform.position + Vector3.up, look_direction, Color.yellow);
        debug__look_direction = look_direction;

        if (crouch) { Debug.LogWarning("enemy crouch not implemented!"); }
        _is_sprinting = sprint;
        if (sprint) {
            nav_mesh_agent.speed = walk_speed * sprint_multiplier;
        } else {
            nav_mesh_agent.speed = walk_speed;
        }

        if (is_hit_stunned) {
            nav_mesh_agent.SetDestination(transform.position);
        } else if (move_target != nav_mesh_agent.destination) {
            nav_mesh_agent.SetDestination(move_target);
        }
        HandleAnimation();
    }

    public Vector3 MoveDirection() {
        return (nav_mesh_agent.nextPosition - transform.position).normalized;
    }
    
    public override Vector3 GetVelocity() {
        return MoveDirection() * movement_speed;
    }

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

    public virtual Vector3 GetLookTarget() {
        Vector3 look_direction = nav_mesh_agent.destination - transform.position;
        return new Vector3(look_direction.x, 0, look_direction.z).normalized;
        // return nav_mesh_agent.de
    }

    protected void DisableCollision() {
        GetComponent<CapsuleCollider>().enabled = false;
    }
    protected void EnableCollision() {
        GetComponent<CapsuleCollider>().enabled = true;
    }
}

public enum AimingTarget {
    stationary,
    target,
    waypoint,
    movement_direction  // TODO --- not implemented
}