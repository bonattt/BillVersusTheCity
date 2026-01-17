using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class DifficultySettings : AbstractSettingsModule {
    public DifficultyLevel difficulty_level { get; private set; }
    public const string DIFFICULTY_LEVEL = "difficulty_level";
    public const string PLAYER_ARMOR = "player_armor";
    public const string PLAYER_HEALTH = "player_health";
    public const string PLAYER_RELOAD_SPEED = "player_reload_speed";
    public const string PLAYER_FLASHBANG_EFFECTIVENESS = "player_flashbang_effectiveness";
    public const string ENEMY_ARMOR = "enemy_armor";
    public const string ENEMY_HEALTH = "enemy_health";
    public const string ENEMY_REACTION_TIME = "enemy_reaction_time";
    public const string ENEMY_RUN_SPEED = "enemy_run_speed";
    public const string ENEMY_RELOAD_SPEED = "enemy_reload_speed";
    public const string ENEMY_FLASHBANG_EFFECTIVENESS = "enemy_flashbang_effectiveness";


    public override List<string> float_field_names  => FIELDS;
    public override List<string> other_field_names  => new List<string>() {DIFFICULTY_LEVEL};
    public static readonly List<string> FIELDS = new List<string>(){
        PLAYER_ARMOR,
        PLAYER_HEALTH,
        PLAYER_RELOAD_SPEED,
        ENEMY_ARMOR,
        ENEMY_HEALTH,
        ENEMY_REACTION_TIME,
        ENEMY_RUN_SPEED,
        ENEMY_RELOAD_SPEED,
        ENEMY_FLASHBANG_EFFECTIVENESS,
        PLAYER_FLASHBANG_EFFECTIVENESS,
    };
    
    // public DifficultySettings() {
    //     RestoreToDefaults();
    // }

    private void SetFromTemplate(Dictionary<string, float> template) {
        float_fields = new Dictionary<string, float>();
        foreach (string key in FIELDS) {
            if (template.ContainsKey(key)) {
                SetMultiplier(key, template[key]);
            } else {
                if (float_fields.Count > 0) {
                    // don't log missing keys if the dictionary is completely empty, that means we're just setting default values.
                    Debug.Log($"Difficulty template missing key '{key}'");
                }
                SetMultiplier(key, 1f); // multiply by 1 is no change to the modified value
            }
        }
    }

    public void SetDifficultyLevel(DifficultyLevel level, bool apply_template=true) {
        if (apply_template) {
            SetFromTemplate(DifficultyTemplates.GetTemplate(level));
        }
        difficulty_level = level;
        AllFieldsUpdated();
    }

    public float GetMultiplier(string key) => GetFloat(key);
    public void SetMultiplier(string key, float value) {
        SetFloat(key, value);
        difficulty_level = DifficultyLevel.custom;
        UpdateSubscribers(key);
    }

    public static readonly Dictionary<DifficultyLevel, string> DISPLAY_VALUES = new Dictionary<DifficultyLevel, string>(){
        {DifficultyLevel.custom, "Custom"},
        {DifficultyLevel.easy, "Easy"},
        {DifficultyLevel.medium, "Medium"},
        {DifficultyLevel.hard, "Hard"}
    };

    protected override void InitializeMinMaxAndDefaults() {
        float_fields_max = new Dictionary<string, float>() {
            {PLAYER_ARMOR, 3f},
            {PLAYER_HEALTH, 3f},
            {PLAYER_RELOAD_SPEED, 3f},
            {PLAYER_FLASHBANG_EFFECTIVENESS, 3f},
            {ENEMY_ARMOR, 3f},
            {ENEMY_HEALTH, 3f},
            {ENEMY_REACTION_TIME, 3f},
            {ENEMY_RUN_SPEED, 1.7f},
            {ENEMY_RELOAD_SPEED, 3f},
            {ENEMY_FLASHBANG_EFFECTIVENESS, 3f},
        };
        float_fields_min = new Dictionary<string, float>() {
            {PLAYER_ARMOR, 0.25f},
            {PLAYER_HEALTH, 0.25f},
            {PLAYER_RELOAD_SPEED, 0.25f},
            {PLAYER_FLASHBANG_EFFECTIVENESS, 0.25f},
            {ENEMY_ARMOR, 0.25f},
            {ENEMY_HEALTH, 0.25f},
            {ENEMY_REACTION_TIME, 0.25f},
            {ENEMY_RUN_SPEED, 0.75f},
            {ENEMY_RELOAD_SPEED, 0.25f},
            {ENEMY_FLASHBANG_EFFECTIVENESS, 0.25f},
        };
        float_fields_default = new Dictionary<string, float>() {
            {PLAYER_ARMOR, 1f},
            {PLAYER_HEALTH, 1f},
            {PLAYER_RELOAD_SPEED, 1f},
            {PLAYER_FLASHBANG_EFFECTIVENESS, 1f},
            {ENEMY_ARMOR, 1f},
            {ENEMY_HEALTH, 1f},
            {ENEMY_REACTION_TIME, 1f},
            {ENEMY_RUN_SPEED, 1f},
            {ENEMY_RELOAD_SPEED, 1f},
            {ENEMY_FLASHBANG_EFFECTIVENESS, 1f},
        };
    }
    
    private static Dictionary<string, DifficultyLevel> REVERSE_DISPLAY_VALUES = null;
    public static string DifficultyLevelDisplay(DifficultyLevel level) {
        if (DISPLAY_VALUES.ContainsKey(level)) {
            return DISPLAY_VALUES[level];
        }
        Debug.LogError($"no display value for difficulty level '{level}'");
        return $"{level}";
    }

    public static DifficultyLevel LevelFromDisplay(string display_value) {
        // initialize reverse lookup for display values only once
        if (REVERSE_DISPLAY_VALUES == null) {
            REVERSE_DISPLAY_VALUES = new Dictionary<string, DifficultyLevel>();
            foreach(DifficultyLevel level in DISPLAY_VALUES.Keys) {
                REVERSE_DISPLAY_VALUES[DISPLAY_VALUES[level]] = level;
            }
        }
        if (! REVERSE_DISPLAY_VALUES.ContainsKey(display_value)) {
            Debug.LogError($"'{display_value}' is not recognized as a difficulty level");
            return DifficultyLevel.custom;
        }
        return REVERSE_DISPLAY_VALUES[display_value];
    }

    public override void RestoreToDefaults() {
        if (difficulty_level == DifficultyLevel.custom) {
            base.RestoreToDefaults();
            difficulty_level = DifficultyLevel.custom;
        } else {
            SetDifficultyLevel(difficulty_level, apply_template: true);
        }
        base.RestoreToDefaults();
    }

    public override DuckDict AsDuckDict()
    {
        // returns json data for the settings in this module
        DuckDict data = base.AsDuckDict();
        data.SetString(DIFFICULTY_LEVEL, DifficultyAsString(difficulty_level));
        return data;
    }
    
    public override void LoadFromJson(DuckDict module_save_data, bool update_subscribers=true) {
        // sets the settings module from a JSON string

        // // DuckDict data = JsonParser.ReadAsDuckDict(json_str);
        // foreach(string field in FIELDS) {
        //     float? value = module_save_data.GetFloat(field);
        //     if (value == null) {
        //         SetFloat(field, 1f);
        //     } else {
        //         SetFloat(field, (float) value);
        //     }
        // }
        difficulty_level = DifficultyFromString(module_save_data.GetString(DIFFICULTY_LEVEL));
        base.LoadFromJson(module_save_data, update_subscribers); // NOTE --- this is actively overwriting with bad data somehow
    }

    public static string DifficultyAsString(DifficultyLevel level) {
        return $"{level}";
    }

    public static DifficultyLevel DifficultyFromString(string level) {
        switch(level) {
            case "custom":
                return DifficultyLevel.custom;
            case "easy":
                return DifficultyLevel.easy;
            case "medium":
                return DifficultyLevel.medium;
            case "hard":
                return DifficultyLevel.hard;

            default:
                Debug.LogError($"unknown difficulty string '{level}'");
                return DifficultyLevel.custom;
        }
    }
}

