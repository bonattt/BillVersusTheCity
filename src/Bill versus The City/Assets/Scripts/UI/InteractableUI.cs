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

    public float hover_opacity = 1f;
    public float default_opacity = 0.5f;

    void Start()
    {
        target_manager = target.GetComponent<Interaction>();

        ui_doc = GetComponent<UIDocument>();
        root = ui_doc.rootVisualElement;
        background_pannel = root.Q<VisualElement>("Panel");
        background_pannel.RegisterCallback<MouseEnterEvent>(evnt => SetAsHovered());
        background_pannel.RegisterCallback<MouseLeaveEvent>(evnt => SetAsNotHovered());
        
        interaction_label = root.Q<Label>("InteractionLabel");

        SetLabels();
        UpdateVisibility();
    }

    void Update() {
        UpdateVisibility();
    }

    private bool is_hovered { get { return false; }}


    public void SetAsHovered() {
        Debug.Log("SetAsHovered"); // TODO --- remove debug
        UpdateOpacity(hover_opacity);
    }

    public void SetAsNotHovered() {
        Debug.Log("SetAsNotHovered"); // TODO --- remove debug
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
        if(IsVisible()) {
            root.style.visibility = Visibility.Visible;
        } else {
            root.style.visibility = Visibility.Hidden;
        }
    }

    protected bool IsVisible() {
        return PlayerInteractor.inst.CanInteractWith(target_manager);
    }

    protected void SetLabels() {
        // updates the UI with current display values
        interaction_label.text = $"{name_header} [{InputSystem.current.InputTypeDisplay(InputType.interact)}]";
    }
}
