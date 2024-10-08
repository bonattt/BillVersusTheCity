using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public abstract class AbstractSettingsModuleMenu : ISettingModuleMenu {

    protected VisualElement root, settings_pannel, buttons_pannel; 
    protected Label header_label;

    // style-sheet class for any top-level item in a settings module menue
    public const string SETTINGS_ITEM_CLASS = "settings_item";

    public abstract ISettingsModule settings_module { get; }
    
    public abstract void Initialize(VisualElement root);

    protected virtual void LoadTemplate(VisualElement root) {
        // loads the root visaul element and finds required class field in the Visual Tree
        this.root = root;
        settings_pannel = root.Q<VisualElement>("List");
        buttons_pannel = root.Q<VisualElement>("Controlls");
        header_label = root.Q<Label>("HeaderText");
        header_label.text = "Settings";

        settings_pannel.Clear();
    }

    public abstract void SaveSettings();

    public abstract void LoadSettings();

    public abstract bool HasUnsavedChanges();

    public void UpdateUI() {
        // updates the UI
        // nothing to do here yet
    }

    public void CleanUp() {
        // disposes of any resources that need to be cleaned when the sub-menu is closed
        // nothing to do here
    }
}