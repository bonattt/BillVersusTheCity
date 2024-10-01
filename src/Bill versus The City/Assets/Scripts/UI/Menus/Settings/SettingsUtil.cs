using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


public static class SettingsMenuUtil {

    public const string SLIDER_CLASS = "settings_slider_container";
    public const string PERCENT_SLIDER_CLASS = "settings_percent_slider";
    public const string SLIDER_LABEL_CLASS = "settings_slider_label";
    public const string SLIDER_TRACKER_CLASS = "settings_slider_tracker";


    public static readonly string[] SETTINGS_ITEM_CLASSES = new string[]{"settings_item"};

    public static VisualElement CreatePercentSlider(string text) {
        // creates a slider for controlling percent fields (0-1f)
        VisualElement parent = new VisualElement();
        parent.name = text;
        parent.AddToClassList(PERCENT_SLIDER_CLASS);
        parent.AddToClassList(SLIDER_CLASS);
        
        Label label = new Label();
        label.name = "Label";
        label.text = text;
        label.AddToClassList(SLIDER_LABEL_CLASS);
        parent.Add(label);

        Slider slider = new Slider();
        slider.name = "Slider";
        slider.label = "";
        slider.lowValue = 0f;
        slider.highValue = 1f;
        // these should be dynamically set, but for testing purposes, the coinflip makes it easier to quickly see where the sliders are 
        slider.value = Mathf.Round(UnityEngine.Random.Range(0.1f, 0.9f));
        slider.direction = SliderDirection.Horizontal;
        slider.AddToClassList(SLIDER_TRACKER_CLASS);
        parent.Add(slider);

        ApplySettingsItemClasses(parent);
        return parent;
    }

    public static void LogChildren(VisualElement element) {
        string msg = $"'{element}' has {element.Children().Count()} children:";
        foreach (VisualElement child in element.Children()) {
            msg += $"\n\t{child}"; 
        }
        Debug.Log(msg);
    }

    private static Label GetSliderLabel(Slider slider) {
        /** Gets the Label element of a slider */
        // return slider.Q<Label>("unity-slider__label");
        return (Label) slider.ElementAt(0);
    }

    private static VisualElement GetSliderTracker(Slider slider) {
        /** gets the VisualElement of a Slider which contains the actual slider */
        // return slider.Q<VisualElement>("unity-slider__tracker");
        return slider.ElementAt(1);
    }

    public static void ApplySettingsItemClasses(VisualElement element) {
        foreach(string style_class in SETTINGS_ITEM_CLASSES) {
            element.AddToClassList(style_class);
        }
    }

    public static VisualElement CreateSettingsControlButtons(ISettingModuleMenu menu, VisualElement controls) {
        // creates a visual element containing the save/revert menu buttons
        if (controls == null) {
            controls = new VisualElement();
        }
        controls.Clear();
        controls.name = "Controls";
        controls.AddToClassList("settings_control_buttons_pannel");

        Button save_button = new Button();
        save_button.text = "Apply";
        save_button.AddToClassList("settings_control_button");
        save_button.RegisterCallback<ClickEvent>(MenuManager.PlayMenuClick);
        save_button.clicked += menu.SaveSettings;
        controls.Add(save_button);

        Button load_button = new Button();
        load_button.text = "Revert";
        load_button.AddToClassList("settings_control_button");
        load_button.RegisterCallback<ClickEvent>(MenuManager.PlayMenuCancelClick);
        load_button.clicked += menu.LoadSettings;
        controls.Add(load_button);

        return controls;

    }

}