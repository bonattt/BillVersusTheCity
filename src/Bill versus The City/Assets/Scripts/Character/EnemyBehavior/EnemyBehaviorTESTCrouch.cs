using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyBehaviorTESTCrouch : EnemyBehavior
{   

    protected override void InitializeBehaviorsDict()
    {
        // initializes the `behaviors` dictionary with values
        behaviors = new Dictionary<BehaviorMode, ISubBehavior>() {
            // agressive behaviors
            {BehaviorMode.engaged, new ShelterInPlaceBehavior()},
            // {BehaviorMode.retreating, new FleeToCoverBehavior(fights_when_cornered: true)},
            {BehaviorMode.retreating, new ShelterInPlaceBehavior()},
            {BehaviorMode.reserve, new ShelterInPlaceBehavior()},
            {BehaviorMode.searching, new ShelterInPlaceBehavior()},
            // passive behaviors
            {BehaviorMode.passive, new ShelterInPlaceBehavior()},
            {BehaviorMode.wondering, new ShelterInPlaceBehavior()},
            {BehaviorMode.patrol, new ShelterInPlaceBehavior()},
            // {BehaviorMode.suppressed, new FleeToCoverBehavior(fights_when_cornered: true)},
            {BehaviorMode.suppressed, new ShelterInPlaceBehavior()},
            // {BehaviorMode.routed, new FleeToCoverBehavior(fights_when_cornered: false)},
            {BehaviorMode.routed, new ShelterInPlaceBehavior()},
            {BehaviorMode.dead, new ShelterInPlaceBehavior()},
            {BehaviorMode.guard, new ShelterInPlaceBehavior()},
            {BehaviorMode.berserk, new ShelterInPlaceBehavior()},
        };
        if (sprint_to_chase)
        {
            behaviors[BehaviorMode.persuing] = new ShelterInPlaceBehavior();
        }
        else
        {
            behaviors[BehaviorMode.persuing] = new ShelterInPlaceBehavior();
        }
    }
}
