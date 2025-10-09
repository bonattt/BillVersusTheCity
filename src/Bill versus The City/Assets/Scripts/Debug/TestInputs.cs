using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInputs : MonoBehaviour
{
    
    public bool pause_menu_input = false;
    public bool menu_next = false;
    public bool cancel_menu = false;
    public bool interact = false;
    public bool reload = false;
    public bool crouch = false;
    public bool crouch_secondary = false;
    public bool inventory_menu = false;
    public bool debug_key = false;
    public bool debug2_key = false;
    public bool next_weapon_mode = false;
    
    // Update is called once per frame
    void Update()
    {
        UpdateKeyDown();    
    }

    private void UpdateKeyDown() {
        // updates if the key is down in unity's vanilla input system (not by translation layer)
        pause_menu_input = Input.GetKey(InputSystem.PAUSE_MENU_INPUT);
        menu_next = Input.GetKey(InputSystem.MENU_NEXT);
        cancel_menu = Input.GetKey(InputSystem.CANCEL_MENU);
        interact = Input.GetKey(InputSystem.INTERACT);
        reload = Input.GetKey(InputSystem.RELOAD);
        crouch = Input.GetKey(InputSystem.DIVE);
        crouch_secondary = Input.GetKey(InputSystem.CROUCH);
        inventory_menu = Input.GetKey(InputSystem.INVENTORY_MENU);
        debug_key = Input.GetKey(InputSystem.DEBUG_KEY);
        next_weapon_mode = Input.GetKey(InputSystem.NEXT_WEAPON_MODE);
    }
}
