using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DebugSettings : AbstractSettingsModule {

    private ShowGrenadeFuse _show_grenade_fuse = ShowGrenadeFuse.default_value;
    private const ShowGrenadeFuse DEFAULT_GRENADE_FUSE = ShowGrenadeFuse.never; 
    public ShowGrenadeFuse show_grenade_fuse {
        get {
            if (_show_grenade_fuse == ShowGrenadeFuse.default_value) { return DEFAULT_GRENADE_FUSE; }
            return _show_grenade_fuse;
        } set {
            _show_grenade_fuse = value;
            UpdateSubscribers("show_grenade_fuse");
        }
    }
    
    private static readonly List<string> BOOL_FIELDS = new List<string>() {
        "show_fps",
        "debug_mode",
        "show_damage_numbers",
        "player_invincibility",
        "player_invisible",
        "allow_debug_actions",
        "unlock_all_weapons",
        "unrestrict_weapon_slots",
    };
    public override List<string> bool_field_names { get => BOOL_FIELDS; }

    private static readonly List<string> OTHER_FIELDS = new List<string>() { "show_grenade_fuse" };
    public override List<string> other_field_names { get => OTHER_FIELDS; }
    
    protected override void InitializeMinMaxAndDefaults() {
        foreach (string f in bool_field_names) {
            bool_fields_default[f] = false;
        }
    }
    public override void RestoreToDefaults() {
        show_grenade_fuse = DEFAULT_GRENADE_FUSE;
        base.RestoreToDefaults();
    }

    public override DuckDict AsDuckDict() {
        // returns json data for the settings in this module
        DuckDict data = base.AsDuckDict();
        data.SetString("show_grenade_fuse", ShowGrenadeStringFromEnum(show_grenade_fuse));
        return data;
    }
    public override void LoadFromJson(DuckDict data, bool update_subscribers = true) {
        // sets the settings module from a JSON string
        show_grenade_fuse = UnpackGrenadeEnum(data, "show_grenade_fuse");
        base.LoadFromJson(data, update_subscribers);
        if (update_subscribers) {
            this.AllFieldsUpdated();
        }
    }

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