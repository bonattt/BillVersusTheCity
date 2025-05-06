using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public enum LevelVictoryType {
    leave_by_truck,
    instant
}

// enum implements a dropdown in the unity inspector for pre-configured options for victory conditions
public enum LevelVictoryConditions {
    none,
    clear_enemies,
    escape_to_truck, 
    survive_countdown, 
}

public enum LevelFailuerConditions {
    none,
    countdown,
}

public enum LevelWeaponSelect {
    none, // start with no weapons equipped
    level_weapons, // gives the player weapons specified by the level config
    weapon_select, // opens weapon select UI at the start of the level
}

// public enum LevelMusicStart {
//     never,
//     on_load,
//     after_dialogue,
// }

public class LevelConfig : MonoBehaviour
{
    [SerializeField]
    private uint level_number = 0;
    private static uint level_counter = 1;

    public static LevelConfig inst { get; private set; }
    public string next_level;
    public bool combat_enabled = true;

    [SerializeField]
    private int sequential_conditions_index = 0;
    public string level_music_name;
    private ISounds level_music;

    public bool weapon_select_on_start { 
        get {
            // NOTE: planning to add additional options here that change the availible selection of weapons between all, or just weapons unlocked by the player
            return level_weapons == LevelWeaponSelect.weapon_select;
        }
    }
    public LevelWeaponSelect level_weapons = LevelWeaponSelect.weapon_select;
    public LevelVictoryConditions victory_conditions_preset = LevelVictoryConditions.clear_enemies;
    public LevelFailuerConditions failure_conditions_preset = LevelFailuerConditions.none; 
    public LevelVictoryType victory_type = LevelVictoryType.leave_by_truck;

    public ScriptableObject init_starting_rifle, init_starting_handgun, init_starting_pickup;
    private IWeapon starting_rifle, starting_handgun, starting_pickup;

    public string dialogue_file_start_level, dialogue_file_objectives_complete, dialogue_file_level_failed, dialogue_file_level_finished;

    public float preset_config_countdown_timer_seconds = 75;

    public List<MonoBehaviour> init_extra_sequential_level_conditions, init_extra_non_sequential_level_conditions;
    private List<ILevelCondition> sequential_level_conditions = new List<ILevelCondition>();
    private List<ILevelCondition> non_sequential_level_conditions = new List<ILevelCondition>();
    public GameObject prefab_countdown_timer_condition, prefab_clear_enemies_condition;
    public bool level_started = false;
    public bool level_restarted {
        get { return ScenesUtil.WasRestarted(); }
    }

    public bool test_mode = false; // disables some level features to streamline testing
    [SerializeField]
    [Tooltip("if true, the unity editor will pause whenever a level is failed, so the state of the game can be inspected.")]
    private bool pause_on_level_failure = false;

    public bool has_start_dialogue {
        get {
            return true;
        }
    }

    [SerializeField]
    private int level_start_dollars = -1; 

    void Awake() {
        inst = this;
    }
    
    void Start()
    {
        PlayerCharacter.inst.inventory.dollars_earned_in_level = 0;
        level_number = level_counter++;
        gameObject.name += $" ({level_number})";
        ConfigureLevel();
        SetupStartLevelCallbackOrStart();
        // StartLevel();
        if (inst != this) {
            Debug.LogWarning("level not set to inst");
        }
    }

    void Update() {
        if (!level_started) { return; } // level not started yet
        CheckSequentialLevelConditions();
        bool level_failed = CheckFailLevelConditions();

        if(level_failed) {
            FailLevel();
        }
    }

    void OnDestroy() {
        CleanupLevel();
    }

    private ISounds LoadLevelMusic() {
        if (level_music_name == null || level_music_name.Equals("")) {
            Debug.Log($"no level music set: '{level_music_name}'");
            return null;
        }
        if (test_mode) {
            Debug.LogWarning($"skipping level music because of test mode");
            return null;
        }
        ISounds sound = SFXLibrary.LoadSound(level_music_name);
        return sound;
    }

    public bool has_level_start_dialogue {
        get {
            // returns a bool if dialogue should open before the level starts.
            // never open dialogue if the level was restarted
            bool _has_dialogue = !level_restarted && DialogueFileNameValid(dialogue_file_start_level);
            if (test_mode && _has_dialogue) {
                Debug.LogWarning("skipping level dialogue because level is in test mode!");
            }
            return !test_mode && _has_dialogue;
        }
    }

    public bool DialogueFileNameValid(string file_name) {
        return file_name != null && !file_name.Equals("");
    }

