using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystemDebugger : MonoBehaviour
{    
    public Vector3 mouse_screen_position;
    public Vector3 mouse_world_position;
    public bool attack_click_input;
    public bool attack_hold_input; 
    public bool aim_attack_input; 
    public bool sprint_input; 
    public bool dash_input; 
    public float move_x_input; 
    public float move_y_input; 
    public bool interact_input; 
    public bool reload_input; 
    public bool test_input; 
    public bool test_input2;
    public bool menu_cancel_input;
    public bool pause_menu_input;
    public int? weapon_slot_input; 
    public bool next_weapon_input;
    public bool previous_weapon_input;

    void Start() {
        UpdateDebugFields();
    }

    void Update() {
        UpdateDebugFields();
    }

    private void UpdateDebugFields() {
        mouse_screen_position = InputSystem.current.MouseScreenPosition();
        mouse_world_position = InputSystem.current.MouseWorldPosition();
        attack_click_input = InputSystem.current.AttackClickInput();
        attack_hold_input = InputSystem.current.AttackHoldInput();
        aim_attack_input = InputSystem.current.AimAttackInput();
        sprint_input = InputSystem.current.SprintInput();
        dash_input = InputSystem.current.DashInput();
        move_x_input = InputSystem.current.MoveXInput();
        move_y_input = InputSystem.current.MoveYInput();
        interact_input = InputSystem.current.InteractInput();
        reload_input = InputSystem.current.ReloadInput();
        test_input = InputSystem.current.DebugInput();
        menu_cancel_input = InputSystem.current.MenuCancelInput();
        pause_menu_input = InputSystem.current.PauseMenuInput();
        weapon_slot_input = InputSystem.current.WeaponSlotInput();
        next_weapon_input = InputSystem.current.NextWeaponInput();
        previous_weapon_input = InputSystem.current.PreviousWeaponInput();
    }
}
