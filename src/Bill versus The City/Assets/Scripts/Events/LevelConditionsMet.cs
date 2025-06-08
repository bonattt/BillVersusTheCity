using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelConditionsMet : AbstractInteractionGameEvent
{
    protected override void Effect() {
        LevelConfig.inst.LevelObjectivesCleared();
    }
}
