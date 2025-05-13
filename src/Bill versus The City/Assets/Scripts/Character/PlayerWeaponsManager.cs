


// using System;
// using System.Collections.Generic;

// using UnityEngine;

// public class PlayerWeaponsManager {
//     // class for loading weapons, and managing which weapons are unlocked by the player

//     private const string DEFAULT_SETTINGS = "default_weapon_settings";
//     private static PlayerWeaponsManager _inst = null;
//     public static PlayerWeaponsManager inst {
//         get {
//             if(_inst == null) {
//                 _inst = new PlayerWeaponsManager();
//             }
//             return _inst;
//         }
//     }

//     public PlayerWeaponsManager() {
//         LoadDefaultSettings();
//     }

//     public const string HANDGUN = "handgun";
//     public const string RIFLE = "rifle";
//     public const string SHOTGUN = "shotgun";
//     public const string SMG = "smg";

//     private Dictionary<string, IWeapon> weapons = new Dictionary<string, IWeapon>();
//     private Dictionary<string, bool> weapon_enabled = new Dictionary<string, bool>();

//     public void LoadFromSettings(WeaponManagerSetting settings) {
//         weapons[HANDGUN] = settings.handgun;
//         weapons[RIFLE] = settings.rifle;
//         weapons[SHOTGUN] = settings.shotgun;
//         weapons[SMG] = settings.smg;
        
//         weapon_enabled[HANDGUN] = settings.handgun_unlocked;
//         weapon_enabled[RIFLE] = settings.rifle_unlocked;
//         weapon_enabled[SHOTGUN] = settings.shotgun_unlocked;
//         weapon_enabled[SMG] = settings.smg_unlocked;
//     }

//     public void LoadDefaultSettings() {
//         WeaponManagerSetting default_settings = Resources.Load<WeaponManagerSetting>(DEFAULT_SETTINGS);
//         LoadFromSettings(default_settings);
//     }

//     public void LoadFromSaveFile(SaveFile save) {
//         throw new NotImplementedException("TODO");
//     }

//     public IWeapon GetWeapon(string weapon_name) {
//         return weapons[weapon_name];
//     }

//     public bool IsWeaponEnabled(string weapon_name) {
//         if (weapons.ContainsKey(weapon_name)) {
//             return weapon_enabled[weapon_name];
//         }
//         return false;
//     }

//     public void SetWeapon(string weapon_name, IWeapon new_weapon) {
//         SetWeapon(weapon_name, new_weapon, true);
//     }
//     public void SetWeapon(string weapon_name, IWeapon new_weapon, bool enabled) {
//         weapons[weapon_name] = new_weapon;
//         weapon_enabled[weapon_name] = enabled; 
//     }

// }