


using UnityEngine;

public class EnableInteractionEffect : AbstractInteractionGameEvent {
    
    public Interaction interaction; 
    public bool set_interaction_enabled = true;
    protected override void Effect() {
        interaction.interaction_enabled = set_interaction_enabled;
    }
}