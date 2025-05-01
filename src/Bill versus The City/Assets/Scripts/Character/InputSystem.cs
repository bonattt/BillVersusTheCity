using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputSystem : ISettingsObserver
{
    public const string MOVE_X = "Horizontal";
    public const string MOVE_Y = "Vertical";
    public const string ATTACK_INPUT = "Fire1";
    public const string AIM_ATTACK = "Fire2";
    public const string SPRINT_INPUT = "Sprint";
    public const KeyCode PAUSE_MENU_INPUT = KeyCode.Escape;
    public const KeyCode MENU_NEXT = KeyCode.Mouse0;
    public const KeyCode CANCEL_MENU = KeyCode.Escape;
    public const KeyCode INTERACT = KeyCode.E;
    public const KeyCode RELOAD = KeyCode.R;
    public const KeyCode CROUCH = KeyCode.Space;
    public const KeyCode CROUCH_SECONDARY = KeyCode.LeftControl;
    public const KeyCode INVENTORY_MENU = KeyCode.I;
    public const KeyCode DEBUG_KEY = KeyCode.BackQuote;
    public const KeyCode DEBUG2_KEY = KeyCode.Mouse3;
    public const KeyCode NEXT_WEAPON_MODE = KeyCode.X;
    public static readonly Vector3 NULL_POINT = new Vector3(float.NaN, float.NaN, float.NaN);

    private InputSystem() {
        // if (_current == null) {
        //     _current = this;
        // }
        if (_current != null) {
            GameSettings.inst.game_play_settings.Unsubscribe(_current);
        }
        GameSettings.inst.game_play_settings.Subscribe(this);
        _current = this;

    }

    private static InputSystem _current = null;
    public static InputSystem current {
        get {
            if (_current == null) {
                new InputSystem();
            }
            return _current;
        }
    }

    public float mouse_sensitivity {
        get { return base_mouse_sensitivity * mouse_sensitivity_percent; }
    }

    private float _base_mouse_sensitivity = 1f;
    public float base_mouse_sensitivity {
        get { return _base_mouse_sensitivity; }
        set { 
            _base_mouse_sensitivity = value; 
        }
    }

    private float _mouse_sensitivity_percent = 1f;
    public float mouse_sensitivity_percent {
        get { return _mouse_sensitivity_percent; }
        set { 
            if (value <= 0) {
                _mouse_sensitivity_percent = 0f; 
            } else if (value >= 1) {
                _mouse_sensitivity_percent = 1f; 
            } else {
                _mouse_sensitivity_percent = value; 
            }
        }
    }
     
    public bool CrouchInput() {
        return Input.GetKey(CROUCH) || Input.GetKey(CROUCH_SECONDARY);
    }

    public GameObject GetHoveredObject() {
        return GetHoveredObject(LayerMask.GetMask("Interaction"));
    }

    public GameObject GetHoveredObject(LayerMask mask) {
        // gets a GameObject the mouse is hovering over.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);;
        RaycastHit hit;
        // int layer_mask = LayerMask.GetMask(TACTICAL_UI_LAYER);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
            return hit.collider.gameObject;
        }
        return null;
    }

    public Vector3 MouseScreenPosition() {
        return new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            -Camera.main.transform.position.z
        );
    }

    public Vector3 MouseWorldPosition() {
        LayerMask mouse_coordinates = LayerMask.GetMask("MouseCoordinates");
        return MouseWorldPosition(mouse_coordinates);
    }
    public Vector3 MouseWorldPosition(LayerMask mask) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // int layer_mask = LayerMask.GetMask(TACTICAL_UI_LAYER);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) { // }, layer_mask)) {
            return hit.point;
        }
        else {
            // Debug.LogError("Cannot find Mouse Position REEEEEEE");
            return NULL_POINT;
        }
    }

    public static bool IsNullPoint(Vector3 point) {
        return float.IsNaN(point.x) || float.IsNaN(point.y) || float.IsNaN(point.z);
    }

    public bool AttackClickInput() {
        // TODO --- implement more control mapping
        // StaticLogger.Warning("AttackClickInput has a placeholder implementation");
        return Input.GetKeyDown(KeyCode.Mouse0);
    }
    
    public bool AttackHoldInput() {
        return Input.GetAxis(ATTACK_INPUT) != 0;
    }

    public bool AimHoldInput() {
        return Input.GetAxis(AIM_ATTACK) != 0;
    }

    public bool SprintInput() {
        return Input.GetAxis(SPRINT_INPUT) != 0;
    }

    public bool DashInput() {
        return Input.GetKeyDown(CROUCH);
    }

    public float MoveXInput() {
        return Input.GetAxis(MOVE_X);
    }

    public bool MoveLeftInputHold() {
        return MoveXInput() < 0;
    }

    public bool MoveRightInputHold() {
        return MoveXInput() > 0;
    }

    public float MoveYInput() {
        return Input.GetAxis(MOVE_Y);
    }

    public bool MoveUpInputHold() {
        return MoveYInput() > 0;
    }

    public bool MoveDownInputHold() {
        return MoveYInput() < 0;
    }

    public bool InteractInput() {
        return Input.GetKeyDown(INTERACT);
    }

    public bool ReloadInput() {
        return Input.GetKeyDown(RELOAD);
    }

    public bool DebugInput() {
        // input key used purely for testing functionality
        // Debug.Log("TestInput: " + Input.GetKeyDown(DEBUG_KEY));
        return Input.GetKeyDown(DEBUG_KEY);
    }

    public bool MenuCancelInput() {
        return Input.GetKeyDown(CANCEL_MENU);
    }

    public bool PauseMenuInput() {
        return Input.GetKeyDown(PAUSE_MENU_INPUT);
    }

    public bool MenuNextInput() {
        // input for advancing dialouge

        return Input.GetKeyDown(MENU_NEXT);
    }

    public int? WeaponSlotInput() {
        // returns a nullable int, containing the weapon slot input for this frame.
        // if no weapon-slot keys are pressed, return null.
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) {
            return 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) {
            return 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) {
            return 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) {
            return 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) {
            return 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) {
            return 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) {
            return 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) {
            return 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) {
            return 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) {
            return 9;
        }
        return null;
    }

    public bool WeaponSlotKeyDown(int slot_number) {
        List<KeyCode> key_query = new List<KeyCode>();
        switch (slot_number) {
            case 0:
                key_query.Add(KeyCode.Keypad1);
                break;

            case 1:
                key_query.Add(KeyCode.Keypad2);
                break;
            
            case 2:
                key_query.Add(KeyCode.Keypad3);
                break;
            
            case 3:
                key_query.Add(KeyCode.Keypad4);
                break;
            
            case 4:
                key_query.Add(KeyCode.Keypad5);
                break;
            
            case 5:
                key_query.Add(KeyCode.Keypad6);
                break;
            
            case 6:
                key_query.Add(KeyCode.Keypad7);
                break;
            
            case 7:
                key_query.Add(KeyCode.Keypad8);
                break;
            
            case 8:
                key_query.Add(KeyCode.Keypad9);
                break;
            
            case 9:
                key_query.Add(KeyCode.Keypad0);
                break;

            default:
                return false;
        }
        foreach(KeyCode code in key_query) {
            if (Input.GetKeyDown(code)) {
                return true;
            }
        }
        return false;
    }

    public float GetScroll() {
        return Input.GetAxis("Mouse ScrollWheel");
    }

    public bool NextWeaponInput() {
        return GetScroll() > 0;
    }

    public bool PreviousWeaponInput() {
        return GetScroll() < 0;
    }

    public bool NextWeaponModeInput() {
        return Input.GetKeyDown(NEXT_WEAPON_MODE);
    }

    public void SettingsUpdated(ISettingsModule updated, string field) {
        switch(updated) {
            case GamePlaySettings gameplay_settings:
                GamePlaySettingsUpdated(gameplay_settings, field);
                break;  

            default:
                // do nothing, no updates needed
                break;
        }
    }

    private void GamePlaySettingsUpdated(GamePlaySettings settings, string field) {
        switch (field) {
            case GamePlaySettings.MOUSE_SENSITIVITY:
                base_mouse_sensitivity = settings.mouse_sensitivity;
                break;

            default:
                // do nothing
                break;
        }
    }

    public string InputTypeDisplay(InputType input_type) {
        // takes an input type and returns a string display value for whatever control that input type is currently mapped to
        switch (input_type) {
            case InputType.fire_weapon:
                Debug.LogWarning($"input text display for '{input_type}' does not yet support key-bindings!");
                return "LMB";
            case InputType.aim_weapon:
                Debug.LogWarning($"input text display for '{input_type}' does not yet support key-bindings!");
                return "RMB";
            case InputType.next_weapon:
                Debug.LogWarning($"input text display for '{input_type}' does not yet support key-bindings!");
                return "Scroll Wheel Up";
            case InputType.previous_weapon:
                Debug.LogWarning($"input text display for '{input_type}' does not yet support key-bindings!");
                return "Scroll Wheel down";
            case InputType.select_weapon:
                Debug.LogWarning($"input text display for '{input_type}' does not yet support key-bindings!");
                return "[Number Key]";
            case InputType.move_up:
                Debug.LogWarning($"input text display for '{input_type}' does not yet support key-bindings!");
                return "W";
            case InputType.move_left:
                Debug.LogWarning($"input text display for '{input_type}' does not yet support key-bindings!");
                return "A";
            case InputType.move_down:
                Debug.LogWarning($"input text display for '{input_type}' does not yet support key-bindings!");
                return "S";
            case InputType.move_right:
                Debug.LogWarning($"input text display for '{input_type}' does not yet support key-bindings!");
                return "D";

            case InputType.crouch:
                Debug.LogWarning($"input text display for '{input_type}' does not yet support key-bindings!");
                return "space";

            case InputType.interact:
                return $"{INTERACT}";
            case InputType.reload:
                return $"{RELOAD}";
            case InputType.sprint:
                return $"{SPRINT_INPUT}";
            case InputType.menu_cancel:
                return $"{CANCEL_MENU}";
            case InputType.pause_menu:
                return $"{PAUSE_MENU_INPUT}";

            default:
                Debug.LogError($"unknown InputType: {input_type}");
                return "null";
        }
    }

    public bool GetGenericInput(InputType input_type) {
        // takes an InputType, and returns bool if that key is being held this frame
        switch (input_type) {
            case InputType.fire_weapon:
                return AttackHoldInput();
            case InputType.aim_weapon:
                return AimHoldInput();
            case InputType.next_weapon:
                return NextWeaponInput();
            case InputType.previous_weapon:
                return PreviousWeaponInput();
            case InputType.select_weapon:
                return WeaponSlotInput() != null; // if a number key is pressed, int? will be not null
            case InputType.move_up:
                return MoveUpInputHold();
            case InputType.move_left:
                return MoveLeftInputHold();
            case InputType.move_down:
                return MoveDownInputHold();
            case InputType.move_right:
                return MoveRightInputHold();
            case InputType.crouch:
                return CrouchInput();
            case InputType.interact:
                return InteractInput();
            case InputType.reload:
                return ReloadInput();
            case InputType.sprint:
                return SprintInput();
            case InputType.menu_cancel:
                return MenuCancelInput();
            case InputType.pause_menu:
                return PauseMenuInput();

            default:
                Debug.LogError($"unhandled InputType: {input_type}");
                return false;
        }
    }
}


public enum InputType {
    // enum for different kinds of player inputs, which may be mapped to any number of different actual inputs
    fire_weapon,
    aim_weapon,
    interact,
    reload,
    sprint,
    crouch,
    menu_cancel,  // cancels out of the current menu
    pause_menu,  // opens the pause menu while in normal gameplay
    next_weapon, // scroll up for next weapon
    previous_weapon, // scroll down for previous weapon
    select_weapon, // number key to select a weapon
    move_up, // W
    move_left, // A
    move_down, // S
    move_right, // D

}