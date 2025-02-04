using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public enum LevelVictoryType {
    leave_by_truck,
    instant
}

public class LevelConfig : MonoBehaviour
{

    public string next_level;
    public bool combat_enabled = true;
    public bool weapon_select_on_start = true;

    [SerializeField]
    private int sequential_conditions_index = 0;

    public LevelVictoryType victory_type = LevelVictoryType.leave_by_truck;

    public static LevelConfig inst = null;

    public bool use_starting_weapons = false;

    public ScriptableObject init_starting_rifle, init_starting_handgun, init_starting_pickup;
    private IWeapon starting_rifle, starting_handgun, starting_pickup;

    public List<MonoBehaviour> init_sequential_level_conditions, init_fail_level_conditions;
    private List<ILevelCondition> sequential_level_conditions = new List<ILevelCondition>();
    private List<ILevelCondition> fail_level_conditions = new List<ILevelCondition>();

    void Awake() {
        inst = this;
    }

    void Start()
    {
        // init condition lists
        sequential_level_conditions = InitConditions(init_sequential_level_conditions);
        fail_level_conditions = InitConditions(init_fail_level_conditions);

        if (inst != null && inst != this) {
            Debug.LogWarning("clearing old level config"); // TODO --- remove debug
            Destroy(inst);
            Destroy(inst.gameObject);
        }
        inst = this;
        // TODO ---
        if (weapon_select_on_start) {
            MenuManager.inst.OpenSubMenuPrefab(MenuManager.inst.weapon_menu_prefab);
        } else if (use_starting_weapons) {
            EquipStartingWeapons();
        }
    }

    void Update() {
        bool level_failed = CheckFailLevelConditions();
        CheckSequentialLevelConditions();

        if(level_failed) {
            FailLevel();
        }
    }

    public void CompleteLevelObjectives() {
        // public method to be called when all a level's objectives are cleared
        // Exact effects of clearing the objectives may vary
        if (victory_type == LevelVictoryType.leave_by_truck) {
            ActivateTruckLevelExit();
        } else if (victory_type == LevelVictoryType.instant) {
            Debug.LogWarning("TODO ---");
        }
        else {
            Debug.LogError($"unknown level victory type: '{victory_type}'");
        }

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

    public void NextLevel() {
        ScenesUtil.NextLevel(next_level);
    }

    public void FailLevel() {
        MenuManager.inst.PlayerDefeatPopup();
    }

    private void CheckSequentialLevelConditions() {
        for (int i = sequential_conditions_index; i < sequential_level_conditions.Count; i++) {
            Debug.Log($"i = {i}, sequential_conditions_index {sequential_conditions_index}");
            ILevelCondition current_condition = sequential_level_conditions[i];
            if (current_condition.was_triggered) { 
                Debug.LogWarning($"current condition was already triggered, skip."); // TODO --- remove debug
                continue; 
            } // skip already triggered conditions
            else if (!current_condition.ConditionMet()) {
                // current condition is NOT met, so we stop iterating. 
                Debug.LogWarning($"condition NOT met, break!"); // TODO --- remove debug
                break;
            } else {
                // current condition was met, trigger it's effects, mark as done, and continue to next condition
                Debug.LogWarning($"condition met, trigger!"); // TODO --- remove debug
                current_condition.TriggerEffects(); // sets `was_triggered = true`
                sequential_conditions_index = i + 1;
            }
        }
    }
    
    private bool CheckFailLevelConditions() {
        /* checks all the level fail conditions, if any is true, returns true and evaluates that conditions triggers.
         */
        for (int i = 0; i < fail_level_conditions.Count; i++) {
            ILevelCondition current_condition = fail_level_conditions[i];
            if (current_condition.was_triggered) { 
                Debug.LogWarning($"current condition was already triggered, skip."); // TODO --- remove debug
                continue; 
            } // skip already triggered conditions
            else if (!current_condition.ConditionMet()) {
                // current condition is NOT met, so we stop iterating. 
                Debug.LogWarning($"condition NOT met, break!"); // TODO --- remove debug
                continue;
            } else {
                // current condition was met, trigger it's effects, mark as done, and continue to next condition
                Debug.LogWarning($"condition met, trigger!"); // TODO --- remove debug
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
