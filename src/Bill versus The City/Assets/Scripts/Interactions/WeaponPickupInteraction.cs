using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponPickupInteraction : MonoBehaviour, IInteractionEffect {
    public InteractableUI ui;
    public ScriptableObject unity_weapon;
    public IFirearm pickup_weapon;
    public bool full_ammo = false; // if true, the weapon will have it's ammo set to full

    [Tooltip("set the pickup item's ammo to this value. If less than 0, do not change the weapon's ammo count. if FULL AMMO is set, also ignore this setting")]
    public int set_ammo = -1;

    void Start() {
        if (pickup_weapon == null && unity_weapon != null) {
            pickup_weapon = (IFirearm)Instantiate(unity_weapon);
            if (full_ammo) {
                pickup_weapon.current_ammo = pickup_weapon.ammo_capacity;
            } else if (set_ammo > -1) {
                if (set_ammo > pickup_weapon.ammo_capacity) {
                    pickup_weapon.current_ammo = pickup_weapon.ammo_capacity;
                } else {
                    pickup_weapon.current_ammo = set_ammo;
                }
            }
        }
        UpdateUIText();
    }

    void Update() {
        UpdateUIText(); // TODO --- optimize this to not be called in update???
    }

    private void UpdateUIText() {
        if (ui == null) {
            Debug.LogWarning("no ui set for pickup!");
            return;
        }
        string verb = "Pickup";
        if (PlayerCharacter.inst.inventory.pickup != null) {
            verb = "Swap to";
        }
        ui.SetNewText($"{verb} {pickup_weapon.item_name}");
    }

    public void Interact(GameObject actor) {
        if (pickup_weapon == null) {
            Debug.LogError("either `unity_weapon` or `pickup_weapon` must be set!");
            return;
        }

        if (IsCarriedConsumable()) {
            int total_ammo = pickup_weapon.current_ammo + PlayerCharacter.inst.inventory.pickup.current_ammo;
            if (total_ammo > PlayerCharacter.inst.inventory.pickup.ammo_capacity) {
                // ammo needed to reach max ammo
                int ammo_needed = PlayerCharacter.inst.inventory.pickup.ammo_capacity - PlayerCharacter.inst.inventory.pickup.current_ammo;
                pickup_weapon.current_ammo -= ammo_needed;
                PlayerCharacter.inst.inventory.pickup.current_ammo += ammo_needed;
            } else {
                PlayerCharacter.inst.inventory.pickup.current_ammo += pickup_weapon.current_ammo;
                pickup_weapon = null; // pickup all the remaining grenades
            }
            PlayerCharacter.inst.inventory.UpdateWeapon(); // manually update UI because of ammo change
            Debug.LogWarning("// TODO --- update weapon ammo display!"); // TODO --- update weapon ammo display!
        } else {
            // pickup_weapon.current_ammo = pickup_weapon.ammo_capacity;
            IFirearm dropped_weapon = PlayerCharacter.inst.inventory.pickup;
            if (dropped_weapon.is_consumable && dropped_weapon.current_ammo == 0) {
                // if the weapon is a consumable, and it's out of ammo, do not drop it
                dropped_weapon = null;
            }
            PlayerCharacter.inst.inventory.pickup = pickup_weapon;
            pickup_weapon = dropped_weapon;
        }
        MenuManager.PlayMenuSound("menu_click");

        if (pickup_weapon == null) {
            // remove the pickup that's been picked up
            Destroy(gameObject);
        } else {
            UpdateUIText();
        }
    }

    private bool IsCarriedConsumable() {
        // returns true if the player is already carrying this pickup item, AND the item is a consumable, so the ammo can be transfered
        IFirearm player_pickup_weapon = PlayerCharacter.inst.inventory.pickup;
        // if the pickup is a consumable, AND it's the same as the pickup item curretly carried by the player,
        return pickup_weapon.is_consumable && player_pickup_weapon != null && pickup_weapon.item_id.Equals(player_pickup_weapon.item_id);
    }
}
