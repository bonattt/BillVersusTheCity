using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWeaponSelectionMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MenuManager.inst.OpenSubMenuPrefab(MenuManager.inst.weapon_menu_prefab);
        Destroy(gameObject);
    }
}
