using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadStaticsIfNotLoaded : MonoBehaviour
{

    public GameObject managers_prefab;

    void Awake() {
        Debug.LogWarning("Awake()!"); // TODO --- remove debug
        if (ManagersManager.inst == null)
        {
            Debug.LogWarning("actually load statics!"); // TODO --- remove debug
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
