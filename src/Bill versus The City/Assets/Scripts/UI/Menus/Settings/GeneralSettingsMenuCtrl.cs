using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class GeneralSettingsMenuCtrl : AbstractSettingsModuleMenu {

    private Toggle reset_tutorials_toggle, skip_tutorials_toggle;

    public GeneralSettingsMenuCtrl() {
        // do nothing
    }

    // return the SettingsModule this controller targets
    public override ISettingsModule settings_module { get { return GameSettings.inst.general_settings; } }
    
    // takes the root element of the sub-menu, and configures the menu's controller
    public override void Initialize(VisualElement root) {
        this.LoadSettingsUXML(root);
        header_label.text = "General Settings";
        
        VisualElement skip_tutorials_div = AddToggle("Skip All Tutorials");
        skip_tutorials_toggle = skip_tutorials_div.Q<Toggle>();

        VisualElement reset_tutorials_div = AddToggle("Reset Tutorials");
        reset_tutorials_toggle = reset_tutorials_div.Q<Toggle>();

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
        GeneralSettings settings = GameSettings.inst.general_settings;

        settings.skip_all_tutorials = skip_tutorials_toggle.value;
        if (reset_tutorials_toggle.value) {
            settings.skipped_tutorials = new HashSet<string>();
        }
        reset_tutorials_toggle.value = false;
    }

    public override void LoadSettings() {
        // sets the UI's elements to match what is stored in settings (reverting any changes)
        GeneralSettings settings = GameSettings.inst.general_settings;
        
        skip_tutorials_toggle.value = settings.skip_all_tutorials;
        reset_tutorials_toggle.value = false;
        UpdateUI();
    }
    
    
    public override bool HasUnsavedChanges() {
        GeneralSettings settings = GameSettings.inst.general_settings;
        // reset_tutorials is never actually stored, it clears "skipped_tutorials" whenever saved
        return skip_tutorials_toggle.value != settings.skip_all_tutorials || reset_tutorials_toggle.value;
    }

}