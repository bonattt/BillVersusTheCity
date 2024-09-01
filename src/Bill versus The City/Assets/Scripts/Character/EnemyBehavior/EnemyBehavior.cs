using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public PlayerMovement player;
    public EnemyController controller;
    
    public BehaviorMode behavior_mode { get; private set; }

    private Dictionary<BehaviorMode, ISubBehavior> behaviors = new Dictionary<BehaviorMode, ISubBehavior>() {
        {BehaviorMode.aggressive, new ChasePlayerBehavior()},
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
        controller.ctrl_target = player.transform;
        behaviors[behavior_mode].SetControllerFlags(this);
    }

    protected void SetBehaviorMode() {
        if (controller.seeing_target) {
            behavior_mode = BehaviorMode.aggressive;
        }
    }
}

public enum BehaviorMode {
    aggressive,
    passive
}
