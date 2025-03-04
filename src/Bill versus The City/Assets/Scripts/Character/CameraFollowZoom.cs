using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowZoom : MonoBehaviour
{
    public float un_zoomed_height = 15f;
    public float zoomed_height = 25f;
    public float slerp = 0.7f;
    [Tooltip("percentage between the mouse and player the camera is positioned. 1 follows just the mouse, 0 follows just the player")]
    public float follow_mouse_percent = 0.5f;
    public float max_vision_range = 6f;
    public const float ZOOM_DISTANCE_CONSTANT = 25f;
    public Transform target;
    public AttackController attack_controller;

    private Vector3 last_mouse_position = new Vector3(0f, 0f, 0f);
    
    private float desired_camera_height {
        get {
            // returns the desired height of the camera
            return un_zoomed_height;
        }
    }

    private void UpdatePosition() {
        // follow_mouse_percent
        if (MenuManager.inst.paused) { return; } // freeze camera while paused
        Vector3 horizontal_position = (target.position * (1 - follow_mouse_percent)) + (mouse_position * follow_mouse_percent);
        horizontal_position = new Vector3(horizontal_position.x, target.position.y, horizontal_position.z);
        if (Vector3.Distance(horizontal_position, target.position) > max_vision_range)  {
            // cap the distance of the camera at max_vision_range
            Vector3 direction = (horizontal_position - target.position).normalized;
            horizontal_position = target.position + (direction * max_vision_range);
        }
        
        Vector3 actual_position = new Vector3(horizontal_position.x, desired_camera_height, horizontal_position.z);
        transform.position = actual_position;
    }

    public static float FlatDistance(Vector3 first, Vector3 second) {
        // negates the Y-axis component before finding the distance between two points
        first = new Vector3(first.x, 0f, first.z);
        second = new Vector3(second.x, 0f, second.z);
        return Vector3.Distance(first, second);
    }

    // private float ShiftToMousePercent() {
    //     // returns a percent (float 0-1) determining how shifted towards the mouse the camera follow should be
    //     // 0 follows the target, 1 follows the mouse, and 0.5 follows the exact midpoint between        
    //     if (current_weapon == null) {
    //         return 0f;
    //     }
    //     return attack_controller.aim_percent * current_weapon.aim_zoom;
    // }

    // private IWeapon current_weapon {
    //     get {
    //         if (attack_controller == null) {
    //             Debug.LogWarning("no attack controller!");
    //             return null;
    //         }
    //         return attack_controller.current_weapon;
    //     }
    // }

    // public float max_zoom_range {
    //     get {
    //         float zoom_range;
    //         if (current_weapon == null) {
    //             zoom_range = ZOOM_DISTANCE_CONSTANT;
    //         }
    //         else {
    //             zoom_range = current_weapon.max_zoom_range;;
    //         }
    //         return zoom_range;
    //     }
    // }

    private Vector3 mouse_position {
        get {
            Vector3 mouse_pos = InputSystem.current.MouseWorldPosition();
            if (! InputSystem.IsNullPoint(mouse_pos)) {
                last_mouse_position = mouse_pos;
            }
            return last_mouse_position;
        }
    }


    void Start()
    {
        transform.position = new Vector3(target.position.x, desired_camera_height, target.position.z);
        last_mouse_position = target.position;
        UpdatePosition();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        last_mouse_position = mouse_position;
        SetDebug();
    }

    public Vector3 debug_last_mouse_pos;
    private void SetDebug() {
        debug_last_mouse_pos = last_mouse_position;
    }
}
