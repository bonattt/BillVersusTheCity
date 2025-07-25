


using System.Linq;
using UnityEngine.UIElements;

public class GraphicsSettingsMenu : AbstractSettingsModuleMenu {

    public DropdownField resolution_select;
    public override ISettingsModule settings_module { get => GameSettings.inst.graphics_settings; }
    public GraphicsSettings settings { get => GameSettings.inst.graphics_settings; }

    public ScreenResolution screen_resolution {
        get {
            return GraphicsSettings.ScreenResolutionFromString(resolution_select.value);
        }
        set {
            resolution_select.value = GraphicsSettings.ScreenResolutionToString(value);
        }
    }

    public override void Initialize(VisualElement root) {
        LoadSettingsUXML(root);

        resolution_select = GetResolutionDropdown();
        settings_pannel.Add(resolution_select);
    }

    
    private DropdownField GetResolutionDropdown() {
        DropdownField dropdown = new DropdownField();
        dropdown.name = "ScreenResolution";
        dropdown.AddToClassList(SETTINGS_ITEM_CLASS);

        dropdown.choices = GraphicsSettings.GetScreenResolutions().ToList();
        dropdown.value = GraphicsSettings.ScreenResolutionToString(settings.screen_resolution);
        return dropdown;
    }
    public override void SaveSettings() {
        // ScreenResolution res = GraphicsSettings.ScreenResolutionFromString(resolution_select.value);
        settings.screen_resolution = screen_resolution;
    }
    public override void LoadSettings() {
        screen_resolution = settings.screen_resolution;
    }
    public override bool HasUnsavedChanges() {
        return screen_resolution != settings.screen_resolution;
    }
}