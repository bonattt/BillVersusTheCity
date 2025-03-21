using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class AmmoPickupInteraction : MonoBehaviour, IInteractionEffect {
    public InteractableUI ui;

    public AmmoType ammo_type = AmmoType.handgun;
    public int ammo_amount = 10;

    void Start() {
        UpdateUIText();
    }

    void Update() {
        UpdateUIText(); // TODO --- optimize this to not be called in update???
    }

    private void UpdateUIText() {
        if (ui == null) {
            Debug.LogWarning("no ui set for ammo pickup!");
            return;
        }
        if(CanPlayerPickupAmmo()) {
            ui.SetNewText($"Pickup {ammo_amount} {ammo_type} ammo");
        } else {
            ui.SetNewText($"{ammo_type} ammo is full!");
        } 
    }

    public bool CanPlayerPickupAmmo() {
        // returns true of the player is able to pickup ammo of this type.
        // if the player doesn't track ammo of this type, or is already full, return false.
        if (PlayerCharacter.inst.reload_ammo == null) { 
            Debug.LogWarning("no player ammo!");
            return false;
        }
        return PlayerCharacter.inst.reload_ammo.HasAmmoType(ammo_type) && PlayerCharacter.inst.reload_ammo.AmmoNeeded(ammo_type) >= 1;
    }

    public void Interact(GameObject actor) {
        if (CanPlayerPickupAmmo()) {
            MenuManager.PlayMenuSound("menu_click");
            int count = PlayerCharacter.inst.reload_ammo.GetCount(ammo_type);
            PlayerCharacter.inst.reload_ammo.SetCount(ammo_type, count + ammo_amount);
            Destroy(gameObject);
        } 
    }

}
