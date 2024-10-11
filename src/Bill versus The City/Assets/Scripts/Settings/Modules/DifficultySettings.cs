using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class DifficultySettings : AbstractSettingsModule {
    private Dictionary<string, float> multipliers;
    public DifficultyLevel difficulty_level { get; private set; }
    public const string DIFFICULTY_LEVEL = "difficulty_level";
    public const string PLAYER_ARMOR = "player_armor";
    public const string PLAYER_HEALTH = "player_health";
    public const string ENEMY_ARMOR = "enemy_armor";
    public const string ENEMY_HEALTH = "enemy_health";

    public readonly List<string> FIELDS = new List<string>(){
        PLAYER_ARMOR,
        PLAYER_HEALTH,
        ENEMY_ARMOR,
        ENEMY_HEALTH,
    };
    
    public DifficultySettings() {
        InitializeDefaults(); 
    }
    private void InitializeDefaults() {
        // Initialize from empty dict sets all fields to default
        difficulty_level = DifficultyLevel.custom;
        SetFromTemplate(new Dictionary<string, float>());
    }

    private void SetFromTemplate(Dictionary<string, float> template) {
        multipliers = new Dictionary<string, float>();
        foreach (string key in FIELDS) {
            if (template.ContainsKey(key)) {
                SetMultiplier(key, template[key]);
            } else {
                if (multipliers.Count > 0) {
                    // don't log missing keys if the dictionary is completely empty, that means we're just setting default values.
                    Debug.Log($"Difficulty template missing key '{key}'");
                }
                SetMultiplier(key, 1f); // multiply by 1 is no change to the modified value
            }
        }
    }

    public void SetDifficultyLevel(DifficultyLevel level) {
        SetFromTemplate(DifficultyTemplates.GetTemplate(level));
        difficulty_level = level;
        AllFieldsUpdates();
    }

    public float GetMultiplier(string key) {
        return multipliers[key];
    }

    public void SetMultiplier(string key, float value) {
        multipliers[key] = value;
        difficulty_level = DifficultyLevel.custom;
        UpdateSubscribers(key);
    }

    public static readonly Dictionary<DifficultyLevel, string> DISPLAY_VALUES = new Dictionary<DifficultyLevel, string>(){
        {DifficultyLevel.custom, "Custom"},
        {DifficultyLevel.easy, "Easy"},
        {DifficultyLevel.medium, "Medium"},
        {DifficultyLevel.hard, "Hard"}
    };

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

    public override List<string> all_fields {
        get { 
            List<string> fields = new List<string>(FIELDS);
            fields.Add(DIFFICULTY_LEVEL);
            return fields; 
        }
    }
    
    public override string AsJson() {
        // returns json data for the settings in this module
        DuckDict data = new DuckDict();
        data.SetString(DIFFICULTY_LEVEL, DifficultyAsString(difficulty_level));
        foreach(string field in multipliers.Keys) {
            data.SetFloat(field, multipliers[field]);
        }

        return data.Jsonify();
    }
    
    public override void LoadFromJson(string json_str) {
        // sets the settings module from a JSON string
        DuckDict data = JsonParser.ReadAsDuckDict(json_str);
        foreach(string field in FIELDS) {
            float? value = data.GetFloat(field);
            if (value == null) {
                multipliers[field] = 1f;
            } else {
                multipliers[field] = (float) value;
            }
        }
        difficulty_level = DifficultyFromString(data.GetString(DIFFICULTY_LEVEL));
        this.AllFieldsUpdates();
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
        {DifficultySettings.ENEMY_ARMOR, 1f},
        {DifficultySettings.ENEMY_HEALTH, 0.5f},
    };

    public static readonly Dictionary<string, float> MEDIUM_TEMPLATE = new Dictionary<string, float>(){
        {DifficultySettings.PLAYER_ARMOR, 1f},
        {DifficultySettings.PLAYER_HEALTH, 1f},
        {DifficultySettings.ENEMY_ARMOR, 1f},
        {DifficultySettings.ENEMY_HEALTH, 1f},
    };
    

    public static readonly Dictionary<string, float> HARD_TEMPLATE = new Dictionary<string, float>(){
        {DifficultySettings.PLAYER_ARMOR, 1f},
        {DifficultySettings.PLAYER_HEALTH, 0.75f},
        {DifficultySettings.ENEMY_ARMOR, 3f},
        {DifficultySettings.ENEMY_HEALTH, 1.5f},
    };

    public static Dictionary<string, float> GetTemplate(DifficultyLevel level) {
        switch(level) {
            case DifficultyLevel.easy:
                return EASY_TEMPLATE;
            case DifficultyLevel.medium:
                return MEDIUM_TEMPLATE;
            case DifficultyLevel.hard:
                return HARD_TEMPLATE;
            default:
                Debug.LogWarning($"using un-expected DifficultyLevel '{level}'");
                return new Dictionary<string, float>(); // empty template uses default values
        }
    }
}


public enum DifficultyLevel {
    easy,  // well done
    medium, // medium
    hard, // rare
    // raw,   // it's bloody raw!
    custom
}