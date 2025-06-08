using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovePickupWeapons : AbstractInteractionGameEvent
{
    public string cleanup_triggered_dialogue;
    public MonoBehaviour dialogue_finished;

    protected override void Effect() {
        if (PlayerCharacter.inst.inventory.pickup != null) {
            PlayerCharacter.inst.inventory.pickup = null;
            DialogueController ctrl = MenuManager.inst.OpenDialoge(cleanup_triggered_dialogue);
            ctrl.dialogue_finished = dialogue_finished;
        }
        else {
            if (dialogue_finished == null) { return; }
            IGameEventEffect g_event = (IGameEventEffect) dialogue_finished;
            if (g_event == null) {
                Debug.LogWarning($"{dialogue_finished} does not have a game event attached!");
            } else {
                g_event.ActivateEffect();
            }
        }
    }
}
