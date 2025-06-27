using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class DebugSettings : AbstractSettingsModule {

    private bool _show_fps = true;
    public bool show_fps {
        get { return _show_fps; }
        set {
            _show_fps = value;
            UpdateSubscribers("show_fps");
        }
    }
    private bool _debug_mode = false;
    public bool debug_mode {
        get { return _debug_mode; }
        set {
            _debug_mode = value;
            UpdateSubscribers("debug_mode");
        }
    }

    private bool _show_damage_numbers = false;
    public bool show_damage_numbers {
        get { return _show_damage_numbers; }
        set {
            _show_damage_numbers = value;
            UpdateSubscribers("show_damage_numbers");
        }
    }

    private bool _player_invincibility = false;
    public bool player_invincibility {
        get { return _player_invincibility; }
        set {
            _player_invincibility = value;
            UpdateSubscribers("player_invincibility");
        }
    }

    private bool _player_invisible = false;
    public bool player_invisible {
        get { return _player_invisible; }
        set {
            _player_invisible = value;
            UpdateSubscribers("player_invisible");
        }
    }

    private bool _allow_debug_actions = true;
    public bool allow_debug_actions {
        get { return _allow_debug_actions; }
        set {
            _allow_debug_actions = value;
            UpdateSubscribers("allow_debug_actions");
        }
    }

    private ShowGrenadeFuse _show_grenade_fuse = ShowGrenadeFuse.while_held;
    public ShowGrenadeFuse show_grenade_fuse {
        get => _show_grenade_fuse;
        set {
            _show_grenade_fuse = value;
            UpdateSubscribers("show_grenade_fuse");
        }
    }


    public override List<string> all_fields { get { return new List<string>() { "show_fps", "debug_mode" }; } }
    public override DuckDict AsDuckDict() {
        // returns json data for the settings in this module
        DuckDict data = new DuckDict();
        data.SetBool("show_fps", show_fps);
        data.SetBool("debug_mode", debug_mode);
        data.SetBool("show_damage_numbers", show_damage_numbers);
        data.SetBool("player_invincibility", player_invincibility);
        data.SetBool("player_invisible", player_invisible);
        data.SetBool("allow_debug_actions", allow_debug_actions);
        data.SetString("show_grenade_fuse", ShowGrenadeStringFromEnum(show_grenade_fuse));
        return data;
    }
    public override void LoadFromJson(DuckDict data) {
        // sets the settings module from a JSON string
        // DuckDict data = JsonParser.ReadAsDuckDict(json_str);
        show_fps = UnpackBool(data, "show_fps");
        debug_mode = UnpackBool(data, "debug_mode");
        show_damage_numbers = UnpackBool(data, "show_damage_numbers");
        player_invincibility = UnpackBool(data, "player_invincibility");
        player_invisible = UnpackBool(data, "player_invisible");
        allow_debug_actions = UnpackBool(data, "allow_debug_actions");
        show_grenade_fuse = UnpackGrenadeEnum(data, "show_grenade_fuse");
        this.AllFieldsUpdates();
    }

    private bool UnpackBool(DuckDict data, string field_name) {
        bool? v = data.GetBool(field_name);
        if (v == null) {
            v = false;
        }
        return (bool)v;
    }

    public const ShowGrenadeFuse show_grenade_default = ShowGrenadeFuse.default_value;
    private ShowGrenadeFuse UnpackGrenadeEnum(DuckDict data, string field_name) {
        string v = data.GetString(field_name);
        if (v == null) {
            Debug.LogError($"json missing ShowGrenadeFuse value at {field_name}");
            return ShowGrenadeFuse.default_value;
        }
        return ShowGrenadeEnumFromString(v);
    }

    public static ShowGrenadeFuse ShowGrenadeEnumFromString(string enum_string) {
        switch (enum_string) {
            case "default_value":
                return ShowGrenadeFuse.default_value;
            case "never":
                return ShowGrenadeFuse.never;
            case "while_held":
                return ShowGrenadeFuse.while_held;
            case "after_throw":
                return ShowGrenadeFuse.after_throw;
            case "always":
                return ShowGrenadeFuse.always;
            case null:
                Debug.LogError($"null string for ShowGrenadeFuse enum!");
                return ShowGrenadeFuse.default_value;
            default:
                Debug.LogError($"unhandled ShowGrenadeFuse enum string '{enum_string}'");
                return ShowGrenadeFuse.default_value;
        }
    }

    public static string ShowGrenadeStringFromEnum(ShowGrenadeFuse enum_val) {
        return $"{enum_val}";
    }
}

public enum ShowGrenadeFuse {
    default_value,
    never,
    while_held,
    after_throw,
    always,
}