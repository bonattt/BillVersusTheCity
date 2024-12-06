using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public EnemyController controller;

    public float optimal_attack_range = 6f;
    public float max_attack_range = 11f;

    public float _shooting_rate = 1f;
    public float shooting_rate {
        get { 
            return _shooting_rate * GetSubBehavior().shooting_rate; 
        }
    }
    
    public BehaviorMode behavior_mode { get; private set; }

    private Dictionary<BehaviorMode, ISubBehavior> behaviors = new Dictionary<BehaviorMode, ISubBehavior>() {
        {BehaviorMode.engaged, new StandAndShootBehavior()},
        {BehaviorMode.persuing, new ChasePlayerBehavior()},
        {BehaviorMode.passive, new StationaryBehavior()},
        {BehaviorMode.retreating, new StationaryBehavior()},  // TODO --- placeholder behavior value
        {BehaviorMode.searching, new SearchingBehavior()} 
    };

    private EnemyPerception _perception = null;
    public EnemyPerception perception {
        get {
            if (_perception == null) {
                _perception = GetComponent<EnemyPerception>();
            }
            return _perception;
        }
    }
    
    void Start()
    {
        behavior_mode = BehaviorMode.passive;
        if (controller == null) {
            controller = GetComponent<EnemyController>();
        }
    }

    void Update()
    {
        SetBehaviorMode();
        controller.ctrl_target = PlayerCharacter.inst.player_transform;
        GetSubBehavior().SetControllerFlags(this);
        SetStandardBehaviorPatterns();
        SetDebug();
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
        return behaviors[behavior_mode];
    }

    protected void SetBehaviorMode() {
        if (behavior_mode == BehaviorMode.passive) {
            // do nothing (stay passive)
            if (controller.seeing_target) {
                behavior_mode = BehaviorMode.engaged;
            }
        }
        else { 
            if (perception.knows_player_location) {
                float dist = DistanceToTarget();
                if (controller.seeing_target && dist < optimal_attack_range) {
                    behavior_mode = BehaviorMode.engaged;
                } else {
                    behavior_mode = BehaviorMode.persuing;
                }
            } else {
                behavior_mode = BehaviorMode.searching;
            }
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
    passive,  // enemy doesn't know the player exists
    searching // enemy is aware of the player, but doesn't know where he is.
}
