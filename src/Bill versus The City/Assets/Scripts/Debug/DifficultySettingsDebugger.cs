using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class DifficultySettingsDebugger : MonoBehaviour {

    public bool set_template = false;
    public bool write = false;
    public bool read = true;

    public DifficultyLevel difficulty_level = DifficultyLevel.custom;
    public float enemy_armor = 1f;
    public float enemy_health = 1f;
    public float player_armor = 1f;
    public float player_health = 1f;
    
    void Update() {
        DifficultySettings settings = GameSettings.inst.difficulty_settings;
        if (set_template) {
            set_template = false;
            if (difficulty_level != DifficultyLevel.custom) {
                settings.SetDifficultyLevel(difficulty_level);
            }
        } else if (write) {
            write = false;
            if (enemy_armor > 0) {
                settings.SetMultiplier(DifficultySettings.ENEMY_ARMOR, enemy_armor);
            } 
            if (enemy_health > 0) {
                settings.SetMultiplier(DifficultySettings.ENEMY_HEALTH, enemy_health);
            }
            if (player_armor > 0) {
                settings.SetMultiplier(DifficultySettings.PLAYER_ARMOR, player_armor);
            }
            if (player_health > 0) {
                settings.SetMultiplier(DifficultySettings.PLAYER_HEALTH, player_health);
            }
        } else if (read) {
            difficulty_level = settings.difficulty_level;
            enemy_armor = settings.GetMultiplier(DifficultySettings.ENEMY_ARMOR);
            enemy_health = settings.GetMultiplier(DifficultySettings.ENEMY_HEALTH);
            player_armor = settings.GetMultiplier(DifficultySettings.PLAYER_ARMOR);
            player_health = settings.GetMultiplier(DifficultySettings.PLAYER_HEALTH);
        }
    }
}