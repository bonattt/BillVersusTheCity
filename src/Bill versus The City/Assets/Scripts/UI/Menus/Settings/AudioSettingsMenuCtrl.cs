using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class AudioSettingsMenuCtrl : AbstractSettingsModuleMenu {

    // private VisualElement root, settings_pannel, buttons_pannel; 
    private Slider master_volume;
    private Label master_volume_label; //, header_label;
    private Dictionary<string, Slider> volume_sliders = new Dictionary<string, Slider>();
    private Dictionary<string, Label> volume_slider_labels = new Dictionary<string, Label>();

    public readonly string[] sound_category_ordering = new string[]{
        AudioSettings.CategoryToString(SoundCategory.master),
        AudioSettings.CategoryToString(SoundCategory.sound_effect),
        AudioSettings.CategoryToString(SoundCategory.music),
        AudioSettings.CategoryToString(SoundCategory.menu),
    };
    // public AudioSettingsMenuCtrl() {
    //     // do nothing
    // }
    // public void RestoreToDefaultsClicked() {
    //     Debug.LogError("AudioSettingsMenuCtrl should extend AbstractSettingsMenuCtrl, instead of implementing from scratch!"); // TODO --- remove debug
    // }

    // return the SettingsModule this controller targets
    public override ISettingsModule settings_module { get { return GameSettings.inst.audio_settings; } }
    
    // takes the root element of the sub-menu, and configures the menu's controller
    public override void Initialize(VisualElement root) {
        // this.root = root;
        // settings_pannel = root.Q<VisualElement>("List");
        // buttons_pannel = root.Q<VisualElement>("Controlls");
        // header_label = root.Q<Label>("HeaderText");
        // header_label.text = "Audio Settings";

        // settings_pannel.Clear();
        LoadSettingsUXML(root);
        // (master_volume, master_volume_label) = AddVolumeSlider("Master Volume");
        for (int i = 0; i < sound_category_ordering.Length; i++) {
            string category = sound_category_ordering[i];
            (Slider new_slider, Label slider_label) = AddVolumeSlider(DisplayValue(category));
            if (new_slider == null) { Debug.LogWarning("Slider null!"); }
            volume_sliders[category] = new_slider;
            volume_slider_labels[category] = slider_label;
        }

        LoadSettings();
    }

    private (Slider, Label) AddVolumeSlider(string slider_label) {
        CustomSettingsSlider slider_element = SettingsMenuUtil.CreatePercentSlider(slider_label);
        settings_pannel.Add(slider_element);
        slider_element.UpdateValueLabel();
        return SettingsMenuUtil.UnpackSlider(slider_element);  // (slider_element.Q<Slider>(), slider_element.Q<Label>(SettingsMenuUtil.SLIDER_VALUE_LABEL));
    }

    public override void SaveSettings() {
        // Saves the menu's changes to settings    
        AudioSettings settings = GameSettings.inst.audio_settings;
        // settings.master_volume = master_volume.value;
        foreach (string category in sound_category_ordering) {
            settings.SetVolumeSetting(category, volume_sliders[category].value);
        }
    }

    public override void LoadSettings() {
        // sets the UI's elements to match what is stored in settings (reverting any changes)
        AudioSettings settings = GameSettings.inst.audio_settings;
        // master_volume.value = settings.master_volume;
        foreach (string category in sound_category_ordering) {
            volume_sliders[category].value = settings.GetVolumeSetting(category);
        }
        UpdateUI();
    }

    public override IEnumerable<string> UnsavedFields() {
        Debug.LogWarning("TODO --- implement `AudioSettingsMenuCtrl.UnsavedFields()`"); // TODO --- implement this
        return new List<string>();
    }
    
    public override bool HasUnsavedChanges() {
        AudioSettings settings = GameSettings.inst.audio_settings;
        foreach (string f in settings.float_field_names) {
            // if any setting doesn't match, shourt-circuit and return false
            if (volume_sliders[f].value != settings.GetVolumeSetting(f)) {
                return true;
            }
        }
        return false;
    }

    // public override void UpdateUI() {
    //     // // updates the UI
    //     // SettingsMenuUtil.UpdateSliderValueDisplay(master_volume, master_volume_label);
    //     // foreach (SoundCategory key in volume_sliders.Keys) {
    //     //     SettingsMenuUtil.UpdateSliderValueDisplay(volume_sliders[key], volume_slider_labels[key]);
    //     // }
    // }

    public static string DisplayValue(string category) {
        return DisplayValue(AudioSettings.CategoryFromString(category));
    }
    public static string DisplayValue(SoundCategory category) {
        switch (category) {
            case SoundCategory.master:
                return "Master";
            case SoundCategory.menu:
                return "Menu";
            case SoundCategory.music:
                return "Music";
            case SoundCategory.sound_effect:
                return "Sound Effect";

            default:
                Debug.LogWarning($"unknown sound category {category}!");
                return $"{category}";
        }
    }

    // public override void CleanUp() {
    //     // disposes of any resources that need to be cleaned when the sub-menu is closed
    //     // nothing to do here
    // }
}