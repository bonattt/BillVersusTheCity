using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public abstract class AbstractSettingsModuleMenu : ISettingModuleMenu {

    protected VisualElement root, settings_pannel, buttons_pannel; 
    protected Label header_label;

    // style-sheet class for any top-level item in a settings module menue
    public const string SETTINGS_ITEM_CLASS = "settings_item";

    public abstract ISettingsModule settings_module { get; }
    
    public abstract void Initialize(VisualElement root);

    protected virtual void LoadTemplate(VisualElement root) {
        // loads the root visaul element and finds required class field in the Visual Tree
        this.root = root;
        settings_pannel = root.Q<VisualElement>("List");
        buttons_pannel = root.Q<VisualElement>("Controlls");
        header_label = root.Q<Label>("HeaderText");
        header_label.text = "Settings";

        settings_pannel.Clear();
    }

    public void RestoreToDefaultsClicked() {
        YesNoPopupController popup = MenuManager.inst.OpenNewPopup();
        popup.header_text = "Reset to Defaults?";
        popup.content_text = "any changes you've made to this settings page will be lost";
        popup.UpdateLabels();
        popup.confirm_button.clicked += RestoreToDefaults;
        // popup.confirm_button.clicked += settings_module.RestoreToDefaults;
    }

    private void RestoreToDefaults() {
        // called when a restore to defaults is confirmed
        settings_module.RestoreToDefaults();
        LoadSettings(); // update the UI with whatever is in the settings module after restoring to default
        SaveSettings(); // apply the new change
    }

    public abstract void SaveSettings();

    public abstract void LoadSettings();

    public virtual IEnumerable<string> UnsavedFields() {
        Debug.LogWarning($"TODO --- implement UnsavedFields for {GetType()}"); // TODO --- make this method abstract
        return new List<string>();
    } 
    public abstract bool HasUnsavedChanges();

    public virtual void UpdateUI() {
        // updates the UI
        // nothing to do here, hook is in place to override in subclasses
    }

    public void CleanUp() {
        // disposes of any resources that need to be cleaned when the sub-menu is closed
        // nothing to do here
    }
}