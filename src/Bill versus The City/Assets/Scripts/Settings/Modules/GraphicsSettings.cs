using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class GraphicsSettings : AbstractSettingsModule {

    public const string SCREEN_RESOLUTION = "screen_resolution";
    public const ScreenResolution DEFAULT_RESOLUTION = ScreenResolution._1920x1080;
    public override List<string> other_field_names { get => new List<string>() { SCREEN_RESOLUTION }; }
    public static readonly List<string> FIELDS = new List<string>(){
        SCREEN_RESOLUTION,
    };

    private ScreenResolution _screen_resolution { get; set; }
    public ScreenResolution screen_resolution {
        get => _screen_resolution;
        set {
            _screen_resolution = value;
            UpdateSubscribers(SCREEN_RESOLUTION);
        }
    }
    public (int, int) screen_dimensions { get => GetScreenResolution(screen_resolution); }

    public static readonly Dictionary<ScreenResolution, string> DISPLAY_VALUES = new Dictionary<ScreenResolution, string>(){
        {ScreenResolution._1366x768, "1366 x 768"},
        {ScreenResolution._1920x1080, "1920 x 1080"},
        {ScreenResolution._2560x1440, "2560 x 1440"},
        {ScreenResolution._3840x2160, "3840 x 2160"},
    };

    protected override void InitializeMinMaxAndDefaults() {
        // nothing to do... yet
    }

    public static string ScreenResolutionDisplay(ScreenResolution resolution) {
        if (resolution == ScreenResolution.error) { return "Error"; }
        (int width, int height) = GetScreenResolution(resolution);
        return $"{width} X {height}";
    }

    public static (int, int) GetScreenResolution(ScreenResolution res) {
        if (res == ScreenResolution.error) { throw new GraphicsSettingsError("cannot get resolution of ERROR"); }
        switch (res) {
            case ScreenResolution._1366x768:
                return (1366, 768);
            case ScreenResolution._1920x1080:
                return (1920, 1080);
            case ScreenResolution._2560x1440:
                return (2560, 1440);
            case ScreenResolution._3840x2160:
                return (3840, 2160);
            default:
                throw new GraphicsSettingsError($"unhandled screen resolution '{res}'!");
        }
    }

    public override void RestoreToDefaults() {
        _screen_resolution = DEFAULT_RESOLUTION;
        base.RestoreToDefaults();
    }

    public override DuckDict AsDuckDict() {
        // returns json data for the settings in this module
        DuckDict data = base.AsDuckDict();
        data.SetString(SCREEN_RESOLUTION, ScreenResolutionToString(screen_resolution));
        return data;
    }

    public override void LoadFromJson(DuckDict module_save_data, bool update_subscribers = true) {
        // sets the settings module from a JSON string
        screen_resolution = ScreenResolutionFromString(module_save_data.GetString(SCREEN_RESOLUTION));
        base.LoadFromJson(module_save_data, update_subscribers); // NOTE --- this is actively overwriting with bad data somehow
    }


    public static string ScreenResolutionToString(ScreenResolution resolution) {
        if (resolution == ScreenResolution.error) { return "error"; }
        return $"{resolution}".Substring(1);
    }

    public static ScreenResolution ScreenResolutionFromString(string res_str) {
        if (res_str == null) {
            Debug.LogError($"screen resolution string is null!");
            return ScreenResolution.error;
        }
        foreach (ScreenResolution res_enum in GetScreenResolutionEnums()) {
            if (res_str.Equals(ScreenResolutionToString(res_enum))) {
                return res_enum;
            }
        }
        Debug.LogError($"ScreenResolution string '{res_str}' could not be matched!");
        return ScreenResolution.error;
    }

    public static IEnumerable<string> GetScreenResolutions() {
        foreach (ScreenResolution res_enum in GetScreenResolutionEnums()) {
            yield return ScreenResolutionToString(res_enum);
        }
    }
    public static IEnumerable<ScreenResolution> GetScreenResolutionEnums() {
        foreach (ScreenResolution res in Enum.GetValues(typeof(ScreenResolution))) {
            if (res == ScreenResolution.error) { continue; }
            yield return res;
        }
    }

}

public enum ScreenResolution {
    _1366x768,
    _1920x1080,
    _2560x1440,
    _3840x2160,
    error, // error
}

public class GraphicsSettingsError : Exception {
    public GraphicsSettingsError(string msg) : base(msg) { /* do nothing */ }
}