    public void SetupStartLevelCallbackOrStart() {
        if (weapon_select_on_start) {
            GameObject menu_obj = MenuManager.inst.OpenWeaponSelectMenu();
            ICloseEventMenu menu = menu_obj.GetComponent<ICloseEventMenu>();
            if (has_level_start_dialogue) {
                menu.AddCloseCallback(new SimpleActionEvent(OpenLevelStartDialouge));
            } else {
                menu.AddCloseCallback(new SimpleActionEvent(StartLevel));
            }
        } 
        else if (has_level_start_dialogue) {
            OpenLevelStartDialouge();
        } else {
            // just start the level immediately, no start menus
            StartLevel();
        }
    }

    public void PreSceneChange() {
        // prepare LevelConfig for a level scene change to avoid side effects 
        sequential_level_conditions = new List<ILevelCondition>();
        non_sequential_level_conditions = new List<ILevelCondition>();
    }

    public void OpenLevelStartDialouge() {
        DialogueController ctrl = MenuManager.inst.OpenDialoge(dialogue_file_start_level);
        ctrl.AddCloseCallback(new SimpleActionEvent(StartLevel));
    }

    private bool has_timer_condition = false;
    public void ConfigureLevel() {
        Validate();
        // init condition lists
        // Debug.LogWarning("TODO --- reimplement victory condition HUD"); // TODO --- remove debug
        CombatHUDManager.inst.victory_type = victory_conditions_preset;
        sequential_level_conditions = InitConditions(init_extra_sequential_level_conditions);
        non_sequential_level_conditions = InitConditions(init_extra_non_sequential_level_conditions);
        level_music = LoadLevelMusic();
        ApplyPresetVictoryCondition();
        ApplyPresetFailureCondition();
        ConfigureCountdonwUI();
        level_started = false;
        inst = this;

        if (level_weapons == LevelWeaponSelect.level_weapons) {
            EquipLevelWeapons();
        } else if (level_weapons == LevelWeaponSelect.none) {
            Debug.LogWarning("`LevelWeaponSelect.none` is not implemented!");
        }
    }
    private void ConfigureCountdonwUI() {
        // ensures that the countdonw HUD is disabled if a timer condition was never added
        if (!has_timer_condition) {
            CombatHUDManager.inst.HideCountdownHUD();
        } else {
            // do nothing, CombatHUDManager.inst.ConfigureCountdown should be called in this.ConfigureCountdownCondition when has_timer_condition is set.
        }
    }
    
    public void StartLevel() {
        if (inst != null && inst != this) {
            Destroy(inst);
            Destroy(inst.gameObject);
        }
        StartLevelMusic();
        level_started = true;
    }

    public void StartLevelMusic() {
        if (level_music == null) { return; } // level has no music
        SFXSystem.inst.PlayMusic(level_music);
    }

    protected void Validate() {
        // Logs warnings for some invalid configuration states
        if (victory_type == LevelVictoryType.leave_by_truck && victory_conditions_preset == LevelVictoryConditions.escape_to_truck) {
            Debug.LogWarning("Objective cannot be escape by truck if victory type is also to escape by truck"); 
        }

        if (victory_conditions_preset == LevelVictoryConditions.survive_countdown && failure_conditions_preset == LevelFailuerConditions.countdown) {
            Debug.LogWarning("victory and failure conditions are both countdown!!");
        }

        if (victory_conditions_preset == LevelVictoryConditions.none && init_extra_sequential_level_conditions.Count == 0) {
            Debug.LogWarning("neither preset nor custom victory conditions provided!");
        }
    }

    protected ILevelCondition InstantiateConditionFromPrefab(GameObject prefab) {
        GameObject condition_obj = Instantiate(prefab);
        condition_obj.name += $" ({level_number})";
        condition_obj.transform.parent = transform;

        ILevelCondition condition = condition_obj.GetComponent<ILevelCondition>();
        if (condition == null) {
            Debug.LogError($"instantiated condition prefab without ILevelCondition component! {condition_obj.name}");
        }
        return condition;
    }

    protected ILevelCondition InstantiateVictoryConditionInstant(GameObject prefab) {
        // instantiates a victory condition from a prefab of a GameObject containing an ILevelCondition script,
        // and sets the condition to instantly end the level
        ILevelCondition condition = InstantiateConditionFromPrefab(prefab);
        condition.AddEffect(new SimpleActionEvent(LevelObjectivesCleared));

        sequential_level_conditions.Add(condition);
        return condition;
    }


