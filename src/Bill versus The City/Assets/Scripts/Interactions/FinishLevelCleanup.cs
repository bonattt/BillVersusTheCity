using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLevelCleanup : MonoBehaviour, IInteractionEffect, IGameEvent
{
    public string cleanup_triggered_dialogue;
    public MonoBehaviour dialogue_finished;

    public void ActivateEvent() {
        if (PlayerCharacter.inst.inventory.pickup != null) {
            PlayerCharacter.inst.inventory.pickup = null;
            DialogueController ctrl = MenuManager.inst.OpenDialoge(cleanup_triggered_dialogue);
            ctrl.dialogue_finished = dialogue_finished;
        }
        else {
            if (dialogue_finished == null) { return; }
            IGameEvent g_event = dialogue_finished.GetComponent<IGameEvent>();
            if (g_event == null) {
                Debug.LogWarning($"{dialogue_finished} does not have a game event attached!");
            } else {
                g_event.ActivateEvent();
            }
        }
    }

    public void Interact(GameObject actor) {
        ActivateEvent();
    }
}
