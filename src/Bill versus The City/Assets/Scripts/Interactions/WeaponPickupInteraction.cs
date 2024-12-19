using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponPickupInteraction : MonoBehaviour, IInteractionEffect
{
    public InteractableUI ui;
    public ScriptableObject unity_weapon;
    public IWeapon pickup_weapon;
    public bool full_ammo = false; // if true, the weapon will have it's ammo set to full

    void Start() {
        if (pickup_weapon == null && unity_weapon != null) {
            pickup_weapon = (IWeapon) Instantiate(unity_weapon);
            if (full_ammo) {
                pickup_weapon.current_ammo = pickup_weapon.ammo_capacity;
            }
        }
        UpdateUIText();
    }

    private void UpdateUIText() {
        if (ui != null) {
            ui.SetNewText($"Pickup {pickup_weapon.item_name}");
        } else {
            Debug.LogWarning("no ui set for pickup!");
        }
    }

    public void Interact(GameObject actor) {
        if (pickup_weapon == null) {
            Debug.LogError("either `unity_weapon` or `pickup_weapon` must be set!");
            return;
        }
        MenuManager.PlayMenuSound("menu_click");
        // pickup_weapon.current_ammo = pickup_weapon.ammo_capacity;
        IWeapon dropped_weapon = PlayerCharacter.inst.inventory.pickup;
        PlayerCharacter.inst.inventory.pickup = pickup_weapon;
        pickup_weapon = dropped_weapon;
        
        if (pickup_weapon == null) {
            // remove the pickup that's been picked up
            Destroy(gameObject);
        }
        else {
            UpdateUIText();
        }
    }
}
