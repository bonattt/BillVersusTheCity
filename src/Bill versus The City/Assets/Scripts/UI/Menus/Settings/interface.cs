using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public interface ISettingModuleMenu {
    // return the SettingsModule this controller targets
    public ISettingsModule settings_module { get; }
    
    // takes the root element of the sub-menu, and configures the menu's controller
    public void Initialize(VisualElement root);

    // Saves the menu's changes to settings
    public void SaveSettings();

    // sets the UI's elements to match what is stored in settings (reverting any changes)
    public void LoadSettings();
    public bool HasUnsavedChanges();

    // Updates UI to display changes
    public void UpdateUI();

    // disposes of any resources that need to be cleaned when the sub-menu is closed
    public void CleanUp();
}