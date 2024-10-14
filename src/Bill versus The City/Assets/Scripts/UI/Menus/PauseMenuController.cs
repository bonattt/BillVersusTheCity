using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    public UIDocument ui_doc;
    private VisualElement root;
    private Button resume_button, settings_button, restart_button, exit_button;
    private Button[] all_buttons;

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

        resume_button = root.Q<Button>("ResumeButton");
        settings_button = root.Q<Button>("SettingsButton");
        restart_button = root.Q<Button>("RestartButton");
        exit_button = root.Q<Button>("ExitButton");


        resume_button.RegisterCallback<ClickEvent>(MenuManager.CloseMenuClick);
        resume_button.clicked += SaveFile.current_save.SaveOnExit;
        settings_button.RegisterCallback<ClickEvent>(SettingsButtonClicked);
        restart_button.RegisterCallback<ClickEvent>(RestartButtonClicked);
        exit_button.RegisterCallback<ClickEvent>(ExitGameClick); 
        
        all_buttons = new Button[]{resume_button, settings_button, restart_button, exit_button};
        MenuManager.AddGenericEvents(all_buttons);
    }

    public void RestartButtonClicked(ClickEvent _) {
        // click event for when the restart level button is clicked
        YesNoPopupController popup = MenuManager.inst.OpenNewPopup();
        popup.header_text = "Restart Level";
        popup.content_text = "Are you sure you want restart? All progress will be lost";
        popup.UpdateLabels();
        popup.confirm_button.clicked += RestartButtonConfirmed;
    }

    public void RestartButtonConfirmed() {
        MenuManager.inst.CloseAllMenus();
        ScenesUtil.RestartLevel();
    }

    public void SettingsButtonClicked(ClickEvent _) {
        MenuManager.inst.OpenSubMenuPrefab(MenuManager.inst.settings_menu_prefab);
    }

    public void ExitGameClick(ClickEvent _) {
        YesNoPopupController popup = MenuManager.inst.OpenNewPopup();
        popup.header_text = "Exit Game";
        popup.content_text = "Are you sure you want to quit?";
        popup.UpdateLabels();
        popup.confirm_button.RegisterCallback<ClickEvent>(ExitGameConfirmClick);
    }

    public void ExitGameConfirmClick(ClickEvent _) {
        ExitGame();
    }

    public static void ExitGame() {
        // TODO --- move this code somewhere more suitable
        Debug.Log("Game is exiting...");
        if(SaveFile.current_save != null) {
            SaveFile.current_save.SaveOnExit();
        }

        // preprocessor #if, #else, #endif optimizes the code by excluding code sections at compile time instead of runtime
        #if UNITY_EDITOR
            // If running in the Unity Editor, stop playing the scene
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // If running in a standalone build, quit the application
            Application.Quit();
        #endif
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
