using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class DebugSettingsMenuCtrl : AbstractSettingsModuleMenu {

    private Toggle show_fps_toggle, show_damage_toggle, debug_mode_toggle;

    public DebugSettingsMenuCtrl() {
        // do nothing
    }

    // return the SettingsModule this controller targets
    public override ISettingsModule settings_module { get { return GameSettings.inst.audio_settings; } }
    
    // takes the root element of the sub-menu, and configures the menu's controller
    public override void Initialize(VisualElement root) {
        this.LoadTemplate(root);
        header_label.text = "Debug Settings";
        VisualElement show_fps_div = AddToggle("Show FPS");
        show_fps_toggle = show_fps_div.Q<Toggle>();
        
        VisualElement show_damage_div = AddToggle("Show Damage Numbers");
        show_damage_toggle = show_damage_div.Q<Toggle>();
        
        VisualElement debug_mode_div = AddToggle("Debug Mode");
        debug_mode_toggle = debug_mode_div.Q<Toggle>();

        LoadSettings();
    }

    private VisualElement AddToggle(string display_text) {
        // adds a div containing a toggle control, and returns the top-level VisualElement
        
        // TODO --- refactor: move this somewhere more reusable 
        VisualElement div = new VisualElement();
        div.style.flexDirection = FlexDirection.Row;
        div.AddToClassList(SETTINGS_ITEM_CLASS);
        settings_pannel.Add(div);

        Label toggle_label = new Label();
        toggle_label.text = display_text;
        div.Add(toggle_label);

        Toggle toggle = new Toggle();
        div.Add(toggle);
        
        return div;
    }

    // private (Slider, Label) AddVolumeSlider(string slider_label) {
    //     VisualElement slider_element = SettingsMenuUtil.CreatePercentSlider(slider_label);
    //     settings_pannel.Add(slider_element);
    //     return (slider_element.Q<Slider>(), slider_element.Q<Label>(SettingsMenuUtil.SLIDER_VALUE_LABEL));
    // }

    public override void SaveSettings() {
        // Saves the menu's changes to settings    
        DebugSettings settings = GameSettings.inst.debug_settings;

        settings.show_fps = show_fps_toggle.value;
        settings.debug_mode = debug_mode_toggle.value;
        settings.show_damage_numbers = show_damage_toggle.value;
    }

    public override void LoadSettings() {
        // sets the UI's elements to match what is stored in settings (reverting any changes)
        DebugSettings settings = GameSettings.inst.debug_settings;
        
        show_fps_toggle.value = settings.show_fps;
        debug_mode_toggle.value = settings.debug_mode;
        show_damage_toggle.value = settings.show_damage_numbers;
        UpdateUI();
    }
    
    
    public override bool HasUnsavedChanges() {
        DebugSettings settings = GameSettings.inst.debug_settings;
        return show_fps_toggle.value != settings.show_fps || debug_mode_toggle.value != settings.debug_mode || show_damage_toggle.value != settings.show_damage_numbers;
    }

}