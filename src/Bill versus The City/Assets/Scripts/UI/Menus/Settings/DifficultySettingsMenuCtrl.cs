using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class DifficultySettingsMenuCtrl : AbstractSettingsModuleMenu {

    private DropdownField difficulty_select;

    private List<VisualElement> custom_difficulty_controls = new List<VisualElement>();
    private Dictionary<string, Slider> multiplier_sliders = new Dictionary<string, Slider>();

    public DifficultySettingsMenuCtrl() {
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

        difficulty_select = GetDifficultyDropdown();
        settings_pannel.Add(difficulty_select);

        foreach (string field_name in DifficultySettings.FIELDS) {
            Slider new_slider = GetMultiplierSlider(field_name);
            custom_difficulty_controls.Add(new_slider);
            settings_pannel.Add(new_slider);
        }

        LoadSettings();
        UpdateCustomControls(GameSettings.inst.difficulty_settings.difficulty_level);
        difficulty_select.RegisterValueChangedCallback((event_) => DifficultySettingUpdated(DifficultySettings.LevelFromDisplay(event_.newValue)));
    }

    public void DifficultySettingUpdated(DifficultyLevel difficulty_level) {
        UpdateMultipliersFromTemplate(difficulty_level); // changing difficulty level may load value from templates
        UpdateCustomControls(difficulty_level);
    }

    public void UpdateMultipliersFromTemplate(DifficultyLevel level) {
        Dictionary<string, float> template = DifficultyTemplates.GetTemplate(level);
        // NOTE: templates can exclude fields, in which case, the value should be left as whatever it was before loading the template
        foreach (string field_name in template.Keys) {
            multiplier_sliders[field_name].value = template[field_name];
        }
    }

    public void UpdateCustomControls(DifficultyLevel difficulty_level) {
        // sets custom controls to be enabled if difficulty level is custom, else disable them.
        foreach (VisualElement ve in custom_difficulty_controls) {
            ve.SetEnabled(difficulty_level == DifficultyLevel.custom);
        }
    }

    private Slider GetMultiplierSlider(string field_name) {
        DifficultySettings settings = GameSettings.inst.difficulty_settings;
        Slider new_slider = new Slider(field_name, settings.GetMin(field_name), settings.GetMax(field_name));
        new_slider.value = settings.GetMultiplier(field_name);
        new_slider.AddToClassList(SETTINGS_ITEM_CLASS);
        multiplier_sliders[field_name] = new_slider;
        return new_slider;
    }

    private DropdownField GetDifficultyDropdown() {
        DropdownField dropdown = new DropdownField();
        dropdown.name = "Difficulty";
        dropdown.AddToClassList(SETTINGS_ITEM_CLASS);

        UpdateDropdownOptions(dropdown);
        return dropdown;
    }

    private void UpdateDropdownOptions(DropdownField dropdown) {
        DifficultyLevel current_level = GameSettings.inst.difficulty_settings.difficulty_level;
        AddCustomOption(dropdown);
        // if (current_level == DifficultyLevel.custom) {
        //     AddCustomOption(dropdown);
        // } else {
        //     RemoveCustomOption(dropdown);
        // }
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
        DifficultySettings settings = GameSettings.inst.difficulty_settings;
        settings.SetDifficultyLevel(GetSelectedLevel());
        if (settings.difficulty_level == DifficultyLevel.custom) {
            ApplyMultipliersToSettings();
        }
    }

    private DifficultyLevel GetSelectedLevel() {
        return DifficultySettings.LevelFromDisplay(difficulty_select.value);
    }

    public override void LoadSettings() {
        // sets the UI's elements to match what is stored in settings (reverting any changes)
        DifficultySettings settings = GameSettings.inst.difficulty_settings;
        UpdateDropdownOptions(difficulty_select);
        difficulty_select.value = DifficultySettings.DifficultyLevelDisplay(settings.difficulty_level);
        UpdateMultipliersFromSettings();
        UpdateUI();
    }

    public override void UpdateUI() {
        base.UpdateUI();
    }

    public void UpdateMultipliersFromSettings() {
        DifficultySettings settings = GameSettings.inst.difficulty_settings;
        Debug.LogWarning($"UpdateMultipliersFromSettings(settings: {settings.difficulty_level}, ui: {GetSelectedLevel()})"); // TODO --- remove debug
        foreach (string field_name in DifficultySettings.FIELDS) {
            multiplier_sliders[field_name].value = settings.GetMultiplier(field_name);
        }
    }

    public void ApplyMultipliersToSettings() {
        DifficultySettings settings = GameSettings.inst.difficulty_settings;
        foreach (string field_name in DifficultySettings.FIELDS) {
            settings.SetMultiplier(field_name, multiplier_sliders[field_name].value);
        }
    }

    public override bool HasUnsavedChanges() {
        DifficultySettings settings = GameSettings.inst.difficulty_settings;
        if (GetSelectedLevel() != settings.difficulty_level) {
            return true;
        }
        foreach (string field_name in DifficultySettings.FIELDS) {
            if (multiplier_sliders[field_name].value != settings.GetMultiplier(field_name)) {
                return true;
            }
        }
        return false;
    }

}