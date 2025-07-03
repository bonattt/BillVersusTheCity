
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
    public override void ActivateEffect() {
        // NOTE: implements calling Effect without setting `effect_completed`, 
        //       because the effect is only complete once the tutorial is closed.
        effect_completed = false;
        Effect();
    }

    protected override void Effect() {
        TutorialPopupController popup = TutorialPopupLibrary.inst.OpenTutorial(tutorial_name);
        if (popup == null) {
            // perform callback immediately, tutorial is skipped!
            if (_tutorial_callback != null) { _tutorial_callback.ActivateEffect(); }
            effect_completed = true;
        } else {
            Debug.LogWarning("Tutorial popup is NOT null!"); // TODO --- remove debug
            // add callback to trigger when tutorial closed
            if (_tutorial_callback != null) { popup.AddCloseCallback(_tutorial_callback); }
            popup.AddCloseCallback(new SimpleActionEvent(() => effect_completed = true));
            popup.AddCloseCallback(new SimpleActionEvent(() => Debug.LogWarning("Tutorial popup Close Callback!"))); // TODO --- remove debug
        }
    }
}