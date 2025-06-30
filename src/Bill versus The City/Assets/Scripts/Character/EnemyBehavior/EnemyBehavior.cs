using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour, IPlayerObserver, IReloadSubscriber
{
    public NavMeshAgentMovement controller;
    public BehaviorMode default_behavior = BehaviorMode.wondering;  // the behavior this unit will exhibit before the player is noticed

    [Tooltip("If true, the enemy will sprint when trying to get closer to the player, otherwise, they will shoot while closing distance.")]
    public bool sprint_to_chase = false;
    [Tooltip("if within this range, the enemy will switch to attack more aggressively.")]
    public float optimal_attack_range = 6f;
    public float max_attack_range {
        get {
            return perception.max_alert_vision_range;
        }
    }
    
    [Tooltip("NOTE: not all behaviors support initial_movement_target")]
    public bool use_initial_movement_target = true; 
    [Tooltip("NOTE: not all behaviors support initial_movement_target")]
    public Vector3 initial_movement_target = new Vector3(float.NaN, float.NaN, float.NaN);

    public bool always_use_cover_to_reload = false;
    public bool never_use_cover_to_reload = false;

    [Tooltip("Debug flag: if true, enemy will always stand still and do nothing.")]
    public bool lock_to_passive = false;
    public float _shooting_rate = 1f;
    private float _shooting_rate_variation = 0f;
    public const float SHOOTING_RATE_VARIATION = 0.5f;
    public float shooting_rate {
        get { 
            return _shooting_rate * GetSubBehavior().shooting_rate + _shooting_rate_variation; 
        }
    }

    //////////////////////////////
    /// Behavior controls ////////
    //////////////////////////////
    
    public CharCtrl ctrl_target;
    public bool ctrl_will_shoot = true;  // set by behavior, determines if the character will shoot if able
    public Vector3 ctrl_waypoint;  // arbitrary movement-target setable by behaviors
    public MovementTarget ctrl_move_mode = MovementTarget.stationary; // used by Behavior to instruct the controller how to move
    public AimingTarget ctrl_aim_mode = AimingTarget.target; // used by Behavior to instruct the controller how to aim
    public bool ctrl_sprint = false; // used by Behavior to instruct the controller to sprint
    public bool ctrl_start_reload = false; // used by Behavior to instruct the controller to sprint
    public bool ctrl_cancel_reload = false; // used by Behavior to instruct the controller to sprint
    public float ctrl_shooting_rate = 0.75f;

    //////////////////////////////
    

    [SerializeField]
    private bool _needs_reload = false;
    public bool needs_reload {
        get { return _needs_reload; }
        set { 
            _needs_reload = value; 
            if(! _needs_reload) {
                _reload_behavior = null;
            }
        }
    }

    private ISubBehavior[] possible_reload_behaviors = new ISubBehavior[]{
        new ReloadFromCoverBehavior(), 
        new ReloadFromStandingBehavior(),
    };
    public ISubBehavior RandomReloadBehavior() {
        if (always_use_cover_to_reload) { return new ReloadFromCoverBehavior(); }
        if (never_use_cover_to_reload) { return new ReloadFromStandingBehavior(); }
        int r = Random.Range(0, possible_reload_behaviors.Length + 1); 
        return possible_reload_behaviors[r];
    }

    protected BehaviorMode previous_behavior_mode;
    private BehaviorMode _behavior_mode;
    public BehaviorMode behavior_mode { 
        get {
            return _behavior_mode;
        } 
        protected set {
            previous_behavior_mode = _behavior_mode;
            _behavior_mode = value;
        }
    }

    private PlayerCombat player_combat;
    private EnemyThreatTracking threat_tracking;
    private IReloadManager reload_manager;

    private Dictionary<BehaviorMode, ISubBehavior> behaviors;
    private ISubBehavior _reload_behavior = null;
    public ISubBehavior reload_behavior {
        get {
            if(_reload_behavior == null) {
                _reload_behavior = RandomReloadBehavior();
            }
            return _reload_behavior; 
        }
    }


    private EnemyPerception _perception = null;
    public EnemyPerception perception {
        get {
            if (_perception == null) {
                _perception = GetComponent<EnemyPerception>();
            }
            return _perception;
        }
    }

    private void InitializeBehaviorsDict()
    {
        // initializes the `behaviors` dictionary with values
        behaviors = new Dictionary<BehaviorMode, ISubBehavior>() {
            // agressive behaviors
            {BehaviorMode.engaged, new StandAndShootBehavior()},
            {BehaviorMode.retreating, new FleeToCoverBehavior(fights_when_cornered: true)},
            {BehaviorMode.searching, new SearchingBehavior(use_initial_movement_target, initial_movement_target)},
            // passive behaviors
            {BehaviorMode.passive, new StationaryBehavior()},
            {BehaviorMode.wondering, new WonderingBehavior(this)},
            {BehaviorMode.patrol, GetComponent<PatrolBehavior>()},
            // {BehaviorMode.suppressed, new FleeToCoverBehavior(fights_when_cornered: true)},
            {BehaviorMode.suppressed, new FleeFromThreatsBehavior(fights_when_cornered: true)},
            {BehaviorMode.routed, new FleeToCoverBehavior(fights_when_cornered: false)},
            {BehaviorMode.dead, new DeadBehavior()},
        };
        if (sprint_to_chase)
        {
            behaviors[BehaviorMode.persuing] = new ChasePlayerSprintBehavior();
        }
        else
        {
            behaviors[BehaviorMode.persuing] = new ChasePlayerBehavior();
        }
    }

    public void Kill() {
        behavior_mode = BehaviorMode.dead;
    }

    public void NewPlayerObject(PlayerCombat new_player) {
        player_combat = new_player;
    }
    
    void Start()
    {
        _shooting_rate_variation = Random.Range(-SHOOTING_RATE_VARIATION/2, SHOOTING_RATE_VARIATION/2);
        InitializeBehaviorsDict();
        player_combat = PlayerCharacter.inst.GetPlayerCombat(this);
        reload_manager = GetComponent<IReloadManager>();
        reload_manager.Subscribe(this);
        threat_tracking = GetComponent<EnemyThreatTracking>();

        behavior_mode = default_behavior; // sets previous_behavior
        behavior_mode = default_behavior;
        if (controller == null) {
            controller = GetComponent<NavMeshAgentMovement>();
        }
    }

    void Update()
    {
        if (!LevelConfig.inst.combat_enabled) { return; } // don't initiate actions while combat is disabled
        SetBehaviorMode();
        
        SetDefaultBehaviorPatterns();
        GetSubBehavior().SetControllerFlags(this, player_combat.movement);
        SetDebug();

        /// actively control EnemyController (new refactor)
        if (! controller.is_active) { return; } 
        if (ReloadInput()) {
            controller.StartReload();
        } else if (CancelReloadInput()) {
            controller.CancelReload();
        }
        else if (AttackInput() && controller.attack_controller.CanAttack()) {
            controller.TryToAttack();
        }
        controller.MoveCharacter(GetMoveTarget(), GetLookDirection(), sprint:ctrl_sprint, crouch:false);
    }

    private Vector3 GetMoveTarget() {
        switch (ctrl_move_mode) {
            case MovementTarget.target:
                return ctrl_target.transform.position;

            case MovementTarget.waypoint:
                return ctrl_waypoint;

            case MovementTarget.stationary:
                return controller.transform.position;

            default:
                Debug.Log($"unknown move mode '{ctrl_move_mode}'");
                return controller.transform.position;
        }
    }

    private Vector3 GetLookDirection() {
        return controller.DirectionFromLookTarget(GetLookTarget());
    }

    private Vector3 GetLookTarget() {
        switch (ctrl_aim_mode) {
            case AimingTarget.movement_direction:
                return controller.GetVelocity();

            case AimingTarget.target:
                return ctrl_target.transform.position;

            case AimingTarget.waypoint:
                return ctrl_waypoint;

            case AimingTarget.stationary:
                return InputSystem.NULL_POINT;

            default:
                Debug.Log($"unknown aim mode '{ctrl_aim_mode}'");
                return ctrl_waypoint;
        }
    }

    
    public bool AttackInput() {
        // Debug.Log($"{Time.time} >= {this.last_attack_time} + {ctrl_shooting_rate}: {Time.time >= (this.last_attack_time + ctrl_shooting_rate)}");
        if (perception.seeing_target && ctrl_will_shoot && !controller.reloading) {
            if (perception.saw_target_last_frame) {
                if (controller.use_full_auto) { return true; }
                return Time.time >= (controller.last_attack_time + ctrl_shooting_rate);
            }
            else {
                // start countdown to shoot once target is seen
                controller.last_attack_time = Time.time;
            }
        }
        return false;
    }
    
    public bool ReloadInput() {
        return ctrl_start_reload && !controller.reloading; 
    }

    public bool CancelReloadInput() {
        return ctrl_cancel_reload && controller.reloading;
    }

    void OnDestroy() {
        PlayerCharacter.inst.UnsubscribeFromPlayer(this);
        GetComponent<IReloadManager>().Subscribe(this);
    }
    
    public void StartReload(IReloadManager manager, IFirearm weapon) {
        // do nothing
    }
    public void ReloadFinished(IReloadManager manager, IFirearm weapon) {
        // if reload is finished, clear `needs_reload` if the weapon is fully loaded, otherwise do not clear it.
        if (perception.seeing_target) {
            needs_reload = weapon.current_ammo == 0;
        } else {
            needs_reload = weapon.current_ammo < weapon.ammo_capacity;
        }
    }
    public void ReloadCancelled(IReloadManager manager, IFirearm weapon) {
        // if reload is canceled, but the weapon has no ammo still, leave `needs_reload` as true, otherwise clear it.
        needs_reload = weapon.current_ammo == 0;
    }

    protected void SetDefaultBehaviorPatterns() {
        // sets control fields that are standardized to properties, and some default flags, which can be overwriten by the actual behavior
        ctrl_shooting_rate = this.shooting_rate;
        ctrl_start_reload = false;
        ctrl_cancel_reload = false;
        ctrl_sprint = false;
    }

    public float DistanceToTarget() {
        // TODO --- flatten the Y co-ordinates
        return Vector3.Distance(controller.transform.position, PlayerCharacter.inst.player_transform.position);  
    }

    protected void TryToStartReload() {
        if (needs_reload) { return; }
        if (controller.current_firearm != null) {
            // can only reload if using a firearm
            needs_reload = controller.current_firearm.current_ammo == 0;
        }
    }

    protected ISubBehavior GetSubBehavior() {
        TryToStartReload();
        if (behavior_mode != previous_behavior_mode) {
            behaviors[previous_behavior_mode].EndBehavior(this);
            behaviors[behavior_mode].AssumeBehavior(this);
        } 
        if (needs_reload && behavior_mode != BehaviorMode.routed && behavior_mode != BehaviorMode.suppressed) {
            Debug.LogWarning($"{gameObject.name} using reload behavior! (behavior_mode = {behavior_mode}), needs_reload: {needs_reload}, reload_behavior is null {_reload_behavior == null}");
            return reload_behavior;
        } else if (!needs_reload && controller.reloading) {
            controller.CancelReload();
        }
        return behaviors[behavior_mode];
    }

    protected void SetBehaviorMode() {
        if (lock_to_passive) {
            behavior_mode = BehaviorMode.passive;
            return;
        }

        // routed is permanent, and the enemy will run away forever
        else if (behavior_mode == BehaviorMode.routed || behavior_mode == BehaviorMode.dead) { 
            return; 
        }
        else if (threat_tracking.is_suppressed) {
            behavior_mode = BehaviorMode.suppressed;
            return;
        }
        switch(perception.state) {
            case PerceptionState.seeing:
                float dist = DistanceToTarget();
                if (controller.seeing_target && dist < optimal_attack_range) {
                    behavior_mode = BehaviorMode.engaged;
                } else {
                    behavior_mode = BehaviorMode.persuing;
                }
                break;

            case PerceptionState.alert:
                behavior_mode = BehaviorMode.persuing;
                break;

            case PerceptionState.searching:
                behavior_mode = BehaviorMode.searching;
                break;

            case PerceptionState.unaware:
                behavior_mode = default_behavior;
                break;

            default:
                Debug.LogError($"unhandled PerceptionState {perception.state}");
                break;
        }
    }
    
    public static bool ValidVectorTarget(Vector3 target) {
        return ValidVectorFloat(target.x) && ValidVectorFloat(target.y) && ValidVectorFloat(target.z);
    }

    private static bool ValidVectorFloat(float n) {
        return !float.IsNaN(n) && float.IsFinite(n);
    }
    
    public float debug_distance_to_target, debug_distance_to_waypoint;
    public string debug_sub_behavior;
    public string debug_sub_behavior_message;
    public BehaviorMode debug_behavior_mode;
    public void SetDebug() {
        debug_behavior_mode = behavior_mode;
        debug_distance_to_target = DistanceToTarget();
        // debug_distance_to_waypoint = WaypointSystem.FlattenedDistance(transform.position, move)
        ISubBehavior sub_behavior = GetSubBehavior();
        debug_sub_behavior = $"{sub_behavior}";
        debug_sub_behavior_message = sub_behavior.GetDebugMessage(this);
    }
}

public enum BehaviorMode {
    engaged,  // enemy is aware of the player, and is in optimal combat range
    persuing, // enemy is aware of the player, but is beyond optimal combat range
    retreating,  // enemey is aware of the player, but is too close for optimal combat range
    searching, // enemy is aware of the player, but doesn't know where he is.
    // passive behaviors:
    passive,  // enemy doesn't know the player exists
    patrol,  // enemy will patrol through a sequence of pre-set points
    wondering,  // enemy is unaware of the player, and wonders idly 
    routed,  // enemy is paniced and will run away forever. (probably mostly for testing retreat behaviors and pathfinding)
    suppressed, // enemy will retreat to cover until recovering from suppressed
    dead // enemy is dead

}
