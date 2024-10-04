using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class InteractableUI : MonoBehaviour
{

    public GameObject target;
    private Interaction target_manager;
    private UIDocument ui_doc;

    private VisualElement root;

    private Label interaction_label; 
    public string name = "name"; 
    public string additional_text = "";
    void Start()
    {
        target_manager = target.GetComponent<Interaction>();

        ui_doc = GetComponent<UIDocument>();
        root = ui_doc.rootVisualElement;
        
        interaction_label = root.Q<Label>("InteractionLabel");

        SetLabels();
        UpdateVisibility();
    }

    void Update() {
        UpdateVisibility();
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
        interaction_label.text = $"{name} [{InputSystem.current.InputTypeDisplay(InputType.interact)}]";
    }
}
