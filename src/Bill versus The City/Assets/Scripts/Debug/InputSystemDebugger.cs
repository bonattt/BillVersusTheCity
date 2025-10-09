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
    public float debug_mouse_wheel_axis; 

    void Start() {
        UpdateDebugFields();
    }

    void Update() {
        UpdateDebugFields();
    }

    private float show_attack_input_for = 0.05f;
    private float debug__attack_at = -1f;
    private void UpdateDebugFields() {

        if (InputSystem.inst.AttackClickInput()) {
            debug__attack_at = Time.time;
            attack_click_input = true;
        } else if (debug__attack_at + show_attack_input_for >= Time.time) {
            attack_click_input = true;
        } else {
            attack_click_input = false;
        }
        // attack_click_input = InputSystem.current.AttackClickInput();
        attack_hold_input = InputSystem.inst.AttackHoldInput();

        mouse_screen_position = InputSystem.inst.MouseScreenPosition();
        mouse_world_position = InputSystem.inst.MouseWorldPosition();
        aim_attack_input = InputSystem.inst.AimHoldInput();
        sprint_input = InputSystem.inst.SprintInput();
        dash_input = InputSystem.inst.DashInput();
        move_x_input = InputSystem.inst.MoveXInput();
        move_y_input = InputSystem.inst.MoveYInput();
        interact_input = InputSystem.inst.InteractInput();
        reload_input = InputSystem.inst.ReloadInput();
        test_input = InputSystem.inst.DebugInput();
        menu_cancel_input = InputSystem.inst.MenuCancelInput();
        pause_menu_input = InputSystem.inst.PauseMenuInput();
        weapon_slot_input = InputSystem.inst.SetWeaponSlotInput();
        next_weapon_input = InputSystem.inst.NextWeaponInput();
        previous_weapon_input = InputSystem.inst.PreviousWeaponInput();
        debug_mouse_wheel_axis = InputSystem.inst.GetScroll();
    }
}
