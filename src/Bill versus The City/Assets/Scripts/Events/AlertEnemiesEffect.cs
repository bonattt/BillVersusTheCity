
using UnityEngine;

public class AlertEnemies : MonoBehaviour, IGameEventEffect, IInteractionEffect {

    public GameObject effect_callback = null;

    public void Interact(GameObject actor) {
        ActivateEffect();
    }
    
    public void ActivateEffect() {
        EnemiesManager.inst.AlertAll();
        
        // if (_effect_callback == null) {
        //     // no callback, so do nothing
        //     Debug.LogWarning("no tutorial callback!"); // TODO --- remove debug
        // } else {
        //     // add callback to trigger when tutorial closed
        //     _effect_callback.ActivateEffect();
        // }
    }
}