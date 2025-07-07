
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class TutorialHUD : MonoBehaviour, IGameEventEffect {

    public UIDocument ui_document;
    public string tutorial_text = "";
    public float max_tutorial_duration = 30f;
    private float openned_at_time = -1f;

    public bool open_on_start = true;
    
    public List<InputType> complete_tutorial_inputs;
    [Tooltip("if true, every input in `complete_tutorial_inputs` must be clicked to close the tutorial. Otherwise, any one of the inputs will close the tutorial.")]
    public bool require_all_inputs = true;

    private HashSet<InputType> inputs_made = new HashSet<InputType>();

    private bool is_open = false;

    private Label tutorial_label;
    
    public bool effect_completed { get; protected set; }

    void Start() {
        effect_completed = false;
        tutorial_label = ui_document.rootVisualElement.Q<Label>();
        tutorial_label.text = tutorial_text;
        

        ui_document.rootVisualElement.experimental.animation.Start(25f, 200f, 300, (b, val) => {
            b.style.height = val;
        }); //.Ease(Easing.OutBounce);

        if (open_on_start) {
            OpenTutorialHUD();
        } else {
            CloseTutorialHUD();
        }
    }

    void Update() {
        if (!is_open || MenuManager.inst.paused) { return; } // do nothing if paused or if the tutorial isn't showing

        if (Time.time >= max_tutorial_duration + openned_at_time) {
            CloseTutorialHUD();
            effect_completed = true;
        }
        else if (RequiredInputsEntered()) {
            CloseTutorialHUD();
            effect_completed = true;
        }
    }

    public bool RequiredInputsEntered() {
        // returns true if every input required to close the tutorial has been made.
        if (complete_tutorial_inputs.Count == 0) { return false; } // if no tutorial inputs are given, never close from inputs, wait for timeout instead. 

        foreach(InputType input in complete_tutorial_inputs) {
            if (inputs_made.Contains(input)) { continue; } // input already made
            else if (InputSystem.current.GetGenericInput(input)) {
                inputs_made.Add(input);
                if (!require_all_inputs) { 
                    Debug.Log("one required input made, short circuit");
                    return true; 
                }
            }
        }
        if (require_all_inputs) {
            if (inputs_made.Count >= complete_tutorial_inputs.Count) {
                Debug.Log("all required inputs made, tutorial should close");
                return true;
            }
        }
        else {
            if (inputs_made.Count >= 1) {
                // This should be unreachable...
                Debug.Log("at least one required input was made, tutorial should close");
                return true;
            }
        }
        return false;
    }

    public void CloseTutorialHUD() {
        is_open = false;
        ui_document.rootVisualElement.style.display = DisplayStyle.None;
    }

    
    public void ActivateEffect() {
        OpenTutorialHUD();
    }

    public void OpenTutorialHUD() {
        openned_at_time = Time.time;
        is_open = true;
        inputs_made = new HashSet<InputType>();
        ui_document.rootVisualElement.style.display = DisplayStyle.Flex;
    }

}
