using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : IPlayerObserver, ISaveProgress { //: IGenericObservable {
    /**
      * 
      */

    public const int STARTING_DOLLARS = 350;
    public int dollars = -1; // total dollars the player has 
    public int dollars_change_in_level = 0; // total dollars earned on the current level; if the level restarts, it is reset to 0, if the level is beaten, it's added to `dollars` and saved\
    public int total_dollars {
        get {
            return dollars + dollars_change_in_level;
        }
    }

    public int? slot_selected {
        get {
            if (combat == null) {
                return null;
            }
            return combat.attacks._current_slot;
        }
    }

    public IFirearm last_handgun_equipped { get; private set; }
    private IFirearm _handgun; // weapon slot for a handgun
    public IFirearm handgun {
        get { return _handgun; }
        set {
            _handgun = value;
            combat.attacks.AssignWeaponSlot(1, _handgun);
            if (combat.attacks.current_slot == 1) {
                combat.attacks.SwitchWeaponBySlot(1);
                combat.attacks.UpdateSubscribers();
            }
        }
    }

    public IFirearm last_rifle_equipped { get; private set; }
    private IFirearm _rifle; // weapon slot for a larger gun
    public IFirearm rifle {
        get { return _rifle; }
        set {
            _rifle = value;
            combat.attacks.AssignWeaponSlot(0, _rifle);
            if (combat.attacks.current_slot == 0) {
                combat.attacks.SwitchWeaponBySlot(0);
            }
        }
    }

    private IFirearm _pickup; // weapon slot for picking up dropped, potentially illegal, weapons
    public IFirearm pickup {
        get { return _pickup; }
        set {
            _pickup = value;
            combat.attacks.AssignWeaponSlot(2, _pickup);
            if (combat.attacks.current_slot == 2) {
                combat.attacks.SwitchWeaponBySlot(2);
            }
        }
    }

    public void UpdateWeapon() {
        // called to manually trigger UI updateds from weapon changes
        combat.attacks.UpdateSubscribers();
    }
    private PlayerCombat combat;

    // private List<IWeapon> _availible_rifles, _availible_handguns;
    public List<IFirearm> AvailibleRifles() {
        bool allow_all_slots = GameSettings.inst.debug_settings.unrestrict_weapon_slots;
        List<IFirearm> _availible_rifles = new List<IFirearm>();
        foreach (IFirearm weapon in availible_weapons) {
            if (allow_all_slots || weapon.weapon_slot == WeaponSlot.longgun) {
                _availible_rifles.Add(weapon);
            }
        }
        return _availible_rifles;
    }
    public List<IFirearm> AvailibleHandguns() {
        bool allow_all_slots = GameSettings.inst.debug_settings.unrestrict_weapon_slots;
        List<IFirearm> _availible_handguns = new List<IFirearm>();
        foreach (IFirearm weapon in availible_weapons) {
            if (allow_all_slots || weapon.weapon_slot == WeaponSlot.handgun) {
                _availible_handguns.Add(weapon);
            }
        }
        return _availible_handguns;
    }

    public PlayerInventory() {
        combat = null; // subscribing to the PlayerCharacter here creates infinite recursion
        _handgun = null; // PlayerWeaponsManager.inst.GetWeapon(PlayerWeaponsManager.HANDGUN);
        _rifle = null; // PlayerWeaponsManager.inst.GetWeapon(PlayerWeaponsManager.SHOTGUN);
        _pickup = null;
    }

    public void StartNewGame() {
        dollars = STARTING_DOLLARS;
        last_rifle_equipped = null;
        last_handgun_equipped = null;
        owned_weapons = new List<IFirearm>();
        foreach (string weapon_id in WeaponSaveLoadConfig.inst.GetStartingWeaponIds()) {
            owned_weapons.Add(WeaponSaveLoadConfig.inst.GetWeaponByID(weapon_id));
        }
    }

    public void ResetLevel() {
        // resets the inventory at the start of a level.
        weapons_purchased_in_level = new List<IFirearm>();
        dollars_change_in_level = 0;
    }

    public void ApplyChangesFromLevel() {
        // permanently stores changes made during the level
        dollars += dollars_change_in_level;
        foreach (IFirearm weapon_purchased in weapons_purchased_in_level) {
            owned_weapons.Add(weapon_purchased);
        }
        if (_rifle != null) {
            last_rifle_equipped = _rifle;
        }
        if (_handgun != null) {
            last_handgun_equipped = _handgun;
        }
        ResetLevel();
    }

    public void LoadProgress(DuckDict progress_data) {
        if (progress_data == null) {
            Debug.LogWarning("Inventory data is null!");
            dollars = STARTING_DOLLARS;
            return;
        }
        dollars = (int)progress_data.GetInt("dollars");
        LoadWeaponsFromProgress(progress_data);
    }

    // public void SaveProgress(DuckDict progress_data) {
    public DuckDict GetProgressData() {
        DuckDict inventory_progress_data = new DuckDict();
        inventory_progress_data.SetInt("dollars", dollars);

        List<string> weapon_ids = new List<string>();
        foreach (IFirearm weapon in owned_weapons) {
            weapon_ids.Add(weapon.item_id);
        }
        inventory_progress_data.SetStringList("weapon_unlocks", weapon_ids);
        inventory_progress_data.SetString(LAST_LONGGUN_EQUIPPED, last_rifle_equipped != null ? last_rifle_equipped.item_id : "");
        inventory_progress_data.SetString(LAST_HANDGUN_EQUIPPED, last_handgun_equipped != null ? last_handgun_equipped.item_id : "");
        return inventory_progress_data;
    }

    public const string LAST_LONGGUN_EQUIPPED = "last_longgun_equipped";
    public const string LAST_HANDGUN_EQUIPPED = "last_handgun_equipped";

    private void LoadWeaponsFromProgress(DuckDict progress_data) {
        HashSet<string> weapon_unlocks;
        List<string> str_ls = progress_data.GetStringList("weapon_unlocks");
        if (str_ls != null) {
            weapon_unlocks = new HashSet<string>(str_ls);
        } else {
            Debug.LogWarning($"no weapon unlocks found in save file!!");
            weapon_unlocks = new HashSet<string>();
        }

        // if the starting weapons have changed, add any missing starting weapons to owned weapons.
        foreach (string starting_id in WeaponSaveLoadConfig.inst.GetStartingWeaponIds()) {
            if (!weapon_unlocks.Contains(starting_id)) {
                weapon_unlocks.Add(starting_id);
            }
        }
        owned_weapons = new List<IFirearm>();
        foreach (string weapon_id in weapon_unlocks) {
            IFirearm weapon = WeaponSaveLoadConfig.inst.GetWeaponByID(weapon_id);
            if (weapon == null) {
                Debug.LogError($"weapon_id '{weapon_id}' not defined!");
                continue;
            }
            owned_weapons.Add(weapon);
        }
        string _last_rifle = progress_data.GetString(LAST_LONGGUN_EQUIPPED);
        string _last_handgun = progress_data.GetString(LAST_HANDGUN_EQUIPPED);
        if (_last_rifle == null || _last_rifle.Equals("")) {
            last_rifle_equipped = null;
        } else {
            last_rifle_equipped = WeaponSaveLoadConfig.inst.GetWeaponByID(_last_rifle);
        }
        if (_last_handgun == null || _last_handgun.Equals("")) {
            last_handgun_equipped = null;
        } else {
            last_handgun_equipped = WeaponSaveLoadConfig.inst.GetWeaponByID(_last_handgun);
        }
    }

    public void SetWeapons(PlayerAttackController attack_ctrl) {
        if (combat == null) {
            Debug.LogWarning("`PlayerInventory.SetWeapons(null)` called!");
            return;
        }
        attack_ctrl.SetWeaponsFromInventory(this);
        // attack_ctrl.ClearWeapons();
        // attack_ctrl.AssignWeaponSlot(0, handgun);
        // attack_ctrl.AssignWeaponSlot(1, rifle);
        // attack_ctrl.AssignWeaponSlot(2, pickup);
    }

    public void NewPlayerObject(PlayerCombat player) {
        combat = player;
        if (starting_weapons_queued) {
            rifle = starting_rifle;
            handgun = starting_handgun;
            pickup = starting_pickup;
            starting_weapons_queued = false;
        } else {
        }
        player.attacks.SetWeaponsFromInventory(this);
    }
    // private List<IGenericObserver> subscribers = new List<IGenericObserver>();
    // public void Subscribe(IGenericObserver sub) => subscribers.Add(sub);
    // public void Unusubscribe(IGenericObserver sub) => subscribers.Remove(sub);
    // public void UpdateSubscribers() {
    //     foreach(IGenericObserver sub in subscribers) {
    //         sub.UpdateObserver(this);
    //     }
    // }
    private bool starting_weapons_queued;
    private IFirearm starting_rifle;
    private IFirearm starting_handgun;
    private IFirearm starting_pickup;

    public List<IFirearm> weapons_purchased_in_level = new List<IFirearm>();
    public void AddWeapon(IFirearm new_weapon) => weapons_purchased_in_level.Add(new_weapon);
    protected List<IFirearm> owned_weapons = new List<IFirearm>();
    public IEnumerable<IFirearm> availible_weapons {
        get {
            foreach (IFirearm w in owned_weapons) {
                yield return w;
            }
            foreach (IFirearm w in weapons_purchased_in_level) {
                yield return w;
            }
        }
    }

    public void EquipStartingWeapons(IFirearm starting_rifle, IFirearm starting_handgun, IFirearm starting_pickup) {
        // if there is a current player, equips starting weapons to that player. otherwise, sets those weapons once the player is initialized
        if (combat == null) {
            this.starting_weapons_queued = true;
            this.starting_rifle = starting_rifle;
            this.starting_handgun = starting_handgun;
            this.starting_pickup = starting_pickup;
        } else {
            rifle = starting_rifle;
            handgun = starting_handgun;
            pickup = starting_pickup;
            starting_weapons_queued = false;
        }
    }

    public bool CanPurchase(IPurchase purchase) {
        return total_dollars >= purchase.purchase_cost;
    }

    public void Purchase(IPurchase purchase) {
        if (!CanPurchase(purchase)) {
            throw new InvalidPurchaseException(this, purchase);
        }
        dollars_change_in_level -= purchase.purchase_cost;
        purchase.ApplyPurchase(this);
    }

    public bool AlreadyOwnsItem(IItem item) {
        switch (item) {
            case IFirearm firearm:
                return AlreadyOwnsFirearm(firearm);
            case IWeapon weapon:
                return AlreadyOwnsWeapon(weapon);
            case IArmor armor:
                return AlreadyOwnsArmor(armor);

            default:
                Debug.LogWarning($"{item} is an unhandled type of item!");
                return false;
        }
    }

    public bool AlreadyOwnsFirearm(IFirearm firearm) {
        return AlreadyOwnsWeapon((IWeapon)firearm);
    }

    public bool AlreadyOwnsWeapon(IWeapon weapon) {
        foreach (IFirearm firearm in availible_weapons) {
            if (firearm.item_id.Equals(weapon.item_id)) {
                return true;
            }
        }
        return false;
    }
    public bool AlreadyOwnsArmor(IArmor armor) {
        Debug.LogWarning("TODO --- implement AlreadyOwnsItem(Armor)!"); // TODO --- remove debug
        return false; /* TODO --- implement */
    }
}

[System.Serializable]
public class PlayerInventoryException : System.Exception {
    public PlayerInventoryException(string message) : base(message) { }
}

[System.Serializable]
public class InvalidPurchaseException : PlayerInventoryException {
    public InvalidPurchaseException(PlayerInventory inv, IPurchase purchase) : 
        base($"Invalid purchase ${purchase.purchase_cost} more than total money {inv.total_dollars}!") { }
}

public enum WeaponSlot {
    handgun, 
    longgun, 
    pickup
}

public enum WeaponClass {
    handgun, 
    rifle,
    shotgun,
    empty
}
