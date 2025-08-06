

using UnityEngine;

public class GraphicsSettingsUpdater : MonoBehaviour, ISettingsObserver {

    public static GraphicsSettingsUpdater inst { get; private set; }
    void Awake() {
        inst = this;
    }

    void Start() {
        GameSettings.inst.graphics_settings.Subscribe(this);
        UpdateScreenResolution();
    }

    public void SettingsUpdated(ISettingsModule updated, string field) {
        if (GraphicsSettings.SCREEN_RESOLUTION.Equals(field)) {
            UpdateScreenResolution();
        }
    }


    private void UpdateScreenResolution() {
        Debug.Log($"UpdateScreenResolution to {GameSettings.inst.graphics_settings.screen_resolution}");
        (int width, int height) = GameSettings.inst.graphics_settings.screen_dimensions;
        Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
    }
}