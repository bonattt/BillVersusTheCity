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
    public ISettingsModule settings_module { get { return GameSettings.inst.audio_settings; } }
    
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

    public void UpdateUI() {
        // do nothing: placeholder class
    }

    public void CleanUp() {
        // do nothing: placeholder class
    }
}