using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadStaticsIfNotLoaded : MonoBehaviour
{

    public bool load_hud = true;
    public GameObject hud_prefab, managers_prefab;

    void Awake()
    {
        if (load_hud && CombatHUDManager.inst == null) {
            Instantiate(hud_prefab);
        }
        if (ManagersManager.inst == null) {
            Instantiate(managers_prefab);
        }
        Destroy(gameObject);
    }
}
