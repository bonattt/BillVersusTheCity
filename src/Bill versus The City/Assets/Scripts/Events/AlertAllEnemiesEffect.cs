
using UnityEngine;

public class AlertAllEnemiesEffect : AbstractInteractionGameEvent {

    protected override void Effect() {
        EnemiesManager.inst.AlertAll();
    }
}