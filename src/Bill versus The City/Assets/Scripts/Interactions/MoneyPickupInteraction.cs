using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class MoneyPickupInteraction : MonoBehaviour, IInteractionEffect {
    public InteractableUI ui;
    public int dollars = 10;

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
        ui.SetNewText($"Pickup ${dollars}.00");
    }

    public void Interact(GameObject actor) {
        MenuManager.PlayMenuSound("menu_click");
        PlayerCharacter.inst.inventory.dollars_earned_in_level += this.dollars;
        Destroy(gameObject);
    }

}
