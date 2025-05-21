
using UnityEngine;

public class IncrimentObjectiveOverride : MonoBehaviour, IGameEventEffect, IInteractionEffect {
    // event that incriment's the level's objective display override when triggered
    public LevelConfig level;

    void Start() {
        if (level == null) {
            level = LevelConfig.inst;
        }
    }

    public void Interact(GameObject actor) {
        ActivateEffect();
    }

    public void ActivateEffect() {
        level.IncrimentObjectiveDisplay();
    }
}