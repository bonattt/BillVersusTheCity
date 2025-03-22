using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour, IPlayerObserver, IReloadSubscriber
{
    public EnemyController controller;
    public BehaviorMode default_behavior = BehaviorMode.wondering;  // the behavior this unit will exhibit before the player is noticed

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

    [SerializeField]
    private bool _needs_reload = false;
    public bool needs_reload {
        get { return _needs_reload; }
        set { 
            Debug.Log("NEEDS RELOAD SET!"); // TODO --- remove debug
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

    private void InitializeBehaviorsDict() {
        // initializes the `behaviors` dictionary with values
        behaviors = new Dictionary<BehaviorMode, ISubBehavior>() {
            // agressive behaviors
            {BehaviorMode.engaged, new StandAndShootBehavior()},
            {BehaviorMode.persuing, new ChasePlayerBehavior()},
            {BehaviorMode.retreating, new FleeToCoverBehavior(fights_when_cornered: true)},
            {BehaviorMode.searching, new SearchingBehavior(use_initial_movement_target, initial_movement_target)},
            // passive behaviors
            {BehaviorMode.passive, new StationaryBehavior()},
            {BehaviorMode.wondering, new WonderingBehavior(this)},
            {BehaviorMode.patrol, GetComponent<PatrolBehavior>()},
            {BehaviorMode.suppressed, new FleeToCoverBehavior(fights_when_cornered: true)},
            {BehaviorMode.routed, new FleeToCoverBehavior(fights_when_cornered: false)},
            {BehaviorMode.dead, new DeadBehavior()},
        };
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
        // Debug.LogWarning($"{gameObject.name}.initial_search_target: {initial_movement_target}, use: {use_initial_movement_target}"); // TODO --- remove debug
        player_combat = PlayerCharacter.inst.GetPlayerCombat(this);
        reload_manager = GetComponent<IReloadManager>();
        reload_manager.Subscribe(this);
        threat_tracking = GetComponent<EnemyThreatTracking>();

        behavior_mode = default_behavior; // sets previous_behavior
        behavior_mode = default_behavior;
        if (controller == null) {
            controller = GetComponent<EnemyController>();
        }
    }

    void Update()
    {
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
        else if (AttackInput()) {
            controller.TryToAttack();
        }
        controller.Move();
    }

    
    public bool AttackInput() {
        // Debug.Log($"{Time.time} >= {this.last_attack_time} + {ctrl_shooting_rate}: {Time.time >= (this.last_attack_time + ctrl_shooting_rate)}");
        if (perception.seeing_target && controller.ctrl_will_shoot && !controller.reloading) {
            if (perception.saw_target_last_frame) {
                if (controller.use_full_auto) { return true; }
                return Time.time >= (controller.last_attack_time + controller.ctrl_shooting_rate);
            }
            else {
                // start countdown to shoot once target is seen
                controller.last_attack_time = Time.time;
            }
        }
        return false;
    }
    
    public bool ReloadInput() {
        return controller.ctrl_start_reload && !controller.reloading; 
        // return attack_controller.current_weapon.current_ammo == 0
        //     && !reloading
        //     && AttackInput();
    }

    public bool CancelReloadInput() {
        return controller.ctrl_cancel_reload && controller.reloading;
    }

    void OnDestroy() {
        PlayerCharacter.inst.UnsubscribeFromPlayer(this);
        GetComponent<IReloadManager>().Subscribe(this);
    }
    
    public void StartReload(IReloadManager manager, IWeapon weapon) {
        // do nothing
    }
    public void ReloadFinished(IReloadManager manager, IWeapon weapon) {
        // if reload is finished, clear `needs_reload` if the weapon is fully loaded, otherwise do not clear it.
        if (perception.seeing_target) {
            Debug.LogWarning($"FinishReload while seeing player! {weapon.current_ammo} {weapon.current_ammo != 0}"); // TODO --- remove debug
            needs_reload = weapon.current_ammo == 0;
        } else {
            Debug.LogWarning($"FinishReload in cover! {weapon.current_ammo} <= {weapon.ammo_capacity}"); // TODO --- remove debug
            needs_reload = weapon.current_ammo < weapon.ammo_capacity;
        }
        Debug.LogWarning($"FinishReload {gameObject.name}: still needs reload?? {needs_reload} ammo ({weapon.current_ammo} / {weapon.ammo_capacity})"); // TODO --- remove debug
    }
    public void ReloadCancelled(IReloadManager manager, IWeapon weapon) {
        // if reload is canceled, but the weapon has no ammo still, leave `needs_reload` as true, otherwise clear it.
        needs_reload = weapon.current_ammo == 0;
    }

    protected void SetDefaultBehaviorPatterns() {
        // sets control fields that are standardized to properties, and some default flags, which can be overwriten by the actual behavior
        controller.ctrl_shooting_rate = this.shooting_rate;
        controller.ctrl_start_reload = false;
        controller.ctrl_cancel_reload = false;
        controller.ctrl_sprint = false;
    }

    public float DistanceToTarget() {
        // TODO --- flatten the Y co-ordinates
        return Vector3.Distance(controller.transform.position, PlayerCharacter.inst.player_transform.position);  
    }

    protected ISubBehavior GetSubBehavior() {
        if (behavior_mode != previous_behavior_mode) {
            behaviors[previous_behavior_mode].EndBehavior(this);
            behaviors[behavior_mode].AssumeBehavior(this);
            Debug.Log($"{gameObject.name}.EnemyBehavior.GetSubBehavio({behavior_mode} -> {previous_behavior_mode})"); // TODO --- remove debug
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
