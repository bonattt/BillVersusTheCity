using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopupLibrary : MonoBehaviour
{
    public const string TUTORIAL_MOVEMENT = "movement";
    public const string TUTORIAL_SHOOTING = "shooting";
    public const string TUTORIAL_COVER = "cover";
    // public const string TUTORIAL_ = "";

    private Dictionary<string, TutorialConfig> configs = new Dictionary<string, TutorialConfig>();


    private static TutorialPopupLibrary _inst;
    public static TutorialPopupLibrary inst {
        get {
            return _inst;
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
        configs[TUTORIAL_MOVEMENT] = new TutorialConfig(TUTORIAL_MOVEMENT, "move using [WASD]. Move the aim and camera with the mouse. Sprint by holding [Shift]");
        configs[TUTORIAL_SHOOTING] = new TutorialConfig(TUTORIAL_SHOOTING, "Use [mouse wheel] or numb-key 2 to equip your handgun. \nFire with [LMB]. \nHolding [RMB] will improve accuracy, but slows movement!");
        configs[TUTORIAL_COVER] = new TutorialConfig(TUTORIAL_COVER, "Holding [Space] or [L. Ctrl] close to cover will protect from enemies on the other side of the cover. Crouching while sprinting [Shift] in a direction will perform a dive, covering a short distance and takeing cover immediately."); 
    }

    public bool ShouldSkipTutorial(string tutorial_name) {
        if (GameSettings.inst.general_settings.skip_all_tutorials) { return true; }
        if (GameSettings.inst.debug_settings.debug_mode) { return false; } // never skip tutorials in debug mode??
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
        string str_keys = "";
        foreach(string key in configs.Keys) {
            str_keys += $"{key}, ";
        }
        string missing_str = $"Tutorial missing! name '{tutorial_name}' not in keys: {str_keys}";
        Debug.LogError(missing_str);
        return new TutorialConfig(tutorial_name, $"tutorial missing ({tutorial_name})!)", missing_str);
    }

}
