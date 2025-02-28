using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class ScenesUtil {
    
    private static HashSet<string> loaded_floors = new HashSet<string>();

    private static bool was_restarted = false;
    private static bool restarting = false;
    public static bool IsRestartInProgress() {
        return restarting;
    }
    public static bool WasRestarted() {
        return was_restarted;
    }

    private static void ResetResources() {
        LevelConfig.inst.PreSceneChange();
        if (EnemiesManager.inst != null) {
            EnemiesManager.inst.Reset();
        }
    }

    public static void RestartLevel() {
        restarting = true;
        was_restarted = true;
        ResetResources();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // LevelConfig.inst.PostRestart();
        // LevelConfig.inst.level_restarted = true;
        restarting = false;
    }

    public static void NextLevel(string next_level) {
        was_restarted = false;
        ResetResources();
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