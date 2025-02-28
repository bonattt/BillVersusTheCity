using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWeaponsInteraction : MonoBehaviour, IInteractionEffect
{
    public void Interact(GameObject actor) {
        GameObject menu = MenuManager.inst.OpenSubMenuPrefab(MenuManager.inst.weapon_menu_prefab);
        WeaponSelectionUIController menu_ctrl = menu.GetComponent<WeaponSelectionUIController>();
        menu_ctrl.allow_cancel = true;
        menu_ctrl.clear_pickup_weapon = false;
    }
}
