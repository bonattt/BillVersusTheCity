using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Scripting.APIUpdating;

[MovedFrom(true, null, null, "NavMeshAgentMovement")]
public class EnemyNavMeshMovement : SimpleNavMeshAgentMovement
{
    public ManagedEnemyState managed_enemy_state;
    public float shoot_inaccuracy = 1f;
    public LayerMask obstacleMask;

    public override bool is_enemy { get => true; }
    public bool saw_target { get { return _perception.saw_target_last_frame; } } // saw the target last frame
    public bool seeing_target { get { return _perception.seeing_target; }  } // seeing the target last frame

    public bool use_full_auto = false;

    private EnemyPerception _perception;
    public EnemyPerception perception => _perception;
    private EnemyBehavior behavior;

    protected override void Start() {
        base.Start();
        _perception = GetComponent<EnemyPerception>();
        behavior = GetComponent<EnemyBehavior>();
        EnemiesManager.inst.AddEnemy(this);
    }
    
    protected override void Update() {
        base.Update();
        if (!is_active) { return; }
        UpdateReload();
    }

    public override Vector3 GetShootVector() {
        if (behavior.ctrl_shoot_flat) {
            return base.GetShootVector();
        } else {
            return ShootTarget() - attack_controller.shoot_from.position; //  DirectionFromLookTarget(ShootTarget());
        }
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
        MetricSystem.instance.IncrimentMetric("enemies_killed", 1);
        behavior.Kill();
    }

    public override void DelayedOnDeath(ICharacterStatus status) {
        base.DelayedOnDeath(status);
        EnemiesManager.inst.KillEnemy(this);
    }
    
    public override void FlashBangHit(Vector3 flashbang_position, float intensity) {
        perception.FlashBangHit(flashbang_position, intensity);
    }

}

public enum MovementTarget {
    stationary,
    target,
    waypoint, 
    random
}
