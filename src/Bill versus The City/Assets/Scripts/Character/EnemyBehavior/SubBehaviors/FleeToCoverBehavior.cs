using UnityEngine;
using UnityEngine.AI;

public class FleeToCoverBehavior : ISubBehavior  {

    public float shooting_rate { get { return 1f; }}

    public float recalculation_period = 0.1f;  // controls how frequently the behavior recalculates it's destination
    private float last_calculation_at = -1f;

    private bool is_cornered = false; // if the enemy becomes cornered, it will start trying to fight

    private const float towards_player_threshold = 0.75f; // 
    
    public bool fights_when_cornered = false;
    public bool reload = false;
    public FleeToCoverBehavior(bool fights_when_cornered, bool reload=false) {
        Debug.LogWarning("FleeToCoverBehavior is OBSOLETE, use FleeFromThreatsBehavior"); // TODO --- remove this code
        this.fights_when_cornered = fights_when_cornered;
        this.reload = reload;
    }
    
    public void SetControllerFlags(EnemyBehavior parent, ManualCharacterMovement player) {
        // parent.ctrl_waypoint = new Vector3(0, 0, 0);

        // recalculate destination
        if (last_calculation_at + recalculation_period <= Time.time) {
            parent.ctrl_waypoint = RecalculatePath(parent, player);
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

    private void UpdateIsCornered(EnemyBehavior parent, ManualCharacterMovement player) {
        if (!is_cornered) {
            is_cornered = WillBecomeCornered(parent, player);
        }
        is_cornered = WillStayCornered(parent, player);
    }

    private Vector3 RecalculatePath(EnemyBehavior parent, ManualCharacterMovement player) {
        last_calculation_at = Time.time;
        UpdatePlayerRaycast(parent, player);
        UpdateIsCornered(parent, player);
        if (!has_los_to_player) {
            // player has no LoS, wherever you are is good
            return parent.controller.transform.position;
        }

        Vector3 start_pos = parent.controller.transform.position;
        if (is_cornered) {
            Vector3 destination = NavMeshUtils.DestinationAwayFromPosition(parent, player.transform.position);
            return destination;
            // Debug.DrawLine(start_pos, parent.ctrl_waypoint, Color.red);
        } 
        else {
            Vector3 cover_from = player.transform.position;
            Transform dest = WaypointSystem.inst.GetClosestCoverPosition(start_pos, cover_from);
            return dest.position;
            // Debug.DrawLine(start_pos, parent.ctrl_waypoint, Color.green);
        }
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

    public string GetDebugMessage(EnemyBehavior parent) {
        ManualCharacterMovement player = PlayerCharacter.inst.player_transform.gameObject.GetComponent<ManualCharacterMovement>();
        return $"is_cornered: {is_cornered}, destination: {parent.ctrl_waypoint}";
    }
}