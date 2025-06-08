
using System.Reflection;
using UnityEngine;

public class SetCombatEnabledEvent : AbstractInteractionGameEvent {

    public bool enable_combat = true;
    public LevelConfig level;

    protected override void Effect() {
        if (level == null) {
            level = LevelConfig.inst;
        }
        level.combat_enabled = enable_combat;
        Debug.LogWarning($"Combat enabled? {enable_combat}"); // TODO --- remove debug
    }
}