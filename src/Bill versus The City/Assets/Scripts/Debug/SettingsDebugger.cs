
// using System;
// using System.Collections.Generic;
// using System.Reflection;
// using UnityEngine;

// public class SettingsDebugger : MonoBehaviour {
//     GeneralSettingsDebugger general = new GeneralSettingsDebugger();
//     DifficultySettingsModuleDebugger difficulty = new DifficultySettingsModuleDebugger();
//     GraphicsSettingsModuleDebugger graphics = new GraphicsSettingsModuleDebugger();
//     GameplaySettingsModuleDebugger game_play = new GameplaySettingsModuleDebugger();
//     AudioSettingsModuleDebugger audio_settings = new AudioSettingsModuleDebugger();
//     DebugSettingsModuleDebugger debug = new DebugSettingsModuleDebugger();

//     private List<ISettingsModuleDebugger> debuggers;

//     void Start() {
//         debuggers = new List<ISettingsModuleDebugger> {general, difficulty, graphics, game_play, audio_settings, debug};
//     }

//     void Update() {
//         foreach (ISettingsModuleDebugger d in debuggers) {
//             d.UpdateDebug();
//         }
//     }
// }


// public interface ISettingsModuleDebugger {
//     public ISettingsModule settings_module { get; }
//     public void UpdateDebug();
// }



// [Serializable]
// public class GeneralSettingsDebugger : ISettingsModuleDebugger {

//     public ISettingsModule settings_module { get; }
//     public string todo = "todo";
//     public void UpdateDebug() {
//         // TODO ---
//     }
// }

// [Serializable]
// public class DifficultySettingsModuleDebugger : ISettingsModuleDebugger {

//     public ISettingsModule settings_module { get; }
//     public string todo = "todo";
//     public void UpdateDebug() {
//         // TODO ---
//     }
// }

// [Serializable]
// public class GraphicsSettingsModuleDebugger : ISettingsModuleDebugger {

//     public ISettingsModule settings_module { get; }
//     public string todo = "todo";
//     public void UpdateDebug() {
//         // TODO ---
//     }
// }

// [Serializable]
// public class GameplaySettingsModuleDebugger : ISettingsModuleDebugger {

//     public ISettingsModule settings_module { get; }
//     public string todo = "todo";
//     public void UpdateDebug() {
//         // TODO ---
//     }
// }

// [Serializable]
// public class AudioSettingsModuleDebugger : ISettingsModuleDebugger {

//     public ISettingsModule settings_module { get; }
//     public string todo = "todo";
//     public void UpdateDebug() {
//         // TODO ---
//     }
// }

// [Serializable]
// public class DebugSettingsModuleDebugger : ISettingsModuleDebugger {

//     public ISettingsModule settings_module { get => GameSettings.inst.debug_settings; }
//     public string show_grenade_fuse;
//     public bool show_fps, debug_mode, show_damage_numbers, player_invincibility, player_invisible,
//         allow_debug_actions, unlock_all_weapons, unrestrict_weapon_slots, no_reload, infinte_ammo_supply;
//     public void UpdateDebug() {
//         DebugSettings debug_settings = GameSettings.inst.debug_settings;
//         show_fps = debug_settings.GetBool("show_fps");
//         debug_mode = debug_settings.GetBool("debug_mode");
//         show_damage_numbers = debug_settings.GetBool("show_damage_numbers");
//         player_invincibility = debug_settings.GetBool("player_invincibility");
//         player_invisible = debug_settings.GetBool("player_invisible");
//         allow_debug_actions = debug_settings.GetBool("allow_debug_actions");
//         unlock_all_weapons = debug_settings.GetBool("unlock_all_weapons");
//         unrestrict_weapon_slots = debug_settings.GetBool("unrestrict_weapon_slots");
//         no_reload = debug_settings.GetBool("no_reload");
//         infinte_ammo_supply = debug_settings.GetBool("infinte_ammo_supply");

//     }
// }
//     // general
//     // difficulty
//     // graphics
//     // game_play
//     // audio
//     // debug


// public interface ISett

// public abstract class AbstractSettingsModuleDebugger : ISettingsModuleDebugger {
    

//     public abstract AbstractSettingsModule settings_module { get; }

//     public virtual void UpdateDebug() {
//         UpdateBoolFields();
//         UpdateFloatFields();
//         UpdateIntFields();
//         UpdateOtherFields();
//     }

//     protected void UpdateBoolFields() {
        
//     }
//     protected void UpdateFloatFields() {
        
//         foreach (string field_name in settings_module.float_field_names) {
//             FieldInfo f = this.GetType().GetField(field_name);
//             float value = settings_module.GetFloat(field_name);
//             f.SetValue(this, value);
//         }
//     }
//     protected void UpdateIntFields() {
//         foreach (string field_name in settings_module.int_field_names) {
//             FieldInfo f = this.GetType().GetField(field_name);
//             int value = settings_module.GetInt(field_name);
//             f.SetValue(this, value);
//         }
//     }
//     protected virtual void UpdateOtherFields() {
//         // Do nothing by defaults, not all settings modules have "other" fields
//     }
// }