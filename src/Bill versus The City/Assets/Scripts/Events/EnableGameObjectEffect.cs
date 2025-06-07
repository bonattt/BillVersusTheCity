


using UnityEngine;

public class EnableGameObjectEffect : MonoBehaviour, IGameEventEffect, IInteractionEffect {
    
    public GameObject target; 
    public bool set_game_object_enabled = true;

    public void Interact(GameObject actor) {
        ActivateEffect();
    }
    
    public void ActivateEffect() {
        target.SetActive(set_game_object_enabled);
    }
}