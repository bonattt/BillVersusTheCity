using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputSystem
{
    public const string MOVE_X = "Horizontal";
    public const string MOVE_Y = "Vertical";
    public const string LIGHT_ATTACK = "Fire1";
    public const string READY_ATTACK = "Fire2";
    public const string SPRINT_INPUT = "Shift";
    public const KeyCode PAUSE_MENU_INPUT = KeyCode.Escape;
    public const KeyCode CANCEL_MENU = KeyCode.Escape;
    public const KeyCode INTERACT = KeyCode.E;
    public const KeyCode DASH = KeyCode.Space;
    public const KeyCode INVENTORY_MENU = KeyCode.I;
    public const KeyCode DEBUG_KEY = KeyCode.BackQuote;
    public const KeyCode DEBUG2_KEY = KeyCode.Mouse3;
    public const KeyCode SWITCH_WEAPON = KeyCode.Alpha1;
    public static readonly Vector3 NULL_POINT = new Vector3(float.NaN, float.NaN, float.NaN);

    private InputSystem() {
        // if (_current == null) {
        //     _current = this;
        // }
    }

    private static InputSystem _current = null;
    public static InputSystem current {
        get {
            if (_current == null) {
                _current = new InputSystem();
            }
            return _current;
        }
    }

    public Vector3 MouseScreenPosition() {
        return new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            -Camera.main.transform.position.z
        );
    }

    public Vector3 MouseWorldPosition() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);;
        RaycastHit hit;
        // int layer_mask = LayerMask.GetMask(TACTICAL_UI_LAYER);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) { // }, layer_mask)) {
            return hit.point;
        }
        else {
            Debug.LogError("Cannot find Mouse Position REEEEEEE");
            return NULL_POINT;
        }
    }

    public bool LightAttackInput() {
        return Input.GetAxis(LIGHT_ATTACK) != 0;
    }

    public bool ReadyAttackInput() {
        return Input.GetAxis(READY_ATTACK) != 0;
    }

    public bool SprintInput() {
        return Input.GetAxis(SPRINT_INPUT) != 0;
    }

    public bool DashInput() {
        return Input.GetKeyDown(DASH);
    }

    public float MoveXInput() {
        return Input.GetAxis(MOVE_X);
    }

    public float MoveYInput() {
        return Input.GetAxis(MOVE_Y);
    }

    public bool InteractInput() {
        return Input.GetKeyDown(INTERACT);
    }

    public bool TestInput() {
        // input key used purely for testing functionality
        // Debug.Log("TestInput: " + Input.GetKeyDown(DEBUG_KEY));
        return Input.GetKeyDown(DEBUG_KEY);
    }

    public bool TestInput2() {
        // input key used purely for testing functionality
        Debug.Log("TestInput2: " + Input.GetKeyDown(DEBUG2_KEY));
        return Input.GetKeyDown(DEBUG2_KEY);
    }

    public bool MenuCancelInput() {
        return Input.GetKeyDown(CANCEL_MENU);
    }

    public bool PauseMenuInput() {
        return Input.GetKeyDown(PAUSE_MENU_INPUT);
    }
}
