


using UnityEngine;

public class EnableInteractionEffect : MonoBehaviour, IGameEventEffect, IInteractionEffect {
    
    public Interaction interaction; 
    public bool set_interaction_enabled = true;

    public void Interact(GameObject actor) {
        ActivateEffect();
    }
    
    public void ActivateEffect() {
        interaction.interaction_enabled = set_interaction_enabled;
    }
}