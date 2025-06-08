


using UnityEngine;

public class EnableGameObjectEffect : AbstractInteractionGameEvent {
    
    public GameObject target; 
    public bool set_game_object_enabled = true;
    protected override void Effect() {
        target.SetActive(set_game_object_enabled);
    }
}