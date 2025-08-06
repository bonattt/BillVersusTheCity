
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TutorialPopupLibrary : MonoBehaviour
{
    public const string TUTORIAL_MOVEMENT = "movement";
    public const string TUTORIAL_SHOOTING = "shooting";
    public const string TUTORIAL_COVER = "cover";
    public const string TUTORIAL_GRENADES = "grenades";
    // public const string TUTORIAL_ = "";

    private Dictionary<string, TutorialConfig> configs = new Dictionary<string, TutorialConfig>();


    private static TutorialPopupLibrary _inst;
    public static TutorialPopupLibrary inst {
        get {
            return _inst;
        }
    }
    private const string FILE_DIR = "tutorials";
    public static string directory_path {
        get {
            return Path.Join(Application.streamingAssetsPath, FILE_DIR);
        }
    }

    void Awake() {
        _inst = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        InitializeData();
    }

    public void InitializeData() {
        configs = new Dictionary<string, TutorialConfig>();
        string[] files = Directory.GetFiles(directory_path);
        foreach (string tutorial_file_path in files) {
            if (tutorial_file_path.EndsWith(".meta")) { continue; } // skip silly unity meta files
            string tutorial_name = Path.GetFileName(tutorial_file_path);
            if (tutorial_file_path.Contains('.')) {
                int i = tutorial_name.LastIndexOf('.');
                tutorial_name = tutorial_name.Substring(0, i); // removes file extension, if present
            } else {
                // tutorial_name = tutorial_name; // DO NOTHING
            }
            string tutorial_text = File.ReadAllText(tutorial_file_path);
            configs[tutorial_name] = new TutorialConfig(tutorial_name, tutorial_text);
        }
    }

    public bool ShouldSkipTutorial(string tutorial_name) {
        if (GameSettings.inst.general_settings.skip_all_tutorials) { return true; }
        if (GameSettings.inst.general_settings.skipped_tutorials.Contains(tutorial_name)) {
            return true;
        }
        // return !configs.ContainsKey(tutorial_name); // actually want an error'd tutorial to popup if the tutorial is missing...
        return false;
    }
    
    public TutorialConfig GetTutorial(string tutorial_name) {
        if (!configs.ContainsKey(tutorial_name)) {
            return MissingTutorial(tutorial_name);
        }
        return configs[tutorial_name];
    }

    public TutorialPopupController OpenTutorial(string tutorial_name) {
        if (ShouldSkipTutorial(tutorial_name)) {
            return null;  // no tutorial opened
        }
        TutorialConfig config = GetTutorial(tutorial_name);
        return MenuManager.inst.OpenTutorial(config);
    }

    private TutorialConfig MissingTutorial(string tutorial_name) {
        // returns a TutorialConfig to display if a tutorial lookup fails
        string str_keys = "";
        foreach(string key in configs.Keys) {
            str_keys += $"{key}, ";
        }
        string missing_str = $"Tutorial missing! name '{tutorial_name}' not in keys: '{str_keys}'";
        Debug.LogError(missing_str);
        return new TutorialConfig(tutorial_name, $"tutorial missing ({tutorial_name})!)", missing_str);
    }

}
