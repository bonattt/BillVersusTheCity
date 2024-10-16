using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DialogueManager {
    // Static manager for opening new dialogues


    private static DialogueManager _inst = null;
    public static DialogueManager inst {
        get {
            if (_inst == null) {
                _inst = new DialogueManager();
            }
            return _inst;
        }
    }

    private DialogueManager() {
        // do nothing
    }

    

}