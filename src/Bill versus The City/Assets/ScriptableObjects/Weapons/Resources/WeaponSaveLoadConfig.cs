using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Weapon Save/Load Config", menuName ="Data/WeaponSaveLoadConfig")]
public class WeaponSaveLoadConfig : ScriptableObject {
    // there should only be one WeaponSaveLoadConfig, and it should be stored where it can be 
    // retrieved with `Resource.Load<WeaponSaveLoadConfig>(WeaponSaveLoadConfig.RESOURCE_PATH)`
    public const string RESOURCE_PATH = "weapon_save_load_config";

    private static WeaponSaveLoadConfig _inst = null;
    public static WeaponSaveLoadConfig inst {
        get {
            if (_inst == null) {
                _inst = Resources.Load<WeaponSaveLoadConfig>(RESOURCE_PATH);
            }
            return _inst;
        }
    }

    [Tooltip("list of weapon which can be loaded using the ID in the same index stored in `weapon_load_ids`.")]
    public List<DetailedWeapon> weapons;

    private Dictionary<string, DetailedWeapon> _weapon_dict = null;
    private Dictionary<string, int> _weapon_index_dict = null;
    // protected Dictionary<string, DetailedWeapon> weapon_dict {
    //     get {
    //         if (_weapon_dict == null) {
    //             InitializeDictionaries();
    //         }
    //         return _weapon_dict;
    //     }
    // }
    protected Dictionary<string, int> weapon_index_dict {
        get {
            if (_weapon_index_dict == null) {
                InitializeDictionaries();
            }
            return _weapon_index_dict;
        }
    }

    [Tooltip("If true, the corresponding weapon index is considered a 'starting weapon' and is added to the players inventory on a new game.")]
    public List<bool> starting_weapons;

    private void InitializeDictionaries() {
        _weapon_index_dict = new Dictionary<string, int>();
        _weapon_dict = new Dictionary<string, DetailedWeapon>();
        for (int i = 0; i < weapons.Count; i++) {
            _weapon_dict[weapons[i].item_id] = weapons[i];
            _weapon_index_dict[weapons[i].item_id] = i;
        }
    }

    public bool IsStartingWeapon(string weapon_id) {
        int index = GetWeaponIndexByID(weapon_id);
        try {
            return starting_weapons[index];
        } catch (IndexOutOfRangeException e) {
            Debug.LogException(e);
            return false;
        }
    }

    protected bool IsStartingWeapon(IWeapon weapon) => IsStartingWeapon(weapon.item_id);

    public void ValidateWeaponConfig(string weapon_id, IWeapon weapon) {
        // does some validation of weapon and ID, and logs warnings/errors for any problems
        if (weapon_id == null) { Debug.LogWarning($"weapon_id is null! weapon: {weapon}"); return; }
        if (weapon == null) { Debug.LogWarning($"weapon is null! weapon_id: {weapon_id}"); return; }
 
        if (! weapon_id.Equals(weapon.item_id)) {
            Debug.LogError($"weapon_id mismatched! weapon_id: {weapon_id}, weapon.item_id: {weapon.item_id}");
        }
    }

    public int GetWeaponIndexByID(string weapon_id) {
        return weapon_index_dict[weapon_id];
    }

    public IWeapon GetWeaponByID(string weapon_id) {
        IWeapon weapon = weapons[GetWeaponIndexByID(weapon_id)];
        ValidateWeaponConfig(weapon_id, weapon);
        return weapon;
    }

    public List<string> GetStartingWeaponIds() {
        List<string> weapon_ids = new List<string>();
        foreach (DetailedWeapon w in weapons) {
            if (IsStartingWeapon(w.item_id)) {
                weapon_ids.Add(w.item_id);
            }
        }
        return weapon_ids;
    }
}