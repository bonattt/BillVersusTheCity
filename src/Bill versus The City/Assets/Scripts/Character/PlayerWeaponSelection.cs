using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSelection : MonoBehaviour
{
    public CharCtrl character_controller;
    public AttackController attack_controller;


    // public IWeapon current_weapon { get; private set; }
    // public ScriptableObject[] init_slots;
    // public IWeapon[] weapon_slots = new IWeapon[10];
    // public bool[] weapon_slots_enabled = new bool[]{true, true, false, false, false, false, false, false, false, false};

    // private int current_slot = 0;
    
    // private List<IWeaponManagerSubscriber> subscribers = new List<IWeaponManagerSubscriber>();
    // public void Subscribe(IWeaponManagerSubscriber sub) => subscribers.Add(sub);
    // public void Unsubscribe(IWeaponManagerSubscriber sub) => subscribers.Remove(sub);
    // public void UpdateSubscribers() {
    //     Debug.Log($"UpdateSubscribers for {subscribers.Count}");
    //     foreach (IWeaponManagerSubscriber sub in subscribers) {
    //         sub.UpdateWeapon(current_slot, current_weapon);
    //     }
    // }

    // void Start() {
    //     for (int i = 0; i < 10; i++) {
    //         if (init_slots[i] != null) {
    //             weapon_slots[i] = (IWeapon) init_slots[i];
    //         }
    //     }
    //     SetWeaponBySlot(current_slot);
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     int? weapon_slot_input = InputSystem.current.WeaponSlotInput();
    //     Debug.Log("weapon slot input: " + weapon_slot_input);
    //     if (weapon_slot_input != null) {
    //         SetWeaponBySlot((int) weapon_slot_input);
    //     }
    // }

    // private bool SetWeaponBySlot(int slot) {
    //     if (weapon_slots_enabled[slot] && weapon_slots[slot] != null) {
    //         current_slot = slot;
    //         current_weapon = weapon_slots[slot];
    //         attack_controller.current_weapon = current_weapon;
    //         UpdateSubscribers();
    //         Debug.Log($"set weapon slot to '{slot}'");
    //         return true;
    //     }
    //     Debug.LogWarning($"tried to set weapon to invalid slot '{slot}'.\nEnabled: {weapon_slots_enabled[slot]}, {weapon_slots[slot]}");
    //     return false;
    // }
}
