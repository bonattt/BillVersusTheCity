
using System;
using System.Collections.Generic;

using UnityEngine;


public class ClearEnemiesCondition : AbstractLevelCondition {
    
    public override bool ConditionMet() {
        return EnemiesManager.inst.remaining_enemies <= remaining_enemies_target;
    }
}