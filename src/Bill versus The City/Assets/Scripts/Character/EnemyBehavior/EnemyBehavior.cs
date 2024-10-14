using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public EnemyController controller;

    public float optimal_attack_range = 6f;

    public float _shooting_rate = 1f;
    public float shooting_rate {
        get { 
            return _shooting_rate * GetSubBehavior().shooting_rate; 
        }
    }
    
    public BehaviorMode behavior_mode { get; private set; }

    private Dictionary<BehaviorMode, ISubBehavior> behaviors = new Dictionary<BehaviorMode, ISubBehavior>() {
        {BehaviorMode.aggressive, new StandAndShootBehavior()},
        {BehaviorMode.hunting, new ChasePlayerBehavior()},
        {BehaviorMode.passive, new StationaryBehavior()}
    };
    
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

    protected float DistanceToTarget() {
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
                behavior_mode = BehaviorMode.aggressive;
            }
        }
        else { 
            if (controller.seeing_target && DistanceToTarget() < optimal_attack_range) {
            behavior_mode = BehaviorMode.aggressive;
            } else {
                behavior_mode = BehaviorMode.hunting;
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
    aggressive,
    hunting,
    passive
}
