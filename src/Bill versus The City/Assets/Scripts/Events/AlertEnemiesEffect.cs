
using UnityEngine;

public class AlertEnemies : MonoBehaviour, IGameEventEffect, IInteractionEffect {

    public void Interact(GameObject actor) {
        ActivateEffect();
    }
    
    public void ActivateEffect() {
        EnemiesManager.inst.AlertAll();
    }
}