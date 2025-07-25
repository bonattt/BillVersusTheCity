using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class PlaceholderSettingsMenuCtrl : ISettingModuleMenu {
    // Placeholder class that implements ISettingModuleMenu but does nothing.

    public PlaceholderSettingsMenuCtrl() {
        // do nothing: placeholder class
    }

    // return the SettingsModule this controller targets
    public ISettingsModule settings_module { get { return GameSettings.inst.general_settings; } }

    public void RestoreToDefaultsClicked() {
        // do nothing
    }

    // takes the root element of the sub-menu, and configures the menu's controller
    public void Initialize(VisualElement root) {
        // do nothing: placeholder class
    }

    public void SaveSettings() {
        // do nothing: placeholder class
    }

    public void LoadSettings() {
        // do nothing: placeholder class
    }

    public IEnumerable<string> UnsavedFields() {
        return new List<string>();
    }

    public bool HasUnsavedChanges() {
        return false;
    }

    public void UpdateUI() {
        // do nothing: placeholder class
    }

    public void CleanUp() {
        // do nothing: placeholder class
    }
}