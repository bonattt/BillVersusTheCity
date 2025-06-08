using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInteraction : MonoBehaviour, IInteractionEffect, IGameEventEffect
{
    public List<string> dialogue_files;
    
    // if endless_dialouge is checked, the dialogue will repeat it's last dialouge indefinitely
    public bool endless_dialogue = true;

    // if `random_dialogue` is checked, dialouges will be shuffled
    public bool random_dialouge = false;

    public MonoBehaviour dialogue_callback;

    public bool effect_completed { get => true; } // TODO --- implement

    void Start() {
        Reset();
    }

    [SerializeField]
    private int index = 0;

    public void Reset() {
        index = 0;
    }
    
    public void ActivateEffect() {
        if (index >= dialogue_files.Count) {
            if (endless_dialogue) {
                index -= 1; // repeat the final dialogue
            }
            else {
                Debug.Log($"no more dialouges left for {transform.parent.gameObject.name}");
                return; // no more dialogue is left
            }
        }

        string file_path;
        int _index;
        if (random_dialouge) {
            _index = Random.Range(0, dialogue_files.Count);
            file_path = dialogue_files[_index];
        }
        else {
            _index = index++;
            file_path = dialogue_files[_index];
        }

        DialogueController ctrl = MenuManager.inst.OpenDialoge(file_path);
        IGameEventEffect callback;
        if (dialogue_callback != null) {
            callback = dialogue_callback.GetComponent<IGameEventEffect>();
        } else {
            callback = null;
        }
        if (callback != null) {
            ctrl.AddDialogueCallback(callback);
        }
        // ctrl.dialogue_finished = dialogue_finished;
        Debug.LogWarning("// TODO --- make event complete when menu is closed!");
    }

    public void Interact(GameObject actor) {
        ActivateEffect();
    }
}
