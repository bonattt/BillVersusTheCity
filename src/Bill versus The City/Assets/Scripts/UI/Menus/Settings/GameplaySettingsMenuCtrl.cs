using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class GameplaySettingsMenuCtrl : AbstractSettingsModuleMenu {

    private Slider mouse_sensitivity_slider;
    private Label mouse_sensitivity_label;  // label that displays the value of `mouse_sensitivity_slider` as a number

    public GameplaySettingsMenuCtrl() {
        // do nothing
    }

    // return the SettingsModule this controller targets
    public override ISettingsModule settings_module { get { return GameSettings.inst.difficulty_settings; } }
    
    // takes the root element of the sub-menu, and configures the menu's controller
    public override void Initialize(VisualElement root) {
        this.LoadTemplate(root);

        // VisualElement div = new VisualElement();
        // div.style.flexDirection = FlexDirection.Row;
        // settings_pannel.Add(div);

        AddMouseSensitivitySlider();
        LoadSettings();
    }

    

    private VisualElement AddMouseSensitivitySlider() {
        VisualElement slider_element = SettingsMenuUtil.CreateSlider("Mouse Sensitivity", 0.25f, 3f);
        settings_pannel.Add(slider_element);

        (mouse_sensitivity_slider, mouse_sensitivity_label) = SettingsMenuUtil.UnpacKSlider(slider_element);
        return slider_element;
    }

    private void UpdateDropdownOptions(DropdownField dropdown) {
        DifficultyLevel current_level = GameSettings.inst.difficulty_settings.difficulty_level;
        if (current_level == DifficultyLevel.custom) {
            AddCustomOption(dropdown);
        } else {
            RemoveCustomOption(dropdown);
        }
    }

    private void RemoveCustomOption(DropdownField dropdown) {
        // recreates the dropdown options without the "custom" option
        List<string> choices = new List<string>();
        foreach (DifficultyLevel level in Enum.GetValues(typeof(DifficultyLevel))) {
            if (level == DifficultyLevel.custom) { continue; }
            choices.Add(DifficultySettings.DifficultyLevelDisplay(level));
        }
        dropdown.choices = choices;
    }

    private void AddCustomOption(DropdownField dropdown) {
        List<string> choices = new List<string>();
        foreach (DifficultyLevel level in Enum.GetValues(typeof(DifficultyLevel))) {
            choices.Add(DifficultySettings.DifficultyLevelDisplay(level));
        }
        dropdown.choices = choices;
    }

    public override void SaveSettings() {
        // Saves the menu's changes to settings    
        GamePlaySettings settings = GameSettings.inst.game_play_settings;
        settings.mouse_sensitivity = mouse_sensitivity_slider.value;
    }

    public override void LoadSettings() {
        // sets the UI's elements to match what is stored in settings (reverting any changes)
        GamePlaySettings settings = GameSettings.inst.game_play_settings;
        mouse_sensitivity_slider.value = settings.mouse_sensitivity;
        UpdateUI();
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        SettingsMenuUtil.UpdateSliderValueDisplay(mouse_sensitivity_slider, mouse_sensitivity_label);
    }

    public override bool HasUnsavedChanges()
    {
        GamePlaySettings settings = GameSettings.inst.game_play_settings;
        return settings.mouse_sensitivity != mouse_sensitivity_slider.value;
    }

}