using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : AttackController
{

    public PlayerWeaponSelection weapon_manager;
    public ScriptableObject[] init_slots;
    public IWeapon[] weapon_slots = new IWeapon[10];
    public bool[] weapon_slots_enabled = new bool[]{true, true, false, false, false, false, false, false, false, false};

    public int current_slot = 0;

    // public void UpdateWeapon(int slot, IWeapon weapon) {
    //     current_weapon = weapon;
    // }
    public override void UpdateSubscribers() {
        foreach (IWeaponManagerSubscriber sub in subscribers) {
            sub.UpdateWeapon(null, current_weapon);
        }
    }

    protected override void AttackControllerStart() {
        for (int i = 0; i < 10; i++) {
            if (init_slots[i] != null) {
                weapon_slots[i] = (IWeapon) init_slots[i];
            }
        }
        SetWeaponBySlot(current_slot);
    }

    // Update is called once per frame
    protected override void AttackControllerUpdate()
    {
        int? weapon_slot_input = InputSystem.current.WeaponSlotInput();
        Debug.Log($"key 1 down: {Input.GetKeyDown(KeyCode.Keypad1)}");
        Debug.Log("weapon slot input: " + weapon_slot_input);
        if (weapon_slot_input != null) {
            SetWeaponBySlot((int) weapon_slot_input);
        }
    }
    
    private bool SetWeaponBySlot(int slot) {
        if (weapon_slots_enabled[slot] && weapon_slots[slot] != null) {
            current_slot = slot;
            current_weapon = weapon_slots[slot];
            UpdateSubscribers();
            Debug.Log($"set weapon slot to '{slot}': {current_weapon}");
            return true;
        }
        Debug.LogWarning($"tried to set weapon to invalid slot '{slot}'.\nEnabled: {weapon_slots_enabled[slot]}, {weapon_slots[slot]}");
        return false;
    }
}
