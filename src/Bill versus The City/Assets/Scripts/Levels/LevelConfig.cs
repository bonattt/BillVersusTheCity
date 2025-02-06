using System;
using System.Collections;
using System.Collections.Generic;
using log4net.Core;
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

// public enum LevelMusicStart {
//     never,
//     on_load,
//     after_dialogue,
// }

public class LevelConfig : MonoBehaviour
{
    public static LevelConfig inst { get; private set; }
    public string next_level;
    public bool combat_enabled = true;
    public bool weapon_select_on_start = true;
    public bool use_starting_weapons = false;

    [SerializeField]
    private int sequential_conditions_index = 0;
    public string level_music_name;
    private ISounds level_music;

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

    void Awake() {
        inst = this;
    }
    
    void Start()
    {
        Configure();
        StartLevel();
    }

    void Update() {
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
        ISounds sound = SFXLibrary.LoadSound(level_music_name);
        return sound;
    }

    public void Configure() {
        Validate();
        // init condition lists
        sequential_level_conditions = InitConditions(init_extra_sequential_level_conditions);
        non_sequential_level_conditions = InitConditions(init_extra_non_sequential_level_conditions);
        level_music = LoadLevelMusic();
        ApplyPresetVictoryCondition();
        ApplyPresetFailureCondition();
    }
    
    public void StartLevel() {
        if (inst != null && inst != this) {
            Debug.LogWarning("clearing old level config"); // TODO --- remove debug
            Destroy(inst);
            Destroy(inst.gameObject);
        }
        inst = this;
        
        // TODO --- handle order of operations for select weapons UI
        if (dialogue_file_start_level != null && !dialogue_file_start_level.Equals("")) {
            MenuManager.inst.OpenDialoge(dialogue_file_start_level);
        }
        if (weapon_select_on_start) {
            MenuManager.inst.OpenSubMenuPrefab(MenuManager.inst.weapon_menu_prefab);
        } else if (use_starting_weapons) {
            EquipStartingWeapons();
        }
        StartLevelMusic();
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
        condition_obj.transform.parent = transform;

        ILevelCondition condition = condition_obj.GetComponent<ILevelCondition>();
        if (condition == null) {
            Debug.LogWarning($"instantiated condition prefab without ILevelCondition component! {condition_obj.name}");
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
                Debug.LogWarning($"unknown victory condition(s) preset '{victory_conditions_preset}'");
                return;
        }
    }

    private void ApplyPresetFailureCondition() {
        if (failure_conditions_preset == LevelFailuerConditions.none) {
            return; // do nothing
        } else if (failure_conditions_preset == LevelFailuerConditions.countdown) {
            // Debug.LogWarning("ApplyPresetFailureCondition: Not implented // TODO --- implement"); // TODO --- implement
            ILevelCondition condition = InstantiateFailureCondition(prefab_countdown_timer_condition);
            ConfigureCountdownCondition(condition, Color.red, preset_config_countdown_timer_seconds);

        } else {
            Debug.LogWarning($"unrecognized preset failure condition enum {failure_conditions_preset}");
        }
    }

    private static void ConfigureCountdownCondition(ILevelCondition condition, Color color, float countdown_start) {
        /* takes a level condition, which should be a Countdown Condition,
         * gets the associated UI and sets it's text color.
         * Handles error cases for all of the above
         */
        try {
            CountdownCondition countdown_condition = (CountdownCondition) condition;
            TimerUIController ui_ctrl = countdown_condition.gameObject.GetComponent<TimerUIController>();
            if (ui_ctrl == null) {
                Debug.LogError("Unable to get TimerUIController from countdown prefab!!");
                return;
            }
            ui_ctrl.text_color = color;
            countdown_condition.start_time_seconds = countdown_start;
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
        Debug.Log("LevelConfig.NextLevel()");
        ScenesUtil.NextLevel(next_level);
    }

    public void FailLevel() {
        DialogueController ctrl = OpenDialogueIfDefined(dialogue_file_level_failed);
        if (ctrl == null) {
            MenuManager.inst.PlayerDefeatPopup();
        } else {
            ctrl.AddDialogueCallback(new SimpleActionEvent(MenuManager.inst.PlayerDefeatPopup));
        }
    }

    private void CheckSequentialLevelConditions() {
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
                Debug.LogWarning($"script {b} cannot be cast to ILevelCondition");
            }
        }
        return conditions;
    }

    private void EquipStartingWeapons() {
        Debug.Log("use starting weapons!");
        starting_rifle = InstantiateStartingWeapon(init_starting_rifle);
        starting_handgun = InstantiateStartingWeapon(init_starting_handgun);
        starting_pickup = InstantiateStartingWeapon(init_starting_pickup);

        PlayerCharacter.inst.inventory.rifle = starting_rifle;
        PlayerCharacter.inst.inventory.handgun = starting_handgun;
        PlayerCharacter.inst.inventory.pickup = starting_pickup;
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
