using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailLevelEvent : AbstractInteractionGameEvent
{
    protected override void Effect() {
        LevelConfig.inst.FailLevel();
    }
}
