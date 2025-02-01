using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LevelConfig : MonoBehaviour
{

    public bool combat_enabled = true;
    public bool weapon_select_on_start = true;

    public static LevelConfig inst = null;

    public bool use_starting_weapons = false;

    public ScriptableObject init_starting_rifle, init_starting_handgun, init_starting_pickup;
    private IWeapon starting_rifle, starting_handgun, starting_pickup;

    public List<MonoBehaviour> init_level_conditions;
    private List<ILevelCondition> level_conditions = new List<ILevelCondition>();

    void Awake() {
        inst = this;
    }

    void Start()
    {
        InitLevelConditions();
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
        CheckLevelConditionsSequentially();
    }


    private void CheckLevelConditionsSequentially() {
        for (int i = 0; i < level_conditions.Count; i++) {
            ILevelCondition current_condition = level_conditions[i];
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
            }
        }

    }

    private void InitLevelConditions() {
        for(int i = 0; i < init_level_conditions.Count; i++) {
            MonoBehaviour b = init_level_conditions[i];
            try {
                ILevelCondition c = (ILevelCondition) b;
                c.was_triggered = false;
                level_conditions.Add(c);
            } catch (InvalidCastException) {
                Debug.LogWarning($"script {b} cannot be cast to ILevelCondition");
            }
        }
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
