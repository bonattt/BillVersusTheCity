using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWeaponsInteraction : MonoBehaviour, IInteractionEffect
{
    public void Interact(GameObject actor) {
        MenuManager.inst.OpenSubMenuPrefab(MenuManager.inst.weapon_menu_prefab);
    }
}
