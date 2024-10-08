using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


public static class SettingsMenuUtil {

    public const string SLIDER_CLASS = "settings_slider"; // top level class for all sliders
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

    public static readonly string[] SETTINGS_ITEM_CLASSES = new string[]{"settings_item"};
    
    public static void UpdateSliderValueDisplay(Slider slider, Label label) {
        // updates the value display of a percent slider to show a percent 0-100
        label.text = $"{Mathf.Round(slider.value * 100)}";
    }

    public static VisualElement CreateSlider(string text, float min_value, float max_value) {
        // creates a slider for controlling percent fields (0-1f)
        VisualElement parent = new VisualElement();
        parent.name = text;
        parent.AddToClassList(SLIDER_CLASS);
        
        Label label = new Label();
        label.name = "SliderLabel";
        label.text = text;
        label.AddToClassList(SLIDER_LABEL_CLASS);
        parent.Add(label);

        VisualElement slider_container = new VisualElement();
        slider_container.AddToClassList(SLIDER_CONTAINER_INNER_CLASS);
        parent.Add(slider_container);

        Label slider_value = new Label();
        slider_value.name = SLIDER_VALUE_LABEL;
        slider_value.text = "9001";
        slider_value.AddToClassList(SLIDER_VALUE_CLASS);
        slider_container.Add(slider_value);
        
        Slider slider = new Slider();
        slider.name = "Slider";
        slider.label = "";
        slider.lowValue = min_value;
        slider.highValue = max_value;
        // // these should be dynamically set, but for testing purposes, the coinflip makes it easier to quickly see where the sliders are 
        // slider.value = Mathf.Round(UnityEngine.Random.Range(0.1f, 0.9f));
        slider.direction = SliderDirection.Horizontal;
        slider.AddToClassList(SLIDER_TRACKER_CLASS);
        slider_container.Add(slider);

        ApplySettingsItemClasses(parent);
        return parent;
    }

    public static (Slider, Label) UnpacKSlider(VisualElement slider_element) {
        // unpacks the Slider and Label components packaged by the parent slider VisualElement
        return (slider_element.Q<Slider>(), slider_element.Q<Label>(SettingsMenuUtil.SLIDER_VALUE_LABEL));
    }

    public static VisualElement CreatePercentSlider(string text) {
        // creates a slider for controlling percent fields (0-1f)
        VisualElement slider = CreateSlider(text, 0f, 1f);
        slider.AddToClassList(PERCENT_SLIDER_CLASS);
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
        foreach(string style_class in SETTINGS_ITEM_CLASSES) {
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