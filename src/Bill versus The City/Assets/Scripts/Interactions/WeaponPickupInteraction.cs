using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickupInteraction : MonoBehaviour, IInteractionEffect
{
    public ScriptableObject weapon;
    public bool active = true;

    public void Interact(GameObject actor) {
        if(! active) { return; }

        MenuManager.PlayMenuSound("menu_click");
        IWeapon pickup_weapon = (IWeapon) Instantiate(weapon);
        pickup_weapon.current_ammo = pickup_weapon.ammo_capacity;
        PlayerCharacter.inst.inventory.pickup = pickup_weapon;
        active = false;
    }
}
