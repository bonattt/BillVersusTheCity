
using UnityEngine;

public class IncrimentObjectiveOverride : AbstractInteractionGameEvent {
    // event that incriment's the level's objective display override when triggered
    public LevelConfig level;

    protected override void Effect() {
        level.IncrimentObjectiveDisplay();
    }
}