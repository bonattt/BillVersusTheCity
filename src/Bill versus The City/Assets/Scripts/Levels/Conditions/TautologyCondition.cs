
using System;
using System.Collections.Generic;

using UnityEngine;


public class TautologyCondition : AbstractLevelCondition {
    public bool condition_met = true;
    public override bool ConditionMet() {
        return condition_met;
    }
}