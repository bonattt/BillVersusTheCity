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
            combat.attacks.AssignWeaponSlot(0, _handgun);
        }
    }

    private IWeapon _rifle; // weapon slot for a larger gun
    public IWeapon rifle {
        get { return _rifle; }
        set {
            _rifle = value;
            combat.attacks.AssignWeaponSlot(1, _rifle);
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

    public PlayerInventory() {
        combat = null; // subscribing to the PlayerCharacter here creates infinite recursion
        _handgun = PlayerWeaponsManager.inst.GetWeapon(PlayerWeaponsManager.HANDGUN);
        _rifle = PlayerWeaponsManager.inst.GetWeapon(PlayerWeaponsManager.RIFLE);
        _pickup = PlayerWeaponsManager.inst.GetWeapon(PlayerWeaponsManager.SHOTGUN);
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

