using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class EscapeMenuController : MonoBehaviour
{
    public UIDocument ui_doc;
    private VisualElement root;

    private bool _menu_open; 
    public bool menu_open {
        get { return _menu_open; }
        set { 
            _menu_open = value;
            if (_menu_open) {
                OpenMenu();
            } else {
                CloseMenu();
            }
        }
    }
    void Start()
    {
        root = ui_doc.rootVisualElement;
        menu_open = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputSystem.current.PauseMenuInput()) {
            menu_open = !menu_open;
        }
    }

    private void OpenMenu() {
        Debug.Log("OpenMenu");
        Time.timeScale = 0f;
        root.style.display = DisplayStyle.Flex;
    }

    private void CloseMenu() {
        Debug.Log("CloseMenu");
        Time.timeScale = 1f;
        root.style.display = DisplayStyle.None;
    }

}
