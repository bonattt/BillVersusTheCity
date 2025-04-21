using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadStaticsIfNotLoaded : MonoBehaviour
{

    public bool load_hud = true;
    public GameObject hud_prefab, managers_prefab;

    private static GameObject hud;

    void Awake()
    {
        if (load_hud) {
            if (CombatHUDManager.inst == null) {
                hud = Instantiate(hud_prefab);
            } else {
                hud.SetActive(true);
            }
        } 
        if (ManagersManager.inst == null) {
            Instantiate(managers_prefab);
        }
        Destroy(gameObject);
    }
}
