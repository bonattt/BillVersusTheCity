using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class PopupController : AbstractCloseEventMenu
{
    protected VisualElement root_visual;

    public string header_text = "Popup";
    public string content_text = "";
    public string confirm_text = "Continue"; 

    public bool allow_escape = true; // if true, the menu can be closed by hitting escape.

    protected Label header_label, content_label, confirm_label;
    public Button confirm_button { get; protected set; }

    void Start() {
        Configure();
        UpdateLabels();
    }

    public override void MenuNavigation() {
        if (allow_escape && InputSystem.current.MenuCancelInput()) {
            MenuManager.inst.CloseMenu();
        }
    }

    public virtual void Configure() {
        // setup object references to UI components.
        root_visual = GetComponentInChildren<UIDocument>().rootVisualElement;

        header_label = root_visual.Q<Label>("HeaderLabel");
        content_label = root_visual.Q<Label>("Contents");
        confirm_label = root_visual.Q<Label>("ConfirmLabel");

        confirm_button = root_visual.Q<Button>("ConfirmButton");

        confirm_button.RegisterCallback<ClickEvent>(MenuManager.CloseMenuClick);
    }

    // public void AddClickHandler(Action<ClickEvent> handler) {
    //     confirm_button.RegisterCallback<ClickEvent>(handler);
    // }

    // public void TestClickEvent(ClickEvent evnt) {
    //     // Debug Log when buttons are pressed. Use for testing
    //     Button target = evnt.currentTarget as Button;
    //     Label child = target.Q<Label>();
    //     if (child == null) {
    //         Debug.LogWarning("Test button press event! (no label found)");
    //     } else {
    //         Debug.LogWarning($"button '{child.text}' pressed with test event!");
    //     }
    // }

    // public void CloseMenuClick(ClickEvent _) {
    //     MenuManager.inst.CloseMenu();
    // }

    public virtual void UpdateLabels() {
        header_label.text = header_text;
        content_label.text = content_text;
        confirm_label.text = confirm_text;
    }

    // public void CloseMenu() {
    //     _menu_open = false;
    //     Destroy(gameObject);
    // }

    // void OnDestroy() {

    // }

}