public static class DifficultyTemplates {
    public static readonly Dictionary<string, float> EASY_TEMPLATE = new Dictionary<string, float>(){
        {DifficultySettings.PLAYER_ARMOR, 2f},
        {DifficultySettings.PLAYER_HEALTH, 2f},
        {DifficultySettings.PLAYER_RELOAD_SPEED, 1f},
        {DifficultySettings.PLAYER_FLASHBANG_EFFECTIVENESS, 1f},
        {DifficultySettings.ENEMY_ARMOR, 1f},
        {DifficultySettings.ENEMY_HEALTH, 0.5f},
        {DifficultySettings.ENEMY_REACTION_TIME, 2f},
        {DifficultySettings.ENEMY_RUN_SPEED, 1f},
        {DifficultySettings.ENEMY_RELOAD_SPEED, 1f},
        {DifficultySettings.ENEMY_FLASHBANG_EFFECTIVENESS, 1.15f},
    };

    public static readonly Dictionary<string, float> MEDIUM_TEMPLATE = new Dictionary<string, float>(){
        {DifficultySettings.PLAYER_ARMOR, 1f},
        {DifficultySettings.PLAYER_HEALTH, 1f},
        {DifficultySettings.PLAYER_RELOAD_SPEED, 1f},
        {DifficultySettings.PLAYER_FLASHBANG_EFFECTIVENESS, 1f},
        {DifficultySettings.ENEMY_ARMOR, 1f},
        {DifficultySettings.ENEMY_HEALTH, 1f},
        {DifficultySettings.ENEMY_REACTION_TIME, 1f},
        {DifficultySettings.ENEMY_RUN_SPEED, 1f},
        {DifficultySettings.ENEMY_RELOAD_SPEED, 1f},
        {DifficultySettings.ENEMY_FLASHBANG_EFFECTIVENESS, 1f},
    };
    

