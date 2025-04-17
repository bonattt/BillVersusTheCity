using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class InitializeSettings : MonoBehaviour
{

    // Start is called before the first frame update
    void Awake() {
        // if (GameSettings.inst == null) {
        //     new GameSettings();
        // }


        Destroy(gameObject);
    }

    public static void Initialize() {
    }

    // public bool save = false;
    // void Update() {
    //     if (save) {
    //         save = false;
    //         SaveFile save_file = new SaveFile(SaveFile.SAVE_SLOT_1);
    //         save_file.Save();
    //     }
    // }
}
