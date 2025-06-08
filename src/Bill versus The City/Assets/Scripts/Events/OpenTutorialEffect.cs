
using UnityEngine;

public class OpenTutorialEffect : AbstractInteractionGameEvent {

    public string tutorial_name;
    public MonoBehaviour tutorial_callback = null;
    private IGameEventEffect _tutorial_callback = null;

    protected override void Start() {
        base.Start();
        if (tutorial_callback != null) {
            _tutorial_callback = tutorial_callback.GetComponent<IGameEventEffect>();
        }
        Debug.LogWarning("// TODO --- make event complete when menu is closed!");
    }

    protected override void Effect() {
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