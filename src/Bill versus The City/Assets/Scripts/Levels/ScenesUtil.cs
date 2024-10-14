using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class ScenesUtil {
    
    private static HashSet<string> loaded_floors = new HashSet<string>();

    public static void RestartLevel() {
        EnemiesManager.inst.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
         // this fails to fix 
    }

    public static void NextLevel(string next_level) {
        EnemiesManager.inst.Reset();
        SceneManager.LoadScene(next_level);
    }

    private static PlayerCombat RegisterScenePlayer() {
        GameObject obj = GetPlayerObjectFromScene();
        if (obj != null) {
            PlayerCombat player = obj.GetComponent<PlayerCombat>();
            PlayerCharacter.inst.PlayerUpdated(player);
            return player;
        }

        Debug.LogWarning("No player found in scene");
        return null;
    }

    private static GameObject GetPlayerObjectFromScene() {
        GameObject[] game_objects = SceneManager.GetActiveScene().GetRootGameObjects();
        Debug.LogWarning($"");  // TODO --- remove debug
        for (int i = 0; i < game_objects.Length; i++) {
            if (game_objects[i].name.Equals("Player")) {
                return game_objects[i];
            }
        }
        Debug.LogWarning("no object 'Player' found in current scene");
        return null;  
    }

    // public static void LoadFloor(string scene_name) {
    //     loaded_floors.Add(SceneManager.GetActiveScene().name);
    //     loaded_floors.Add(scene_name);
    //     SceneManager.LoadScene(scene_name, LoadSceneMode.Additive);
    // }
}