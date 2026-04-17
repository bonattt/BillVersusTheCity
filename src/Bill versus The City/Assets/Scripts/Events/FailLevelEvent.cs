using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailLevelEvent : AbstractInteractionGameEvent
{
    public LevelFailureReason failure_reson = LevelFailureReason.none;
    protected override void Effect() {
        LevelConfig.inst.FailLevel(failure_reson);
    }
}
