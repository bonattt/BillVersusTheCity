


using UnityEngine;

public class ActivateInteractionEvent : MonoBehaviour, IGameEvent, IInteractionEffect {
    
    public Interaction interaction; 

    public void Interact(GameObject actor) {
        ActivateEvent();
    }
    
    public void ActivateEvent() {
        interaction.interaction_enabled = true;
    }
}