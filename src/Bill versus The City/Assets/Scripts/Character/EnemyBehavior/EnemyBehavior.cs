using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour, IPlayerObserver
{
    public EnemyController controller;
    public BehaviorMode default_behavior = BehaviorMode.wondering;  // the behavior this unit will exhibit before the player is noticed

    public float optimal_attack_range = 6f;
    public float max_attack_range = 11f;

    public float _shooting_rate = 1f;
    public float shooting_rate {
        get { 
            return _shooting_rate * GetSubBehavior().shooting_rate; 
        }
    }

    public BehaviorMode behavior_mode { get; private set; }

    private PlayerCombat player_combat;

    private Dictionary<BehaviorMode, ISubBehavior> behaviors;

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
            {BehaviorMode.retreating, new StationaryBehavior()},  // TODO --- placeholder behavior value
            {BehaviorMode.searching, new SearchingBehavior()},
            // passive behaviors
            {BehaviorMode.passive, new StationaryBehavior()},
            {BehaviorMode.wondering, new WonderingBehavior(this)},
            {BehaviorMode.patrol, GetComponent<PatrolBehavior>()},
            {BehaviorMode.routed, new FleeToCoverBehavior()},
        };
    }

    public void NewPlayerObject(PlayerCombat new_player) {
        player_combat = new_player;
    }
    
    void Start()
    {
        InitializeBehaviorsDict();
        player_combat = PlayerCharacter.inst.GetPlayerCombat(this);

        behavior_mode = default_behavior;
        if (controller == null) {
            controller = GetComponent<EnemyController>();
        }
    }

    void Update()
    {
        SetBehaviorMode();
        
        GetSubBehavior().SetControllerFlags(this, player_combat.movement);
        SetStandardBehaviorPatterns();
        SetDebug();
    }

    void OnDestroy() {
        PlayerCharacter.inst.UnsubscribeFromPlayer(this);
    }

    protected void SetStandardBehaviorPatterns() {
        // sets control fields that are standardized to properties
        controller.ctrl_shooting_rate = this.shooting_rate;
    }

    public float DistanceToTarget() {
        // TODO --- flatten the Y co-ordinates
        return Vector3.Distance(controller.transform.position, PlayerCharacter.inst.player_transform.position);  
    }

    protected ISubBehavior GetSubBehavior() {
        Debug.Log($"behavior {behavior_mode} => {behaviors[behavior_mode]}"); // TODO --- remove debug
        return behaviors[behavior_mode];
    }

    protected void SetBehaviorMode() {
        // routed is permanent, and the enemy will run away forever
        if (behavior_mode == BehaviorMode.routed) { 
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
    
    public float debug_distance_to_target;
    public string debug_sub_behavior;
    public BehaviorMode debug_behavior_mode;
    public void SetDebug() {
        debug_behavior_mode = behavior_mode;
        debug_distance_to_target = DistanceToTarget();
        debug_sub_behavior = $"{GetSubBehavior()}";
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

}
