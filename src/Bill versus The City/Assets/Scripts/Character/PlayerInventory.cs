using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerInventory : IPlayerObserver { //: IGenericObservable {
    /**
      * 
      */
    public int dollars = 0;

    private IWeapon _handgun; // weapon slot for a handgun
    public IWeapon handgun {
        get { return _handgun; }
        set {
            _handgun = value;
            combat.attacks.AssignWeaponSlot(1, _handgun);
        }
    }

    private IWeapon _rifle; // weapon slot for a larger gun
    public IWeapon rifle {
        get { return _rifle; }
        set {
            _rifle = value;
            combat.attacks.AssignWeaponSlot(0, _rifle);
        }
    }
    private IWeapon _pickup; // weapon slot for picking up dropped, potentially illegal, weapons
    public IWeapon pickup {
        get { return _pickup; }
        set {
            _pickup = value;
            combat.attacks.AssignWeaponSlot(2, _pickup);
        }
    }
    private PlayerCombat combat;

    private List<IWeapon> _availible_rifles, _availible_handguns;
    public List<IWeapon> availible_rifles {
        get {
            if (_availible_rifles == null) {
                // initialize with starting weapons
                _availible_rifles = new List<IWeapon>();
                foreach (IWeapon weapon in GetStartingEquipment().rifles) {
                    _availible_rifles.Add(weapon.CopyWeapon());
                }
            }
            return _availible_rifles;
        }
    }
    public List<IWeapon> availible_handguns {
        get {
            if (_availible_handguns == null) {
                // initialize with starting weapons
                _availible_handguns = new List<IWeapon>();
                foreach (IWeapon weapon in GetStartingEquipment().handguns) {
                    _availible_handguns.Add(weapon.CopyWeapon());
                }
            }
            return _availible_handguns;
        }
    }

    public static EquipmentSet GetStartingEquipment() {
        return Resources.Load<EquipmentSet>("StartingEquipment");
    }

    public PlayerInventory() {
        combat = null; // subscribing to the PlayerCharacter here creates infinite recursion
        _handgun = PlayerWeaponsManager.inst.GetWeapon(PlayerWeaponsManager.HANDGUN);
        _rifle = PlayerWeaponsManager.inst.GetWeapon(PlayerWeaponsManager.SHOTGUN);
        _pickup = null;
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
}

public enum WeaponClass {
    handgun, 
    longgun, 
    pickup
}

