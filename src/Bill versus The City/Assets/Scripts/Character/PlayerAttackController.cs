using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : AttackController
{
    public IFirearm[] weapon_slots = new IFirearm[10];
    public bool[] weapon_slots_enabled = new bool[]{true, true, true, false, false, false, false, false, false, false};

    public int _current_slot = 0;
    
    public override int? current_slot
    {
        get { return _current_slot; }
    }

    private int? _min_slot = null;
    public int min_slot
    {
        get
        {
            if (_min_slot != null) { return _min_slot.Value; }

            Debug.Log("re-evaluate min_slot");
            int _min = weapon_slots.Length - 1;
            for (int i = _min; i >= 0; i--)
            {
                if (weapon_slots_enabled[i])
                {
                    _min = i;
                }
            }
            _min_slot = _min;
            return _min;
        }
    }
    
    private int? _max_slot = null;
    public int max_slot
    {
        get
        {
            if (_max_slot != null) { return _max_slot.Value; }

            Debug.Log("re-evaluate max_slot");
            int _max = 0;
            for (int i = 0; i < weapon_slots.Length; i++)
            {
                if (weapon_slots_enabled[i])
                {
                    _max = i;
                }
            }
            _max_slot = _max;
            return _max;
        }
    }

    public bool SlotHasWeapon(int i) {
        return weapon_slots[i] != null && weapon_slots_enabled[i];
    }

    private void UpdateAimSensitivity() {
        InputSystem.current.mouse_sensitivity_percent = 1f - (aim_percent * 0.5f);
    }

    protected override void Start() {
        base.Start();
        SwitchWeaponBySlot(_current_slot);
    }
    
    public void SetWeaponsFromInventory(PlayerInventory inventory) {
        ClearWeapons();
        AssignWeaponSlot(0, inventory.rifle);
        AssignWeaponSlot(1, inventory.handgun);
        AssignWeaponSlot(2, inventory.pickup);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        TrySwitchWeapons();
        UpdateAimSensitivity();
    }
    
    public override void UpdateSubscribers() {
        // update UI's to display changes to equipped weapon or ammo amound
        _max_slot = null; // TODO --- this should be called elsewhere
        _min_slot = null; // TODO --- this should be called elsewhere
        foreach (IWeaponManagerSubscriber sub in subscribers)
        {
            sub.UpdateWeapon(current_slot, current_gun);
        }
    }

    private void TrySwitchWeapons() {
        // Polls for player inputs, and switches weapons if necessary
        if (!switch_weapons_blocked)
        {
            int? weapon_slot_input = InputSystem.current.SetWeaponSlotInput();
            if (weapon_slot_input != null) {
                SwitchWeaponBySlot((int)weapon_slot_input);
            }  else if (InputSystem.current.NextWeaponInput()) {
                SwitchToNextWeapon();
            } else if (InputSystem.current.PreviousWeaponInput()) {
                SwitchToPreviousWeapon();
            }
        }
    }

    public bool SwitchToNextWeapon() {
        int _next_slot = _current_slot;
        while (_next_slot < max_slot) {
            _next_slot += 1;
            if (SlotHasWeapon(_next_slot)) { return SwitchWeaponBySlot(_next_slot); }
        }
        return false;
    }
    
    public bool SwitchToPreviousWeapon() {
        int _next_slot = _current_slot;
        while (_next_slot > min_slot) {
            _next_slot -= 1;
            if (SlotHasWeapon(_next_slot)) { return SwitchWeaponBySlot(_next_slot); }
        }
        return SwitchWeaponBySlot(_next_slot); 
    }

    public bool SwitchWeaponBySlot(int slot, bool force = false) {
        // switches the current weapon slot to the given value, equipping whatever weapon is loaded in that slot
        if (force || (weapon_slots_enabled[slot] && weapon_slots[slot] != null))
        {
            if (_current_slot != slot)
            {
                AimOnSwitch();
                Debug.Log($"SwitchWeaponBySlot{_current_slot} --> {slot}");
            }
            _current_slot = slot;
            current_gun = weapon_slots[slot];
            UpdateSubscribers();
            return true;
        }
        Debug.Log($"tried to set weapon to invalid slot '{slot}'.\nEnabled: {weapon_slots_enabled[slot]}, {weapon_slots[slot]}");
        return false;
    }

    public void ClearWeapons() {
        // remove all equipped weapons
        for (int i = 0; i < weapon_slots.Length; i++) {
            weapon_slots[i] = null;
        }
        UpdateSubscribers();
    }

    public void AssignWeaponSlot(int slot, IFirearm new_weapon) {
        IFirearm previous_weapon = weapon_slots[slot];
        weapon_slots[slot] = new_weapon;
        if (slot == current_slot) {
            current_gun = new_weapon;
        }
        // only update subscribers if the ne~w weapon is actually new
        if (previous_weapon != new_weapon) {
            UpdateSubscribers();
        }
    }

    /////////////////// DEBUG CODE /////////////////////////////
    
    public string debug_rifle_equipped, debug_handgun_equipped, debug_pickup_equipped;
    public int debug_min_slot, debug_max_slot;
    protected override void UpdateDebugFields()
    {
        base.UpdateDebugFields();
        debug_rifle_equipped = $"{weapon_slots[0]}";
        debug_handgun_equipped = $"{weapon_slots[1]}";
        debug_pickup_equipped = $"{weapon_slots[2]}";
        debug_min_slot = min_slot;
        debug_max_slot = max_slot;
    }
}
