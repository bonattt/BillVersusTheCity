
using System.Reflection;
using UnityEngine;

public class SetCombatEnabledEvent : MonoBehaviour, IGameEventEffect, IInteractionEffect {

    public bool enable_combat = true;
    public LevelConfig level;

    public void ActivateEffect() {
        if (level == null) {
            level = LevelConfig.inst;
        }
        level.combat_enabled = enable_combat;
        Debug.LogWarning($"Combat enabled? {enable_combat}"); // TODO --- remove debug
    }

    public void Interact(GameObject actor) {
        ActivateEffect();
    }
}