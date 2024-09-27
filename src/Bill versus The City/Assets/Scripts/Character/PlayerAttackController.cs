using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : AttackController
{

    public PlayerWeaponSelection weapon_manager;
    public ScriptableObject[] init_slots;
    public IWeapon[] weapon_slots = new IWeapon[10];
    public bool[] weapon_slots_enabled = new bool[]{true, true, false, false, false, false, false, false, false, false};

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
        for (int i = 0; i < 10; i++) {
            if (init_slots[i] != null) {
                weapon_slots[i] = (IWeapon) Instantiate(init_slots[i]);
                weapon_slots[i].current_ammo = weapon_slots[i].ammo_capacity;
            }
        }
        SetWeaponBySlot(_current_slot);

        UpdateSubscribers();
    }

    // Update is called once per frame
    protected override void AttackControllerUpdate()
    {
        TrySwitchWeapons();
        UpdateSubscribers();
    }
    
    public override void UpdateSubscribers() {
        foreach (IWeaponManagerSubscriber sub in subscribers) {
            sub.UpdateWeapon(current_slot, current_weapon);
        }
    }
    

    private void TrySwitchWeapons() {
        // Polls for player inputs, and switches weapons if necessary
        if (! switch_weapons_blocked) {
            int? weapon_slot_input = InputSystem.current.WeaponSlotInput();
            if (weapon_slot_input != null) {
                SetWeaponBySlot((int) weapon_slot_input);
            }
        }
    }
    
    private bool SetWeaponBySlot(int slot) {
        if (weapon_slots_enabled[slot] && weapon_slots[slot] != null) {
            _current_slot = slot;
            current_weapon = weapon_slots[slot];
            UpdateSubscribers();
            return true;
        }
        Debug.LogWarning($"tried to set weapon to invalid slot '{slot}'.\nEnabled: {weapon_slots_enabled[slot]}, {weapon_slots[slot]}");
        return false;
    }
}
