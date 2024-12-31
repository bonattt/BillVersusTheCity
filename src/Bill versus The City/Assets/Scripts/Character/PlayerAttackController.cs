using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : AttackController
{
    public IWeapon[] weapon_slots = new IWeapon[10];
    public bool[] weapon_slots_enabled = new bool[]{true, true, true, false, false, false, false, false, false, false};

    public bool switch_weapons_blocked = false;

    public int _current_slot = 0;
    public override int? current_slot {
        get { return _current_slot; }
    }

    // public void UpdateWeapon(int slot, IWeapon weapon) {
    //     current_weapon = weapon;
    // }

    private void UpdateAimSensitivity() {
        InputSystem.current.mouse_sensitivity_percent = 1f - (aim_percent * 0.5f);
    }

    protected override void AttackControllerStart() {
        ClearWeapons();
        SwitchWeaponBySlot(_current_slot);
        UpdateSubscribers();
    }
    
    public void SetWeaponsFromInventory(PlayerInventory inventory) {
        ClearWeapons();
        AssignWeaponSlot(0, inventory.handgun);
        AssignWeaponSlot(1, inventory.rifle);
        AssignWeaponSlot(2, inventory.pickup);
    }

    // Update is called once per frame
    protected override void AttackControllerUpdate()
    {
        TrySwitchWeapons();
        // UpdateSubscribers();
        UpdateAimSensitivity();
    }
    
    public override void UpdateSubscribers() {
        // update UI's to display changes to equipped weapon or ammo amound
        foreach (IWeaponManagerSubscriber sub in subscribers) {
            sub.UpdateWeapon(current_slot, current_weapon);
        }
    }
    

    private void TrySwitchWeapons() {
        // Polls for player inputs, and switches weapons if necessary
        if (! switch_weapons_blocked) {
            int? weapon_slot_input = InputSystem.current.WeaponSlotInput();
            if (weapon_slot_input != null) {
                SwitchWeaponBySlot((int) weapon_slot_input);
            }
        }
    }
    
    public bool SwitchWeaponBySlot(int slot, bool force=false) {
        if (force || (weapon_slots_enabled[slot] && weapon_slots[slot] != null)) {
            _current_slot = slot;
            current_weapon = weapon_slots[slot];
            UpdateSubscribers();
            return true;
        }
        Debug.LogWarning($"tried to set weapon to invalid slot '{slot}'.\nEnabled: {weapon_slots_enabled[slot]}, {weapon_slots[slot]}");
        return false;
    }

    public void ClearWeapons() {
        // remove all equipped weapons
        for (int i = 0; i < weapon_slots.Length; i++) {
            weapon_slots[i] = null;
        }
        UpdateSubscribers();
    }

    public void AssignWeaponSlot(int slot, IWeapon new_weapon) {
        IWeapon previous_weapon = weapon_slots[slot];
        weapon_slots[slot] = new_weapon;
        if (slot == current_slot) {
            current_weapon = new_weapon;
        }
        // only update subscribers if the ne~w weapon is actually new
        if (previous_weapon != new_weapon) {
            UpdateSubscribers();
        }
    }
}
