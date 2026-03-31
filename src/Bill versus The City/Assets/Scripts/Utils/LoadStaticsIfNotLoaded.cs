using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadStaticsIfNotLoaded : MonoBehaviour
{

    public GameObject managers_prefab;

    void Awake() {
        if (ManagersManager.inst == null)
        {
            Instantiate(managers_prefab);
            LoadSaveFile();
        }
        Destroy(gameObject);
    }

    private void LoadSaveFile() {
        // TODO --- error handling
        SaveProfile.inst.LoadSaveData();
    }
}
