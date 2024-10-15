using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadStaticsIfNotLoaded : MonoBehaviour
{

    public GameObject hud_prefab, managers_prefab;

    void Awake()
    {
        if (CombatHUDManager.inst == null) {
            Instantiate(hud_prefab);
        }
        if (ManagersManager.inst == null) {
            Instantiate(managers_prefab);
        }
        Destroy(gameObject);
    }
}
