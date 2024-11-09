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

        SaveFile.SetupDirectory();
        // sets and mutates settings singleton
        try {
            SaveFile.current_save = SaveFile.Load(SaveFile.SAVE_SLOT_1);
        } catch (FileNotFoundException) {
            Debug.LogWarning($"no save at {SaveFile.SAVE_SLOT_1}, creating a new save");
            // DirectoryNotFoundException: Could not find a part of the path "C:\MY-documents\git-repos\BillVersusTheCity\src\Bill versus The City\Build\Bill versus The City_Data\.save_files\save_1".
            SaveFile.current_save = SaveFile.NewSave(SaveFile.SAVE_SLOT_1);
        }

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
