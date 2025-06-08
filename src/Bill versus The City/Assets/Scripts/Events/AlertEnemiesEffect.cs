
using UnityEngine;

public class AlertEnemies : AbstractInteractionGameEvent {

    protected override void Effect() {
        EnemiesManager.inst.AlertAll();
    }
}