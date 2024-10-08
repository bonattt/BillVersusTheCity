using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class GeneralSettingsMenuCtrl : AbstractSettingsModuleMenu {

    private Toggle show_fps_toggle;

    public GeneralSettingsMenuCtrl() {
        // do nothing
    }

    // return the SettingsModule this controller targets
    public override ISettingsModule settings_module { get { return GameSettings.inst.audio_settings; } }
    
    // takes the root element of the sub-menu, and configures the menu's controller
    public override void Initialize(VisualElement root) {
        this.LoadTemplate(root);
        header_label.text = "General Settings";

        // TODO --- refactor: extract Toggle creation 
        VisualElement div = new VisualElement();
        div.style.flexDirection = FlexDirection.Row;
        div.AddToClassList(SETTINGS_ITEM_CLASS);
        settings_pannel.Add(div);

        Label show_fps_label = new Label();
        show_fps_label.text = "Show FPS";
        div.Add(show_fps_label);

        show_fps_toggle = new Toggle();
        div.Add(show_fps_toggle);

        LoadSettings();
    }

    private (Slider, Label) AddVolumeSlider(string slider_label) {
        VisualElement slider_element = SettingsMenuUtil.CreatePercentSlider(slider_label);
        settings_pannel.Add(slider_element);
        return (slider_element.Q<Slider>(), slider_element.Q<Label>(SettingsMenuUtil.SLIDER_VALUE_LABEL));
    }

    public override void SaveSettings() {
        // Saves the menu's changes to settings    
        GeneralSettings settings = GameSettings.inst.general_settings;

        settings.show_fps = show_fps_toggle.value;
    }

    public override void LoadSettings() {
        // sets the UI's elements to match what is stored in settings (reverting any changes)
        GeneralSettings settings = GameSettings.inst.general_settings;
        
        show_fps_toggle.value = settings.show_fps;
        UpdateUI();
    }
    
    
    public override bool HasUnsavedChanges() {
        GeneralSettings settings = GameSettings.inst.general_settings;
        return show_fps_toggle.value != settings.show_fps;
    }

}