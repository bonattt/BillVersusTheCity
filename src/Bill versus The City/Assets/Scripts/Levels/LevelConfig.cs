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

    void Awake() {
        inst = this;
    }

    void Start()
    {
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
