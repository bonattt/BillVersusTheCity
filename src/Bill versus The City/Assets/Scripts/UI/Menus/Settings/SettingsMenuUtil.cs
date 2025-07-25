using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


public static class SettingsMenuUtil {

    public const string SLIDER_PARENT_CLASS = "settings_slider"; // top level class for all sliders
    public const string SLIDER_CONTAINER_INNER_CLASS = "settings_slider_container_inner";  // for the Element containing the slider and it's value display
    public const string PERCENT_SLIDER_CLASS = "settings_percent_slider"; // top level class for percent sliders
    public const string SLIDER_LABEL_CLASS = "settings_slider_label";  // for Label displaying the field for the slider
    public const string SLIDER_TRACKER_CLASS = "settings_slider_tracker"; // for actual slider element
    public const string SLIDER_VALUE_CLASS = "settings_slider_value"; // for Label displaying slider value

    public const string SLIDER_VALUE_LABEL = "SliderValue"; // name of the Label used to display the value of a slider

    // public static VisualElement EmptySettingsModule(ISettingModuleMenu menu_ctrl, string name) {
    //     VisualElement root = new VisualElement();
    //     root.name = "root";

    //     VisualElement panel = new VisualElement();
    //     panel.name = "panel";
    //     root.Add(panel);

    //     Label header_label = new Label();
    //     header_label.name = "HeaderText";
    //     header_label.text = name;
    //     panel.Add(header_label);

    //     VisualElement list = new VisualElement();
    //     list.name = "List";
    //     list.AddToClassList("menu_settings_list");
    //     panel.Add(list);

    //     VisualElement controls = CreateSettingsControlButtons(menu_ctrl);
    //     panel.Add(controls);

    //     return root;
    // }

    public static readonly string[] SETTINGS_ITEM_CLASSES = new string[] { "settings_item" };

    public static void UpdateSliderValueDisplay(Slider slider, Label label) {
        // updates the value display of a percent slider to show a percent 0-100
        label.text = $"{Mathf.Round(slider.value * 100)}";
    }

    // public static VisualElement CreateSlider(string text, float min_value, float max_value) {
    //     // creates a slider for controlling percent fields (0-1f)
    //     return new CustomSettingsSlider(text, min_value, max_value);
    // }

    public static (Slider, Label) UnpackSlider(VisualElement slider_element) {
        // unpacks the Slider and Label components packaged by the parent slider VisualElement
        return (slider_element.Q<Slider>(), slider_element.Q<Label>(SettingsMenuUtil.SLIDER_VALUE_LABEL));
    }

    public static CustomSettingsSlider CreatePercentSlider(string text, float min = 0f, float max = 1f) {
        // creates a slider for controlling percent fields (0-1f)
        CustomSettingsSlider slider = new CustomSettingsSlider(text, min, max);
        slider.AddToClassList(PERCENT_SLIDER_CLASS);
        slider.display_multiplier = 100;
        slider.diplay_rounding_places = 0;
        slider.diplay_display_prefix = "";
        slider.diplay_display_postfix = "%";
        return slider;
    }

    public static void LogChildren(VisualElement element) {
        string msg = $"'{element}' has {element.Children().Count()} children:";
        foreach (VisualElement child in element.Children()) {
            msg += $"\n\t{child}";
        }
        Debug.Log(msg);
    }

    public static void ApplySettingsItemClasses(VisualElement element) {
        foreach (string style_class in SETTINGS_ITEM_CLASSES) {
            element.AddToClassList(style_class);
        }
    }


    public static VisualElement CreateSettingsControlButtons(ISettingModuleMenu menu) {
        return CreateSettingsControlButtons(menu, null);
    }

    public static VisualElement CreateSettingsControlButtons(ISettingModuleMenu menu, VisualElement controls) {
        // creates a visual element containing the save/revert menu buttons
        if (controls == null) {
            controls = new VisualElement();
        }
        controls.Clear();
        controls.name = "Controls";
        controls.AddToClassList("settings_control_buttons_pannel");

        CreateSettingsControlButton("Apply\nSettings", menu.SaveSettings, controls);
        CreateSettingsControlButton("Revert\nChanges", menu.LoadSettings, controls);
        CreateSettingsControlButton("Restore\nDefaults", menu.RestoreToDefaultsClicked, controls);
        CreateSettingsControlButton("Close\nMenu", MenuManager.inst.CloseMenu, controls);

        return controls;
    }

    private static Button CreateSettingsControlButton(string button_text, Action Callback, VisualElement parent) {
        Button new_button = new Button();
        new_button.text = button_text;
        new_button.AddToClassList("settings_control_button");
        new_button.clicked += Callback;
        new_button.RegisterCallback<ClickEvent>(MenuManager.PlayMenuClick);
        parent.Add(new_button);
        return new_button;
    }

}