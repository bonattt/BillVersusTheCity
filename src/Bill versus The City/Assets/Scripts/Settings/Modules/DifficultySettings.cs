using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class DifficultySettings : AbstractSettingsModule {
    private Dictionary<string, float> multipliers;

    public const string PLAYER_DAMAGE = "player_damage";
    public const string PLAYER_ARMOR = "player_armor";
    public const string PLAYER_HEALTH = "player_health";
    public const string PLAYER_AMMO = "player_ammo";
    public const string ENEMY_DAMAGE = "enemy_damage";
    public const string ENEMY_ARMOR = "enemy_armor";
    public const string ENEMY_HEALTH = "enemy_health";
    public const string ENEMY_AMMO = "enemy_ammo";

    public readonly HashSet<string> FIELDS = new HashSet<string>(){
        PLAYER_DAMAGE,
        PLAYER_ARMOR,
        PLAYER_HEALTH,
        PLAYER_AMMO,
        ENEMY_DAMAGE,
        ENEMY_ARMOR,
        ENEMY_HEALTH,
        ENEMY_AMMO
    };
    
    public DifficultySettings() {
        InitializeDefaults(); 
    }
    private void InitializeDefaults() {
        // Initialize from empty dict sets all fields to default
        SetFromTemplate(new Dictionary<string, float>());
    }

    private void SetFromTemplate(Dictionary<string, float> template) {
        multipliers = new Dictionary<string, float>();
        foreach (string key in FIELDS) {
            if (template.ContainsKey(key)) {
                multipliers[key] = template[key];
            } else {
                multipliers[key] = 1f; // multiply by 1 is no change to the modified value
            }
        }
    }

    public float GetMultiplier(string key) {
        return multipliers[key];
    }

    public void SetMultiplier(string key, float value) {
        multipliers[key] = value;
        UpdateSubscribers(key);
    }

}


public enum DifficultyLevel {
    easy,  // well done
    medium, // medium
    hard, // rare
    raw,   // it's bloody raw!
    custom
}