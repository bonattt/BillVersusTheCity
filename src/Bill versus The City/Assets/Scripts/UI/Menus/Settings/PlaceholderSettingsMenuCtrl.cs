using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class PlaceholderSettingsMenuCtrl : ISettingModuleMenu {
    // Placeholder class that implements ISettingModuleMenu but does nothing.
    
    public PlaceholderSettingsMenuCtrl() {
        // do nothing
    }

    // return the SettingsModule this controller targets
    public ISettingsModule settings_module { get { return GameSettings.inst.audio_settings; } }
    
    // takes the root element of the sub-menu, and configures the menu's controller
    public void Initialize(VisualElement root) {
        // do nothing
    }

    public void SaveSettings() {
        // do nothing
    }

    public void LoadSettings() {
        // do nothing
    }

    public void CleanUp() {
        // do nothing
    }
}