    protected ILevelCondition InstantiateFailureCondition(GameObject prefab) {
        // create conditions from prefab
        ILevelCondition condition = InstantiateConditionFromPrefab(prefab);
        condition.AddEffect(new SimpleActionEvent(this.FailLevel));

        // add the condition to the level for evaluation
        non_sequential_level_conditions.Add(condition);

        return condition;
    }

    private void ApplyPresetVictoryCondition() {
        switch(victory_conditions_preset) {
            case LevelVictoryConditions.none:
                return; // do nothing
            
            case LevelVictoryConditions.clear_enemies:
                InstantiateVictoryConditionInstant(prefab_clear_enemies_condition);
                return;
                
            case LevelVictoryConditions.escape_to_truck:
                // no conditions are required, just activate the truck exit
                ActivateTruckLevelExit();
                return; 

            case LevelVictoryConditions.survive_countdown:
                ILevelCondition condition = InstantiateVictoryConditionInstant(prefab_countdown_timer_condition);
                ConfigureCountdownCondition(condition, Color.green, preset_config_countdown_timer_seconds);
                return;
                
            default:
                Debug.LogError($"unknown victory condition(s) preset '{victory_conditions_preset}'");
                return;
        }
    }

    private void ApplyPresetFailureCondition() {
        if (failure_conditions_preset == LevelFailuerConditions.none) {
            return; // do nothing
        } else if (failure_conditions_preset == LevelFailuerConditions.countdown) {
            ILevelCondition condition = InstantiateFailureCondition(prefab_countdown_timer_condition);
            ConfigureCountdownCondition(condition, Color.red, preset_config_countdown_timer_seconds);

        } else {
            Debug.LogWarning($"unrecognized preset failure condition enum {failure_conditions_preset}");
        }
    }

    private void ConfigureCountdownCondition(ILevelCondition condition, Color color, float countdown_start) {
        /* takes a level condition, which should be a Countdown Condition,
         * gets the associated UI and sets it's text color.
         * Handles error cases for all of the above
         */
        try {
            CountdownCondition countdown_condition = (CountdownCondition) condition;
            // TimerUIController ui_ctrl = countdown_condition.gameObject.GetComponent<TimerUIController>();
            // if (ui_ctrl == null) {
            //     Debug.LogError("Unable to get TimerUIController from countdown prefab!!");
            //     return;
            // }
            // ui_ctrl.text_color = color;
            countdown_condition.start_time_seconds = countdown_start;
            CombatHUDManager.inst.ConfigureCountdown(countdown_condition, color);
            has_timer_condition = true;
        } catch (InvalidCastException) {
            Debug.LogError($"Casting error trying to setup countdown failure condition");
        }
    }

    private DialogueController OpenDialogueIfDefined(string dialogue_file) {
        // takes a string defining a filepath to a dialogue file.
        // if the string is valid (not null and not empty), open a dialouge and return the controller
        // if the string is invalid (null or empty) return null, and do not open a dialogue
        if (dialogue_file == null || dialogue_file.Equals("")) {
            return null;
        }
        return MenuManager.inst.OpenDialoge(dialogue_file);
    }

    public void ActivateTruckLevelExit() {
        GameObject truck = GameObject.Find("PlayerTruck");
        if (truck == null) { 
            Debug.LogError("cannot find 'PlayerTruck' in the scene to activate level exit!");
            return;
        }
        Transform child = truck.transform.Find("FinishLevelInteraction");
        if (child == null) {
            Debug.LogError("'PlayerTruck' is missing its finish level interaction!!");
            return;
        }
        Interaction finish_level = child.gameObject.GetComponent<Interaction>();
        if (finish_level == null) {
            Debug.LogError("FinishLevelInteraction is missing it's `Interaction` component!");
            return;
        }
        finish_level.interaction_enabled = true;
        child.gameObject.SetActive(true);
    }

    public void LevelObjectivesCleared() {
        DialogueController ctrl = OpenDialogueIfDefined(dialogue_file_objectives_complete);

        switch (this.victory_type) {
            case LevelVictoryType.instant:
                if (ctrl == null) {
                    // if there is no dialogue, immediately CompleteLevel
                    CompleteLevel();
                } else {
                    // if there is dialogue, set CompleteLevel as a callback on the dialogue controller
                    ctrl.AddDialogueCallback(new SimpleActionEvent(CompleteLevel));
                }
                return;
            case LevelVictoryType.leave_by_truck:
                ActivateTruckLevelExit();
                return;

            default:
                Debug.LogError($"unhandled victory type '{this.victory_type}'");
                break;
        }
    }

