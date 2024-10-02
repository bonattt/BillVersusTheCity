using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class GeneralSettingsMenuCtrl : ISettingModuleMenu {

    private VisualElement root, settings_pannel, buttons_pannel; 
    private Label header_label;

    private Toggle show_fps_toggle;

    public GeneralSettingsMenuCtrl() {
        // do nothing
    }

    // return the SettingsModule this controller targets
    public ISettingsModule settings_module { get { return GameSettings.inst.audio_settings; } }
    
    // takes the root element of the sub-menu, and configures the menu's controller
    public void Initialize(VisualElement root) {
        this.root = root;
        settings_pannel = root.Q<VisualElement>("List");
        buttons_pannel = root.Q<VisualElement>("Controlls");
        header_label = root.Q<Label>("HeaderText");
        header_label.text = "Audio Settings";

        settings_pannel.Clear();

        // TODO --- refactor: extract Toggle creation 
        VisualElement div = new VisualElement();
        div.style.flexDirection = FlexDirection.Row;
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

    public void SaveSettings() {
        // Saves the menu's changes to settings    
        GeneralSettings settings = GameSettings.inst.general_settings;

        settings.show_fps = show_fps_toggle.value;
    }

    public void LoadSettings() {
        // sets the UI's elements to match what is stored in settings (reverting any changes)
        GeneralSettings settings = GameSettings.inst.general_settings;
        
        show_fps_toggle.value = settings.show_fps;
        UpdateUI();
    }

    public void UpdateUI() {
        // updates the UI
        // nothing to do here yet
    }

    public void CleanUp() {
        // disposes of any resources that need to be cleaned when the sub-menu is closed
        // nothing to do here
    }
}