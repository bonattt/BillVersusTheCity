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
            Debug.LogWarning($"combat is null: {combat == null}, attacks is null {combat == null || combat.attacks == null}"); // TODO --- remove debug
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
        // gets a config for the players initial availible equipment
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
        if (starting_weapons_queued) {
            rifle = starting_rifle;
            handgun = starting_handgun;
            pickup = starting_pickup;
            starting_weapons_queued = false;
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
    private IWeapon starting_rifle;
    private IWeapon starting_handgun;
    private IWeapon starting_pickup;

    public void EquipStartingWeapons(IWeapon starting_rifle, IWeapon starting_handgun, IWeapon starting_pickup) {
        // if there is a current player, equips starting weapons to that player. otherwise, sets those weapons once the player is initialized
        this.starting_weapons_queued = true;
        this.starting_rifle = starting_rifle;
        this.starting_handgun = starting_handgun;
        this.starting_pickup = starting_pickup;
    }
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
