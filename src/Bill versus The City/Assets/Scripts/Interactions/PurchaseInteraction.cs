using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class PurchaseInteraction : MonoBehaviour, IInteractionEffect, IPurchase {
    public InteractableUI ui;

    public bool destroy_after_purchase = true;
    public ScriptableObject purchased_item;
    public int _purchase_cost = 10;
    public int purchase_cost { get => _purchase_cost; }

    void Start() {
        UpdateUIText();
    }

    void Update() {
        UpdateUIText(); // TODO --- optimize this to not be called in update???
    }

    private void UpdateUIText() {
        if (ui == null) {
            Debug.LogWarning("no ui set foritem purcah interaction!");
            return;
        }
        try {
            ui.SetNewText($"Buy {((IItem) purchased_item).item_name} for ${purchase_cost}");
        } catch (InvalidCastException) {
            ui.SetNewText($"invalid cast, {purchased_item} is not a valid item");
        }
    }

    public void Interact(GameObject actor) {
        if (PlayerCharacter.inst.inventory.CanPurchase(this)) {
            MenuManager.PlayMenuClick();
            PlayerCharacter.inst.inventory.Purchase(this);
            if (destroy_after_purchase) {
                Destroy(gameObject);
            }
        }
        else {
            Debug.LogWarning($"insufficient funds! ${PlayerCharacter.inst.inventory.total_dollars} < ${purchase_cost}");
            MenuManager.PlayMenuErrorClick();
        }
        // PlayerCharacter.inst.inventory.dollars_earned_in_level += this.dollars;
    }
    
    public void ApplyPurchase(PlayerInventory inv) {
        inv.AddWeapon(((IWeapon) purchased_item).CopyWeapon());
    }

}
