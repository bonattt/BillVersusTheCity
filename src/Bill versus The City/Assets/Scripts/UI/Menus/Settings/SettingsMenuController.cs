using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenuController : AbstractCloseEventMenu
{

    public UIDocument main_doc;
    public VisualTreeAsset general_settings_ui, graphics_settings_ui, audio_settings_ui, gameplay_settings_ui, difficulty_settings_ui;

    private VisualElement root, tabs_list, sub_menu_root, sub_menu_panel;

    private ISettingModuleMenu open_settings_controller = null;
    private string open_tab_name = "";

    public const string GENERAL_TAB = "General";
    public const string GAMEPLAY_TAB = "Gameplay";
    public const string DIFFICULTY_TAB = "Difficulty";
    public const string GRAPHICS_TAB = "Graphics";
    public const string AUDIO_TAB = "Audio";
    public readonly string[] tabs_order = new string[]{GENERAL_TAB, GAMEPLAY_TAB, DIFFICULTY_TAB, GRAPHICS_TAB, AUDIO_TAB};
    /** ADDING A NEW TAB
      * 1) add the tab to `tabs_order`
      * 2) Add the tab to `GetTabUI`
      * 3) Add the tab to `GetTabController`
      * 4) add the UXML for the new Tab's UI to the controller's prefab
      */

    private VisualTreeAsset GetTabUI(string tab_name) {
        // takes a tab-name and returns the UXML for that tab.
        // needs to be a method, because the VisualTreeAssets are dynamic
        switch(tab_name) {
            case GENERAL_TAB: return general_settings_ui;
            case GRAPHICS_TAB: return graphics_settings_ui;
            case DIFFICULTY_TAB: return difficulty_settings_ui;
            case GAMEPLAY_TAB: return gameplay_settings_ui;
            case AUDIO_TAB: return audio_settings_ui;

            default:
                Debug.LogError($"unknown settings tab {tab_name}");
                return null;
        }
    }
    
    private ISettingModuleMenu GetTabController(string tab_name) {
        // takes a tab name and returns a new controller script for that tab type
        switch(tab_name) {
            case AUDIO_TAB: return new AudioSettingsMenuCtrl();
            case DIFFICULTY_TAB: return new DifficultySettingsMenuCtrl();
            case GENERAL_TAB: return new GeneralSettingsMenuCtrl();
            case GAMEPLAY_TAB: return new GameplaySettingsMenuCtrl();
            default:
                Debug.LogWarning($"Unknown settings tab `{tab_name}`");
                return new PlaceholderSettingsMenuCtrl();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        root = main_doc.rootVisualElement;
        tabs_list = root.Q<VisualElement>("tabs_list");
        sub_menu_panel = root.Q<VisualElement>("options_panel");

        OpenSubDocument(tabs_order[0]);
        UpdateTabs();
    }

    void Update() {
        if (open_settings_controller != null) {
            open_settings_controller.UpdateUI();
        }
    }

    public void UpdateTabs() {
        // updates the navigation tab sidebar.
        tabs_list.Clear();
        for (int i = 0; i < tabs_order.Length; i++) {
            string tab_name = tabs_order[i];
            AddModuleTab(tab_name);
        }
    }

    
    public override void MenuNavigation() {
        if (InputSystem.current.MenuCancelInput()) {
            if (open_settings_controller == null || !open_settings_controller.HasUnsavedChanges()) {
                MenuManager.inst.CloseMenu();
            } else {
                OpenConfirmExitDialogue();
            }

        }
    }

    // public Action GetClickEvent(string tab_name) {
        
    // }

    // public void TabClicked

    private void AddModuleTab(string tab_name) {
        // adds a navigation tab for the given tab_name.
        Button button = new Button();
        button.text = tab_name;
        button.AddToClassList("settings_module_tab");
        tabs_list.Add(button);
        MenuManager.AddGenericEvents(button);
        button.clicked += GetSettingsTabCallback(tab_name);
    }

    private System.Action GetSettingsTabCallback(string tab_name) {
        return () => SettingsTabClicked(tab_name);
    }

    public void SettingsTabClicked(string tab_name) {
        Debug.Log($"settings tab clicked. open_settings_controller: {open_settings_controller}");
        if (tab_name.Equals(open_tab_name)) { 
            return; // do nothing if the already open tab is clicked
        }
        else if (open_settings_controller != null) {
            Debug.Log($"has unsaved changes: {open_settings_controller.HasUnsavedChanges()}");
            if (open_settings_controller.HasUnsavedChanges()) {
                OpenConfirmNaviagtionDialogue(tab_name);
            } else {
                OpenSubDocument(tab_name);
            }
        }
        else {
            OpenSubDocument(tab_name);
        }
    }

    public void OpenConfirmNaviagtionDialogue(string tab_name) {
        YesNoPopupController popup = _OpenConfirmationDialogue();
        popup.confirm_button.clicked -= MenuManager.inst.CloseMenu;
        popup.confirm_button.clicked += () => OpenSubDocument(tab_name);
        // popup.cancel_button.clicked += 
    }

    public void OpenConfirmExitDialogue() {
        YesNoPopupController popup = _OpenConfirmationDialogue();
        popup.confirm_button.clicked += () => CloseSettingsFromPopup();
        // popup.cancel_button.clicked += 
    }

    public void CloseSettingsFromPopup() {
        // closes the settings menu from a popup dialogue button.
        MenuManager.inst.CloseMenu(); // close pop-up dialogue
        MenuManager.inst.CloseMenu(); // close settings menu 

    }

    private YesNoPopupController _OpenConfirmationDialogue() {
        YesNoPopupController popup = MenuManager.inst.OpenNewPopup();
        popup.header_text = "Unsaved Changes";
        popup.content_text = "You have unsaved changes, are you sure you want to leave this menu?";
        popup.confirm_text = "Discard";
        popup.reject_text = "Cancel";
        popup.UpdateLabels();
        return popup;
    }

    private void CleanUpSubDocument() {
        // cleans up the already open sub document so a new doc can be opened
        // returns true if cleanup is successful
        sub_menu_panel.Clear();
        if (open_settings_controller != null) {
            open_settings_controller.CleanUp();
            open_settings_controller = null;
        }
    }

    private void OpenSubDocument(string tab_name) {
        // opens a new tab in the settings menu
        CleanUpSubDocument();
        open_tab_name = tab_name;
        VisualTreeAsset uxml = GetTabUI(tab_name);
        if (uxml == null) {
            Debug.LogWarning("opening null settings menu sub-module");
            return;
        }

        sub_menu_root = uxml.CloneTree();
        sub_menu_panel.Add(sub_menu_root);
        open_settings_controller = GetTabController(tab_name);
        AddSaveLoadButtons();

        open_settings_controller.Initialize(sub_menu_root);

    }

    private VisualElement AddSaveLoadButtons() {
        VisualElement controls = sub_menu_root.Q<VisualElement>("Controls");
        controls = SettingsMenuUtil.CreateSettingsControlButtons(open_settings_controller, controls);
        // if (!sub_menu_root.Children().Contains(controls)) {
            sub_menu_root.Add(controls);
        // }
        return controls;
    }

}
