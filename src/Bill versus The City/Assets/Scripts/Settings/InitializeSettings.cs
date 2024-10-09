using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeSettings : MonoBehaviour
{

    // Start is called before the first frame update
    void Awake() {
        // if (GameSettings.inst == null) {
        //     new GameSettings();
        // }

        // sets and mutates settings singleton
        SaveFile.current_save = SaveFile.Load(SaveFile.SAVE_SLOT_1);

        Destroy(gameObject);
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
