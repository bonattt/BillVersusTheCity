using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelConditionsMet : MonoBehaviour, IGameEventEffect, IInteractionEffect
{
    public void Interact(GameObject actor) {
        ActivateEffect();
    }
    
    public void ActivateEffect() {
        LevelConfig.inst.CompleteLevelObjectives();
    }
}
