using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadStaticsIfNotLoaded : MonoBehaviour
{

    public GameObject managers_prefab;

    void Awake() {
        if (ManagersManager.inst == null)
        {
            LoadSaveFile();
            Instantiate(managers_prefab);
        }
        Destroy(gameObject);
    }

    private void LoadSaveFile() {
        // TODO --- error handling
        SaveProfile.inst.LoadSaveData();
    }
}
