using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickupInteraction : MonoBehaviour, IInteractionEffect
{
    public ScriptableObject weapon;

    public void Interact(GameObject actor) {
        MenuManager.PlayMenuSound("menu_click");
        PlayerCharacter.inst.inventory.pickup = (IWeapon) Instantiate(weapon);
    }
}
