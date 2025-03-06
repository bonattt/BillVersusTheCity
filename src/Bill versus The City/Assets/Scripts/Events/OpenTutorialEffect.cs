
using UnityEngine;

public class OpenTutorialEffect : MonoBehaviour, IGameEventEffect, IInteractionEffect {

    public string tutorial_name;
    public MonoBehaviour tutorial_callback = null;
    private IGameEventEffect _tutorial_callback = null;

    void Start() {
        _tutorial_callback = tutorial_callback.GetComponent<IGameEventEffect>();
    }

    public void Interact(GameObject actor) {
        ActivateEffect();
    }
    
    public void ActivateEffect() {
        TutorialPopupController popup = TutorialPopupLibrary.inst.OpenTutorial(tutorial_name);
        
        if (_tutorial_callback == null) {
            // no callback, so do nothing
            Debug.LogWarning("no tutorial callback!"); // TODO --- remove debug
        }
        else if (popup == null) {
            // perform callback immediately, tutorial is skipped!
            _tutorial_callback.ActivateEffect();
        } else {
            // add callback to trigger when tutorial closed
            popup.AddCloseCallback(_tutorial_callback);
        }
    }
}