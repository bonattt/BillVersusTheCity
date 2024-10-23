using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInteraction : MonoBehaviour, IInteractionEffect
{
    public List<string> dialogue_files;
    
    // if endless_dialouge is checked, the dialogue will repeat it's last dialouge indefinitely
    public bool endless_dialogue = true;

    // if `random_dialogue` is checked, dialouges will be shuffled
    public bool random_dialouge = false;


    void Start() {
        Reset();
    }

    [SerializeField]
    private int index = 0;

    public void Reset() {
        index = 0;
    }

    public void Interact(GameObject actor) {
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
        if (random_dialouge) {
            file_path = dialogue_files[Random.Range(0, dialogue_files.Count)];
        }
        else {
            file_path = dialogue_files[index];
            index += 1;
        }

        MenuManager.inst.OpenDialoge(file_path);
    }
}
