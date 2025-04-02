using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingMouseSensitivity : MonoBehaviour
{
    public float debug_sensitivity = 1.0f; // Sensitivity multiplier
    private Vector3 last_mouse_position; // Store last frame's mouse position

    public static AimingMouseSensitivity inst { get; private set; }
    void Awake() {
        inst = this;
    }

    void Start() {
        // Store the initial mouse position
        last_mouse_position = Input.mousePosition;
    }

    void Update() {
        float sensitivity = InputSystem.current.mouse_sensitivity;
        debug_sensitivity = sensitivity;
        
        // Get mouse movement delta since the last frame
        Vector3 mouse_delta = Input.mousePosition - last_mouse_position;
        
        // Scale the delta by sensitivity
        mouse_delta *= sensitivity;

        // Update the cursor position manually
        Vector3 new_cursor_pos = last_mouse_position + mouse_delta;

        // Clamp to ensure cursor stays within screen bounds
        new_cursor_pos.x = Mathf.Clamp(new_cursor_pos.x, 0, Screen.width);
        new_cursor_pos.y = Mathf.Clamp(new_cursor_pos.y, 0, Screen.height);

        // Apply new position
        Cursor.SetCursor(null, new_cursor_pos, CursorMode.Auto);

        // Update the stored mouse position for the next frame
        last_mouse_position = Input.mousePosition;
    }
}
