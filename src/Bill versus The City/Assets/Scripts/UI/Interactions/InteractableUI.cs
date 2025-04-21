using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class InteractableUI : MonoBehaviour
{

    public GameObject target;
    protected Interaction target_manager;
    protected UIDocument ui_doc;
    protected VisualElement root, background_pannel;
    protected Label interaction_label; 
    public string name_header = "name"; 
    public bool hide_until_enabled = false;

    public float hover_opacity = 1f;
    public float default_opacity = 0.5f;

    void Start()
    {
        Configure();
        SetLabels();
        UpdateVisibility();
    }

    private void Configure() {
        // initializes fields
        target_manager = target.GetComponent<Interaction>();

        ui_doc = GetComponent<UIDocument>();
        root = ui_doc.rootVisualElement;
        background_pannel = root.Q<VisualElement>("Panel");
        // background_pannel.RegisterCallback<MouseEnterEvent>(evnt => SetAsTargeted());
        // background_pannel.RegisterCallback<MouseLeaveEvent>(evnt => SetAsNotTargeted());
        
        interaction_label = root.Q<Label>("InteractionLabel");
    }

    public void SetNewText(string new_text) {
        name_header = new_text;
        SetLabels();
    }

    void Update() {
        UpdateVisibility();
    }

    public void SetAsTargeted() {
        UpdateOpacity(hover_opacity);
    }

    public void SetAsNotTargeted() {
        UpdateOpacity(default_opacity);
    }

    protected void UpdateOpacity(float opacity) {
        Color current_color = background_pannel.resolvedStyle.backgroundColor;
        Color new_color = new Color(
            current_color.r,
            current_color.g,
            current_color.b,
            opacity
        );
        background_pannel.style.backgroundColor = new_color;
    }

    protected void UpdateVisibility() {
        if (hide_until_enabled && target_manager.enabled) {
            // Debug.Log($"clear hide until enabled on {gameObject.name} for {target_manager.gameObject.name}");
            hide_until_enabled = false;
        }
        if(IsVisible()) {
            root.style.visibility = Visibility.Visible;
            if (IsTargeted()) {
                SetAsTargeted();
                SetLabels();
            } else {
                SetAsNotTargeted();
                SetLabels();
            }
        } else {
            root.style.visibility = Visibility.Hidden;
        }
    }

    protected bool IsVisible() {
        return !hide_until_enabled && target_manager.interaction_tracked;
    }
     
    protected bool IsTargeted() {
        return target_manager.interaction_targeted;
    }

    protected void SetLabels() {
        // updates the UI with current display values
        if (interaction_label == null) {
            Configure();
        }
        string interaction_input = "";
        if (IsTargeted()) {
            // only show the keystroke to interact with the interaction if it is actually targetted to interact with
            interaction_input = $"[{InputSystem.current.InputTypeDisplay(InputType.interact)}]";
        }

        interaction_label.text = $"{name_header} {interaction_input}";
    }
}