    public void CleanupLevel() {
        SFXSystem.inst.StopMusic();
    }

    public void CompleteLevel() {
        // TODO --- add configurations for what "next level" does, besides just loading a new scene
        DialogueController ctrl = OpenDialogueIfDefined(dialogue_file_level_finished);
        if (ctrl == null) {
            NextLevel(); 
        } else {
            ctrl.AddDialogueCallback(new SimpleActionEvent(NextLevel));
        }
    }

    public void NextLevel() {
        CleanupLevel();
        PlayerCharacter.inst.inventory.StoreEarnedDollars();
        SaveProfile.inst.save_file.SaveProgress(current_scene: next_level);
        ScenesUtil.NextLevel(next_level);
    }

    public void FailLevel() {
        #if UNITY_EDITOR
            EditorApplication.isPaused = pause_on_level_failure;
        #endif
        DialogueController ctrl = OpenDialogueIfDefined(dialogue_file_level_failed);
        if (ctrl == null) {
            MenuManager.inst.PlayerDefeatPopup();
        } else {
            ctrl.AddDialogueCallback(new SimpleActionEvent(MenuManager.inst.PlayerDefeatPopup));
        }
    }

    private void CheckSequentialLevelConditions() {
        if (!level_started) { return; /* don't check if level isn't started yet */ }
        for (int i = sequential_conditions_index; i < sequential_level_conditions.Count; i++) {
            ILevelCondition current_condition = sequential_level_conditions[i];
            if (current_condition.was_triggered) { 
                continue; 
            } // skip already triggered conditions
            else if (!current_condition.ConditionMet()) {
                // current condition is NOT met, so we stop iterating. 
                break;
            } else {
                // current condition was met, trigger it's effects, mark as done, and continue to next condition
                current_condition.TriggerEffects(); // sets `was_triggered = true`
                sequential_conditions_index = i + 1;
            }
        }
    }
    
    private bool CheckFailLevelConditions() {
        /* checks all the level fail conditions, if any is true, returns true and evaluates that conditions triggers.
         */
        if (!level_started) { return false; /* don't check if level isn't started yet */ }
        for (int i = 0; i < non_sequential_level_conditions.Count; i++) {
            ILevelCondition current_condition = non_sequential_level_conditions[i];
            if (current_condition.was_triggered) { 
                continue; 
            } // skip already triggered conditions
            else if (!current_condition.ConditionMet()) {
                // current condition is NOT met, so we stop iterating. 
                continue;
            } else {
                // current condition was met, trigger it's effects, mark as done, and continue to next condition
                current_condition.TriggerEffects(); // sets `was_triggered = true`
                return true;
            }
        }
        return false;
    }

    private static List<ILevelCondition> InitConditions(List<MonoBehaviour> init_scripts) {
        List<ILevelCondition> conditions = new List<ILevelCondition>();
        for(int i = 0; i < init_scripts.Count; i++) {
            MonoBehaviour b = init_scripts[i];
            if (b == null) {
                Debug.LogWarning("empty script slot in LevelConfig init conditions");
                continue;
            }
            try {
                ILevelCondition c = (ILevelCondition) b;
                c.was_triggered = false;
                conditions.Add(c);
            } catch (InvalidCastException) {
                Debug.LogError($"script {b} cannot be cast to ILevelCondition");
            }
        }
        return conditions;
    }

    private void EquipLevelWeapons() {
        Debug.Log("use starting weapons!");
        starting_rifle = InstantiateStartingWeapon(init_starting_rifle);
        starting_handgun = InstantiateStartingWeapon(init_starting_handgun);
        starting_pickup = InstantiateStartingWeapon(init_starting_pickup);

        PlayerCharacter.inst.inventory.EquipStartingWeapons(starting_rifle, starting_handgun, starting_pickup);
    }

    private IWeapon InstantiateStartingWeapon(ScriptableObject scriptable_object) {
        /* Instantiates an IWeapon from a generic scriptable object, which implements that interface */

        if (scriptable_object == null) { return null; }
        IWeapon weapon;
        try {
            weapon = ((IWeapon) scriptable_object).CopyWeapon();
        } catch (InvalidCastException) {
            Debug.LogError($"ScriptableObject {scriptable_object} not castable to a valid IWeapon.");
            return null;
        }   
        weapon.current_ammo = weapon.ammo_capacity;
        return weapon;
    }


}