    public static readonly Dictionary<string, float> HARD_TEMPLATE = new Dictionary<string, float>(){
        {DifficultySettings.PLAYER_ARMOR, 0.5f},
        {DifficultySettings.PLAYER_HEALTH, 0.75f},
        {DifficultySettings.PLAYER_RELOAD_SPEED, 1f},
        {DifficultySettings.PLAYER_FLASHBANG_EFFECTIVENESS, 1f},
        {DifficultySettings.ENEMY_ARMOR, 3f},
        {DifficultySettings.ENEMY_HEALTH, 1.25f},
        {DifficultySettings.ENEMY_REACTION_TIME, 0.5f},
        {DifficultySettings.ENEMY_RUN_SPEED, 1f},
        {DifficultySettings.ENEMY_RELOAD_SPEED, 1f},
        {DifficultySettings.ENEMY_FLASHBANG_EFFECTIVENESS, 0.85f},
    };

    public static Dictionary<string, float> GetTemplate(DifficultyLevel level) {
        switch (level) {
            case DifficultyLevel.easy:
                return EASY_TEMPLATE;
            case DifficultyLevel.medium:
                return MEDIUM_TEMPLATE;
            case DifficultyLevel.hard:
                return HARD_TEMPLATE;
            case DifficultyLevel.custom:
                break; // do nothing, hit default return, but don't log a warning.
            default:
                Debug.LogWarning($"using un-expected DifficultyLevel '{level}'");
                break;
        }
        return new Dictionary<string, float>(); // empty template uses default values
    }
}


public enum DifficultyLevel {
    easy,  // well done
    medium, // medium
    hard, // rare
    // raw,   // it's bloody raw!
    custom
}