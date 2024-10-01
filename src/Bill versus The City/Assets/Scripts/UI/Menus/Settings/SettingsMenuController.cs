using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenuController : MonoBehaviour
{

    public UIDocument main_doc;
    public VisualTreeAsset general_settings_ui, graphics_settings_ui, audio_settings_ui, gameplay_settings_ui;

    private VisualElement root, tabs_list, sub_menu_root, sub_menu_panel;

    private ISettingModuleMenu open_settings_controller = null;

    public const string GENERAL_TAB = "General";
    public const string GRAPHICS_TAB = "Graphics";
    public const string GAMEPLAY_TAB = "Gameplay";
    public const string AUDIO_TAB = "Audio";
    public readonly string[] tabs_order = new string[]{GENERAL_TAB, GRAPHICS_TAB, GAMEPLAY_TAB, AUDIO_TAB};
    private VisualTreeAsset GetTabUI(string tab_name) {
        // takes a tab-name and returns the UXML for that tab.
        // needs to be a method, because the VisualTreeAssets are dynamic
        switch(tab_name) {
            case GENERAL_TAB: return general_settings_ui;
            case GRAPHICS_TAB: return graphics_settings_ui;
            case GAMEPLAY_TAB: return gameplay_settings_ui;
            case AUDIO_TAB: return audio_settings_ui;

            default:
                Debug.LogError($"unknown settings tab {tab_name}");
                return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        root = main_doc.rootVisualElement;
        tabs_list = root.Q<VisualElement>("tabs_list");
        sub_menu_panel = root.Q<VisualElement>("options_panel");

        OpenSubDocument(audio_settings_ui);
        UpdateTabs();
    }

    public void UpdateTabs() {
        tabs_list.Clear();
        for (int i = 0; i < tabs_order.Length; i++) {
            string tab_name = tabs_order[i];
            AddModuleTab(tab_name);
        }
    }

    // public Action GetClickEvent(string tab_name) {
        
    // }

    // public void TabClicked

    private void AddModuleTab(string tab_name) {
        Button button = new Button();
        button.text = tab_name;
        button.AddToClassList("settings_module_tab");
        tabs_list.Add(button);
        MenuManager.AddGenericEvents(button);
        button.clicked += GetSettingsTabCallback(tab_name);
    }

    private System.Action GetSettingsTabCallback(string tab_name) {
        return () => OpenSubDocument(GetTabUI(tab_name));
    }

    private void CleanUpSubDocument() {
        // cleans up the already open sub document so a new doc can be opened
        sub_menu_panel.Clear();
        if (open_settings_controller != null) {
            open_settings_controller.CleanUp();
            open_settings_controller = null;
        }
    }

    private void OpenSubDocument(VisualTreeAsset uxml) {
        // opens a new tab in the settings menu
        CleanUpSubDocument();

        if (uxml == null) {
            Debug.LogWarning("opening null settings menu sub-module");
            return;
        }

        sub_menu_root = uxml.CloneTree();
        sub_menu_panel.Add(sub_menu_root);
        open_settings_controller = new AudioSettingsMenuCtrl();
        open_settings_controller.Initialize(sub_menu_root);

    }

}
