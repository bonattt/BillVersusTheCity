using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DifficultySettingsMenuCtrl : AbstractSettingsModuleMenu {

    private DropdownField difficulty_select;

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

        LoadSettings();
    }

    

    private DropdownField GetDifficultyDropdown() {
        // DifficultyLevel current_level = GameSettings.inst.difficulty_settings.difficulty_level;
        // DropdownField dropdown = new DropdownField(DifficultySettings.DifficultyLevelDisplay(current_level));
        DropdownField dropdown = new DropdownField();
        dropdown.name = "Difficulty";
        // dropdown.text = "Difficulty";
        
        UpdateDropdownOptions(dropdown);
        return dropdown;
    }

    private void UpdateDropdownOptions(DropdownField dropdown) {
        DifficultyLevel current_level = GameSettings.inst.difficulty_settings.difficulty_level;
        if (current_level == DifficultyLevel.custom) {
            RemoveCustomOption(dropdown);
        } else {
            AddCustomOption(dropdown);
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
        DifficultySettings settings = GameSettings.inst.difficulty_settings;
        settings.SetDifficultyLevel(GetSelectedLevel());
    }

    private DifficultyLevel GetSelectedLevel() {
        return DifficultySettings.LevelFromDisplay(difficulty_select.value);
    }

    public override void LoadSettings() {
        // sets the UI's elements to match what is stored in settings (reverting any changes)
        DifficultySettings settings = GameSettings.inst.difficulty_settings;
        UpdateDropdownOptions(difficulty_select);
        difficulty_select.value = DifficultySettings.DifficultyLevelDisplay(settings.difficulty_level);
        UpdateUI();
    }

    public override bool HasUnsavedChanges()
    {
        DifficultySettings settings = GameSettings.inst.difficulty_settings;
        return GetSelectedLevel() != settings.difficulty_level;
    }

}