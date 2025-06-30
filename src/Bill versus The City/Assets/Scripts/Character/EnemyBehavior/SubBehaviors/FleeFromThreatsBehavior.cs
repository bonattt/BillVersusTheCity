using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class FleeFromThreatsBehavior : ISubBehavior  {

    public float shooting_rate { get { return 1f; }}

    public float recalculation_period = 0.1f;  // controls how frequently the behavior recalculates it's destination
    private float last_calculation_at = -1f;

    private bool is_cornered = false; // if the enemy becomes cornered, it will start trying to fight

    private const float towards_player_threshold = 0.75f; // 
    
    public bool fights_when_cornered = false;
    public bool reload = false;
    public FleeFromThreatsBehavior(bool fights_when_cornered, bool reload=false) {
        this.fights_when_cornered = fights_when_cornered;
        this.reload = reload;
    }
    private EnemyThreatTracking threat_tracking = null;
    
    public void SetControllerFlags(EnemyBehavior parent, ManualCharacterMovement player) {
        // initialize threat_tracking, if it's not already initialized
        if (threat_tracking == null) { threat_tracking = parent.GetComponent<EnemyThreatTracking>(); }

        // recalculate destination
        if (last_calculation_at + recalculation_period <= Time.time) {
            parent.ctrl_waypoint = RecalculateDestination(parent, player);
        }

        parent.ctrl_target = player;
        parent.ctrl_move_mode = MovementTarget.waypoint;
        if (is_cornered) {
            parent.ctrl_sprint = false;
            parent.ctrl_will_shoot = fights_when_cornered;
            parent.ctrl_aim_mode = AimingTarget.target;
        } else {
            parent.ctrl_sprint = true;
            parent.ctrl_will_shoot = false;
            parent.ctrl_aim_mode = AimingTarget.movement_direction;
        }
        Color color = is_cornered ? Color.red : Color.green;
        Debug.DrawLine(parent.controller.transform.position, parent.ctrl_waypoint, color);
    }

    // private void UpdateIsCornered(EnemyBehavior parent, ManualCharacterMovement player) {
    //     if (!is_cornered) {
    //         is_cornered = WillBecomeCornered(parent, player);
    //     }
    //     is_cornered = WillStayCornered(parent, player);
    // }

    private Vector3 RecalculateDestination(EnemyBehavior parent, ManualCharacterMovement player) {
        last_calculation_at = Time.time;
        // UpdatePlayerRaycast(parent, player);
        // UpdateIsCornered(parent, player);
        // if (!has_los_to_player) {
        //     // player has no LoS, wherever you are is good
        //     return parent.controller.transform.position;
        // }

        // Vector3 start_pos = parent.controller.transform.position;
        // if (is_cornered) {
        List<ITrackedProjectile> threats = BulletTracking.inst.PlayerBullets().ToList();
        if (threats.Count == 0) {
            return NavMeshUtils.DestinationAwayFromPosition(parent, player.transform.position);
        } else {
            Vector3 threat_centroid = GetThreatCentroid(parent.transform.position, threats);
            return NavMeshUtils.DestinationAwayFromPosition(parent, threat_centroid);
        }

            // Debug.DrawLine(start_pos, parent.ctrl_waypoint, Color.red);
        // } 
        // else {
        //     Vector3 cover_from = player.transform.position;
        //     Transform dest = WaypointSystem.inst.GetClosestCoverPosition(start_pos, cover_from);
        //     return dest.position;
        //     // Debug.DrawLine(start_pos, parent.ctrl_waypoint, Color.green);
        // }
    }

    public static Vector3 GetThreatCentroid(Vector3 position, IEnumerable<ITrackedProjectile> threats) {
        float total_weight = 0f;
        float sum_x = 0f;
        float sum_y = 0f;
        float sum_z = 0f;
        foreach (ITrackedProjectile p in threats) {
            float weight = Vector3.Distance(position, p.location.position) * p.threat_level;
            total_weight += weight;
            sum_x += p.location.position.x * weight;
            sum_y += p.location.position.y * weight;
            sum_z += p.location.position.z * weight;
        }
        Vector3 final_position = new Vector3(sum_x, 0f, sum_z) / total_weight;
        return final_position;
    }

    private bool has_los_to_player = false; // cached 
    private void UpdatePlayerRaycast(EnemyBehavior parent, ManualCharacterMovement player) {
        has_los_to_player = parent.perception.seeing_target;
    }
    public void StartBehavior(EnemyBehavior parent) { 
        Debug.LogWarning($"start behavior {this}");
        has_los_to_player = false;
    }

    private bool WillStayCornered(EnemyBehavior parent, ManualCharacterMovement player) {
        return is_cornered && has_los_to_player; 
    }

    private bool WillBecomeCornered(EnemyBehavior parent, ManualCharacterMovement player) {
        if (is_cornered) { return false; } // already cornered, don't BECOME cornered
        
        Vector3 travel_direction = parent.controller.nav_mesh_agent.velocity.normalized;
        Vector3 toward_player = (player.transform.position - parent.transform.position).normalized;

        float dot = Vector3.Dot(toward_player, travel_direction);
        return dot >= towards_player_threshold;
    }


    // private static Vector3 DestinationAwayFromPlayer(EnemyBehavior parent, ManualCharacterMovement player) {
    //     Vector3 toward_player = (player.transform.position - parent.transform.position).normalized;
    //     Debug.DrawRay(parent.transform.position, toward_player, Color.red);

    //     Vector3 raycast_start = new Vector3(parent.transform.position.x, 0.5f, parent.transform.position.z);
    //     Vector3 raycast_target = new Vector3(parent.transform.position.x, 0.5f, parent.transform.position.z);
    //     Vector3 direction = (raycast_target - raycast_start);
    //     float magnitude = direction.magnitude;
    //     RaycastHit hit;
    //     if (Physics.Raycast(raycast_start, direction.normalized, out hit, magnitude)) {
    //         Vector3 destination = hit.point - (direction.normalized / 2);
    //         return new Vector3(destination.x, 0, destination.z);
    //     } else {
    //         return parent.transform.position + (-toward_player * 3);
    //     }
    // }


    public string GetDebugMessage(EnemyBehavior parent) {
        ManualCharacterMovement player = PlayerCharacter.inst.player_transform.gameObject.GetComponent<ManualCharacterMovement>();
        return $"is_cornered: {is_cornered}, destination: {parent.ctrl_waypoint}";
    }
}