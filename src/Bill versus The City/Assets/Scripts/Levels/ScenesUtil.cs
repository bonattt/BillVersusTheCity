using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class ScenesUtil {
    
    private static HashSet<string> loaded_floors = new HashSet<string>();

    public static void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void LoadFloor(string scene_name) {
        loaded_floors.Add(SceneManager.GetActiveScene().name);
        loaded_floors.Add(scene_name);
        SceneManager.LoadScene(scene_name, LoadSceneMode.Additive);
    }
}