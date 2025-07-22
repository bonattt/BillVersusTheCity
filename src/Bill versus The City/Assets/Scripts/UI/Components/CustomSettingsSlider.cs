

using UnityEngine;
using UnityEngine.UIElements;

public class CustomSettingsSlider : VisualElement {
    // A class that incorporates a slider, field-name label, and value label in a single container, with some styles to make them look consistent.

    private Label value_display, field_name_display;
    private Slider slider;
    private VisualElement slider_container;

    public float value {
        get => slider.value;
        set {
            slider.value = value;
            UpdateValueLabel();
        }
    }

    public string label_text {
        get => field_name_display.text;
        set => field_name_display.text = value;
    }

    public string displayed_value {
        get => value_display.text;
    }

    public float display_multiplier = 1f;
    public int diplay_rounding_places = 0;
    public string diplay_display_prefix = "";
    public string diplay_display_postfix = "";

    public CustomSettingsSlider(string text, float min_value, float max_value) {
        this.name = text;
        this.AddToClassList(SettingsMenuUtil.SLIDER_PARENT_CLASS);

        field_name_display = new Label();
        field_name_display.name = "SliderLabel";
        field_name_display.text = text;
        field_name_display.AddToClassList(SettingsMenuUtil.SLIDER_LABEL_CLASS);
        this.Add(field_name_display);

        slider_container = new VisualElement();
        slider_container.AddToClassList(SettingsMenuUtil.SLIDER_CONTAINER_INNER_CLASS);
        this.Add(slider_container);

        value_display = new Label();
        value_display.name = SettingsMenuUtil.SLIDER_VALUE_LABEL;
        value_display.text = "";
        value_display.AddToClassList(SettingsMenuUtil.SLIDER_VALUE_CLASS);
        slider_container.Add(value_display);

        slider = new Slider();
        slider.name = "Slider";
        slider.label = "";
        slider.lowValue = min_value;
        slider.highValue = max_value;
        // // these should be dynamically set, but for testing purposes, the coinflip makes it easier to quickly see where the sliders are 
        slider.direction = SliderDirection.Horizontal;
        slider.AddToClassList(SettingsMenuUtil.SLIDER_TRACKER_CLASS);
        slider_container.Add(slider);

        SettingsMenuUtil.ApplySettingsItemClasses(this);
        slider.RegisterValueChangedCallback(event_ => UpdateValueLabel());
        UpdateValueLabel();
    }

    public void UpdateValueLabel() {
        // updates the label that displays the value
        value_display.text = GetAdjustDisplayValue(slider.value);
    }

    public string GetAdjustDisplayValue(float base_value) {
        float output_value = base_value * display_multiplier;
        float places_multiplier = Mathf.Pow(10, diplay_rounding_places);
        output_value = Mathf.Round(output_value * places_multiplier) / places_multiplier;
        return $"{diplay_display_prefix}{output_value}{diplay_display_postfix}";
    }

}