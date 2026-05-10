using UnityEngine;
using UnityEngine.AI;

public class FleeToCoverBehavior : ISubBehavior  {

    public float shooting_rate { get { return 1f; }}

    public float recalculation_period = 0.1f;  // controls how frequently the behavior recalculates it's destination
    private float last_calculation_at = -1f;

    private bool _is_cornered = false; // if the enemy becomes cornered, it will start trying to fight
    public bool is_cornered => _is_cornered;

    private const float towards_player_threshold = 0.75f; // 
    
    public bool fights_when_cornered = false;
    public bool reload = false;
    public FleeToCoverBehavior(bool fights_when_cornered, bool reload=false) {
        // Debug.LogWarning("FleeToCoverBehavior is OBSOLETE, use FleeFromThreatsBehavior"); // For now, at least, flee to cover isn't obsolete, but shouldn't be used as a top-level behavior. I am composing this behavior in ReloadFromCover
        this.fights_when_cornered = fights_when_cornered;
        this.reload = reload;
    }
    
    public void SetControllerFlags(EnemyBehavior parent, ManualCharacterMovement player) => 
        SetControllerFlags(parent, player, force_recaclulation:false);
    public void SetControllerFlags(EnemyBehavior parent, ManualCharacterMovement player, bool force_recaclulation = false) {
        if (NavMeshUtils.PointIsNaN(parent.ctrl_waypoint)) { force_recaclulation = true; } 
        // recalculate destination
        if (force_recaclulation || last_calculation_at + recalculation_period <= Time.time) {
            parent.ctrl_waypoint = RecalculatePath(parent, player);
            if (NavMeshUtils.PointIsNaN(parent.ctrl_waypoint)) {
                Debug.LogError($"waypoint set to NaN");
                # if UNITY_EDITOR
                Debug.Break();
                # endif
            }
        }

        parent.ctrl_target = player;
        parent.ctrl_move_mode = MovementTarget.waypoint;
        if (_is_cornered) {
            parent.ctrl_sprint = false;
            parent.ctrl_will_shoot = fights_when_cornered;
            parent.ctrl_aim_mode = AimingTarget.target;
        } else {
            parent.ctrl_sprint = true;
            parent.ctrl_will_shoot = false;
            parent.ctrl_aim_mode = AimingTarget.movement_direction;
        }
        Color color = _is_cornered ? Color.red : Color.green;
        Debug.DrawLine(parent.movement_script.transform.position, parent.ctrl_waypoint, color);
    }

    private void UpdateIsCornered(EnemyBehavior parent, ManualCharacterMovement player) {
        if (!_is_cornered) {
            _is_cornered = WillBecomeCornered(parent, player);
        }
        _is_cornered = WillStayCornered(parent, player);
    }

    private Vector3 RecalculatePath(EnemyBehavior parent, ManualCharacterMovement player) {
        last_calculation_at = Time.time;
        UpdatePlayerRaycast(parent, player);
        UpdateIsCornered(parent, player);
        if (!has_los_to_player) {
            return parent.movement_script.transform.position;
        }

        Vector3 start_pos = parent.movement_script.transform.position;
        if (_is_cornered) {
            Vector3 destination = NavMeshUtils.DestinationAwayFromPosition(parent, player.transform.position);
            if (NavMeshUtils.PointIsNaN(destination)) { Debug.LogError("CORNERED: DestinationAwayFromPosition is NaN"); }
            return destination;
        } 
        else {
            Vector3 cover_from = player.transform.position;
            Transform dest = GetBestCoverPosition(parent, start_pos, cover_from);
            if (dest == null) {
                Vector3 destination = NavMeshUtils.DestinationAwayFromPosition(parent, player.transform.position);
                if (NavMeshUtils.PointIsNaN(destination)) { Debug.LogError("not cornered: DestinationAwayFromPosition is NaN"); }
                return destination;
            }
            return dest.position;
        }
    }

    private Transform GetBestCoverPosition(EnemyBehavior parent, Vector3 start_pos, Vector3 cover_from) {
        // TODO --- this should return Vector3s, not Transforms
        NavMeshAgent agent = parent.movement_script.nav_mesh_agent;
        foreach(Transform destination in WaypointSystem.inst.GetCoverPositionsByDistance(start_pos, cover_from)) {
            NavMeshPath path = new NavMeshPath();
            bool success = agent.CalculatePath(destination.position, path);
            
            if (success && path.status == NavMeshPathStatus.PathComplete) { return destination; }
            else { continue; } 
        }
        Debug.LogWarning($"{parent.gameObject.name}.FleeToCoverBehavior cannot find a valid cover position!");
        return null; // no valid cover positions found
    }

    public bool has_los_to_player { get; private set; } // cached 
    private void UpdatePlayerRaycast(EnemyBehavior parent, ManualCharacterMovement player) {
        has_los_to_player = parent.perception.seeing_target;
    }
    public void StartBehavior(EnemyBehavior parent) { 
        Debug.LogWarning($"start behavior {this}");
        has_los_to_player = false;
    }

    private bool WillStayCornered(EnemyBehavior parent, ManualCharacterMovement player) {
        return _is_cornered && has_los_to_player; 
    }

    private bool WillBecomeCornered(EnemyBehavior parent, ManualCharacterMovement player) {
    //     if (is_cornered) { return false; } // already cornered, don't BECOME cornered
        
    //     Vector3 travel_direction = parent.controller.nav_mesh_agent.velocity.normalized;
    //     Vector3 toward_player = (player.transform.position - parent.transform.position).normalized;

    //     float dot = Vector3.Dot(toward_player, travel_direction);
    //     return dot >= towards_player_threshold;
        return NavMeshUtils.WillBecomeCornered(parent, player, towards_player_threshold);
    }

    public string GetDebugMessage(EnemyBehavior parent) {
        ManualCharacterMovement player = PlayerCharacter.inst.player_transform.gameObject.GetComponent<ManualCharacterMovement>();
        return $"FleeToCoverBehavior: is_cornered: {_is_cornered}, destination: {parent.ctrl_waypoint}";
    }
}