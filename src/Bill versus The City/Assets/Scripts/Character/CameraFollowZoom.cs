using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////// NOTES ON ENEMY VISIBILITY RANGE //////////////
// fully extended vision [left/right]: 25
// centered vision [left/right]: 19
// fully retracted vision [left/right]: 13
//
// fully extended vision [up/down]: 17
// centered vision [up/down]: 11
// fully retracted vision [up/down]: 5

public class CameraFollowZoom : MonoBehaviour
{
    public const CameraFollowMode DEFAULT_FOLLOW_MODE = CameraFollowMode.player_and_mouse;
    [Tooltip("setting that overrides the normal camera follow mode, iff `override_camera_follow` is set to true")]
    public CameraFollowMode camera_follow_override;
    public bool override_camera_follow;

    private Vector2 _camera_offset;
    public Vector3 camera_offset {
        get => _camera_offset;
        set { _camera_offset = value; }
    }

    public CameraFollowMode camera_follow_mode {
        get {
            if (override_camera_follow) {
                return camera_follow_override;
            }
            return DEFAULT_FOLLOW_MODE;
        }
    }

    public float un_zoomed_height = 15f;
    public float zoomed_height = 25f;
    public float slerp = 0.85f;
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
        if (MenuManager.inst.paused) { return; } // freeze camera while paused
        switch (camera_follow_mode) {
            case CameraFollowMode.player_and_mouse:
                UpdatePositionMouseAndPlayer();
                break;
            case CameraFollowMode.target:
                UpdatePositionPlayer();
                break;

            case CameraFollowMode.stationary | CameraFollowMode.choreography:
                // do nothing
                break;

            default:
                Debug.LogError($"unhandled CameraFollowMode: {camera_follow_mode}, override? {camera_follow_override}");
                break;
        } 
    }

    private void UpdatePositionMouseAndPlayer() {
        // follow_mouse_percent
        Vector3 horizontal_position = (target.position * (1 - follow_mouse_percent)) + (mouse_position * follow_mouse_percent);
        horizontal_position = new Vector3(horizontal_position.x, target.position.y, horizontal_position.z);
        if (Vector3.Distance(horizontal_position, target.position) > max_vision_range)  {
            // cap the distance of the camera at max_vision_range
            Vector3 direction = (horizontal_position - target.position).normalized;
            horizontal_position = target.position + (direction * max_vision_range);
        }
        UpdateCameraPosition(horizontal_position);
    }

    private void UpdatePositionPlayer() {
        UpdateCameraPosition(target.position);
    }

    public void UpdateCameraPosition(Vector3 new_position) {
        new_position = AdjustCameraHeight(new_position);
        new_position += camera_offset;
        transform.position = Vector3.Slerp(transform.position, new_position, slerp);
    }

    private Vector3 AdjustCameraHeight(Vector3 horizontal_position) {
        // returns the given vector with it's height set to `desired_camera_height`
        return new Vector3(horizontal_position.x, desired_camera_height, horizontal_position.z);
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


public enum CameraFollowMode {
    player_and_mouse,
    target,
    stationary,
    choreography, // camera will be moved by choreography
}