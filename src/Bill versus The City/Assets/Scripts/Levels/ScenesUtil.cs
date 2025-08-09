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

    public static bool LoadSceneAdditively(string scene_name) {
        // Loads the given scene additively, and returns true if the scene was actually loaded.
        // before loading the scene, checks if the scene is already loaded, and ignores the load if the scene is already loaded.
        // (this is mainly to avoid duplicates playing a scene from the editor)
        Scene scene = SceneManager.GetSceneByName(scene_name);
        bool already_loaded = scene.isLoaded;
        if (!already_loaded) {
            SceneManager.LoadSceneAsync(scene_name, LoadSceneMode.Additive);
        }
        return !already_loaded;
    }

    private static void ResetResources() {
        ObjectTracker.inst.Reset();
        
        if (LevelConfig.inst != null) {
            LevelConfig.inst.PreSceneChange();
        }
        if (EnemiesManager.inst != null) {
            EnemiesManager.inst.Reset();
        }
        if (PlayerCharacter.inst != null && PlayerCharacter.inst.inventory != null) {
            PlayerCharacter.inst.inventory.ResetLevel();
        }
    }

    public static string GetCurrentSceneName() {
        return SceneManager.GetActiveScene().name;
    }

    public static void RestartLevel() {
        restarting = true;
        was_restarted = true;
        ResetResources();
        SceneManager.LoadScene(GetCurrentSceneName());
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

    public static void ExitToMainMenu() {
        MenuManager.inst.CloseAllMenus();
        NextLevel("MainMenu");
    }

    public static void ExitGame() {
        // TODO --- move this code somewhere more suitable
        Debug.Log("Game is exiting...");
        if(SaveProfile.inst.save_file != null) {
            SaveProfile.inst.save_file.SaveOnExit();
        }

        // preprocessor #if, #else, #endif optimizes the code by excluding code sections at compile time instead of runtime
        #if UNITY_EDITOR
            // If running in the Unity Editor, stop playing the scene
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // If running in a standalone build, quit the application
            Application.Quit();
        #endif
    }

    // public static void LoadFloor(string scene_name) {
    //     loaded_floors.Add(GetCurrentSceneName());
    //     loaded_floors.Add(scene_name);
    //     SceneManager.LoadScene(scene_name, LoadSceneMode.Additive);
    // }
}