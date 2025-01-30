using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelConfig : MonoBehaviour
{

    public bool combat_enabled = true;
    public bool weapon_select_on_start = true;

    void Start()
    {
        // TODO ---
        if (weapon_select_on_start) {
            MenuManager.inst.OpenSubMenuPrefab(MenuManager.inst.weapon_menu_prefab);
        }   
    }
}
