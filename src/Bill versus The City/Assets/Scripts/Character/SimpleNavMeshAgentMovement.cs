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
    
    public Vector3 debug__look_direction;
    
    protected override void Update() {
        base.Update();
        if (!is_active) { return; } // do nothing while controller disabled
        HandleAnimation();
    }
    public override void MoveCharacter(Vector3 move_target, Vector3 look_direction, bool sprint = false, bool crouch = false, bool walk=false) {
        SetCharacterLookDirection(look_direction);
        if (time_scale_setting == TimeScaleSetting.unscaled_time) { Debug.LogError("paused movement is not implemented for NavMeshAgents!"); }
        // Debug.DrawRay(transform.position + Vector3.up, look_direction, Color.yellow);
        if (walk) { Debug.LogError("NavMeshAgent walk=true not implemented!"); } // TODO --- remove debug
        debug__look_direction = look_direction;

        if (sprint && crouch) { Debug.LogError($"{gameObject.name} cannot spint and crouch at the same time!!"); }
        _is_sprinting = sprint;
        if (sprint) {
            nav_mesh_agent.speed = walk_speed * sprint_multiplier;
            crouch = false;
        } else if (crouch) {
            Debug.LogWarning("CROUCH!");
            nav_mesh_agent.speed = walk_speed * crouched_speed_multiplier;
        } else {
            nav_mesh_agent.speed = walk_speed;
        }
        UpdateCrouch(crouch);

        if (is_hit_stunned) {
            nav_mesh_agent.SetDestination(transform.position);
        } else if (move_target != nav_mesh_agent.destination) {
            nav_mesh_agent.SetDestination(move_target);
        }
    }

    public override void TeleportTo(Vector3 new_position) {
        nav_mesh_agent.Warp(new_position);
    }

    public Vector3 MoveDirection() {
        return (nav_mesh_agent.nextPosition - transform.position).normalized;
    }
    
    public override Vector3 GetVelocity() {
        return MoveDirection() * movement_speed;
    }

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

    public override void OnDeath(ICharacterStatus status) {
        base.OnDeath(status);
        DisableCollision();
        nav_mesh_agent.enabled = false;
    }

    public override void OnDeathCleanup(ICharacterStatus status) {
        base.OnDeathCleanup(status);
        Destroy(gameObject);
    }

    public override void FlashBangHit(Vector3 flashbang_position, float intensity) {
        Debug.LogWarning($"flash bang hit: '{intensity}'");
    }
}

public enum AimingTarget {
    stationary,
    target,
    waypoint,
    movement_direction  // TODO --- not implemented
}