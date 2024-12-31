using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupPlayerInventory : MonoBehaviour, IPlayerObserver
{
    public int slot_selected = 0;
    public PlayerAttackController attack_controller;
    public DetailedWeapon rifle_equipped_init, handgun_equipped_init, pickup_equipped_init;
    public IWeapon rifle_equipped, handgun_equipped, pickup_equipped;
    public bool destroy_all = false;

    void Start() {
        PlayerCharacter.inst.GetPlayerCombat(this);
    }
    
    public void NewPlayerObject(PlayerCombat player) {
        if (rifle_equipped == null && rifle_equipped_init != null) {
            rifle_equipped = rifle_equipped_init.CopyWeapon();
        }
        if (handgun_equipped == null && handgun_equipped_init != null) {
            handgun_equipped = handgun_equipped_init.CopyWeapon();
        }
        if (pickup_equipped == null && pickup_equipped_init != null) {
            pickup_equipped = pickup_equipped_init.CopyWeapon();
        }
        PlayerCharacter.inst.inventory.rifle = rifle_equipped;
        PlayerCharacter.inst.inventory.handgun = rifle_equipped;
        PlayerCharacter.inst.inventory.pickup = pickup_equipped;

        attack_controller.SetWeaponsFromInventory(PlayerCharacter.inst.inventory);
        attack_controller.SwitchWeaponBySlot(slot_selected);
        Cleanup();
    }

    private void Cleanup() {
        PlayerCharacter.inst.UnsubscribeFromPlayer(this);
        if (destroy_all) {
            Destroy(gameObject);
        } else {
            Destroy(this);
        }
    }
}
