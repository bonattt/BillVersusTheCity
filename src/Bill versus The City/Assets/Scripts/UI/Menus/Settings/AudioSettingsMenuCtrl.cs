using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class AudioSettingsMenuCtrl : ISettingModuleMenu {

    private VisualElement root, settings_pannel, buttons_pannel; 
    private Slider master_volume;
    private Dictionary<SoundCategory, Slider> volume_sliders = new Dictionary<SoundCategory, Slider>();

    public readonly SoundCategory[] sound_category_ordering = new SoundCategory[]{
        SoundCategory.sound_effect,
        SoundCategory.music,
        SoundCategory.menu,
    };
    public AudioSettingsMenuCtrl() {
        // do nothing
    }

    // return the SettingsModule this controller targets
    public ISettingsModule settings_module { get { return GameSettings.inst.audio_settings; } }
    
    // takes the root element of the sub-menu, and configures the menu's controller
    public void Initialize(VisualElement root) {
        this.root = root;
        settings_pannel = root.Q<VisualElement>("List");
        buttons_pannel = root.Q<VisualElement>("Controlls");

        settings_pannel.Clear();
        master_volume = AddVolumeSlider("Master Volume");
        for (int i = 0; i < sound_category_ordering.Length; i++) {
            SoundCategory category = sound_category_ordering[i];
            Slider new_slider = AddVolumeSlider($"{category}");
            if (new_slider == null) { Debug.LogWarning("Slider null!"); }
            volume_sliders[category] = new_slider;
        }

        LoadSettings();
    }

    private Slider AddVolumeSlider(string slider_label) {
        VisualElement slider_element = SettingsMenuUtil.CreatePercentSlider(slider_label);
        settings_pannel.Add(slider_element);
        return slider_element.Q<Slider>();
    }

    public void SaveSettings() {
        // Saves the menu's changes to settings    
        AudioSettings settings = GameSettings.inst.audio_settings;
        settings.master_volume = master_volume.value;
        foreach (SoundCategory category in sound_category_ordering) {
            settings.SetVolumeSetting(category, volume_sliders[category].value);
        }
    }

    public void LoadSettings() {
        // sets the UI's elements to match what is stored in settings (reverting any changes)
        AudioSettings settings = GameSettings.inst.audio_settings;
        master_volume.value = settings.master_volume;
        foreach (SoundCategory category in sound_category_ordering) {
             volume_sliders[category].value = settings.GetVolumeSetting(category);
        }
    }

    public void CleanUp() {
        // disposes of any resources that need to be cleaned when the sub-menu is closed
        // nothing to do here
    }
}