using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class EscapeMenuController : MonoBehaviour
{
    public UIDocument ui_doc;
    private VisualElement root;

    // private bool _menu_open; 
    // public bool menu_open {
    //     get { return _menu_open; }
    //     set { 
    //         _menu_open = value;
    //         if (_menu_open) {
    //             OpenMenu();
    //         } else {
    //             CloseMenu();
    //         }
    //     }
    // }

    void Start()
    {
        root = ui_doc.rootVisualElement;
        // menu_open = false;
    }
    
    public void MenuNavigation() {
        if (InputSystem.current.MenuCancelInput()) {
            MenuManager.inst.CloseMenu();
        }
    }

    // private void OpenMenu() {
    //     root.style.display = DisplayStyle.Flex;
    // }

    // private void CloseMenu() {
    //     root.style.display = DisplayStyle.None;
    // }

}
