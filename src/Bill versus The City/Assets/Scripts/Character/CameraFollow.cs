using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float x_offset = 0f;
    public float y_offset = 10f;
    public float z_offset = 0f; 
    public float slerp = 0.7f;
    public const float ZOOM_DISTANCE_CONSTANT = 25f;
    public Transform target;
    public AttackController attack_controller;

    private Vector3 last_mouse_position = new Vector3(0f, 0f, 0f);
    
    public Vector3 offset {
        get {
            return new Vector3(x_offset, y_offset, z_offset);
        }
    }

    private Vector3 FollowTargetPosition() {
        // returns the position from which the camera would directly follow the follow target (eg. the player)
        return target.position + this.offset;
    }

    private float desired_camera_height {
        get {
            // returns the desired height of the camera
            return this.target.position.y + this.offset.y;
        }
    }

    private void UpdatePosition() {
        Vector3 target_pos = FollowTargetPosition();
        float percent_mouse = ShiftToMousePercent();
        Vector3 mid_point = (target_pos * (1 - percent_mouse)) + (mouse_position * percent_mouse);
        // point `percent_mouse` between the mouse position and the target-follow
        //   possition, adjusted to the desired camera height
        mid_point = new Vector3(mid_point.x, desired_camera_height, mid_point.z);

        float distance_from_follow_target = FlatDistance(target_pos, mid_point);
        Vector3 camera_destination;
        if (distance_from_follow_target > max_zoom_range) {
            Vector3 vector_to = (mid_point - transform.position).normalized * max_zoom_range;
            camera_destination = target_pos + vector_to + new Vector3(0f, zoom_out_distance, 0f);
        } else {
            camera_destination = mid_point + new Vector3(0f, zoom_out_distance, 0f);
        }
        
        transform.position = Vector3.Slerp(transform.position, camera_destination, this.slerp * Time.deltaTime);
    }

    public static float FlatDistance(Vector3 first, Vector3 second) {
        // negates the Y-axis component before finding the distance between two points
        first = new Vector3(first.x, 0f, first.z);
        second = new Vector3(second.x, 0f, second.z);
        return Vector3.Distance(first, second);
    }

    private float ShiftToMousePercent() {
        // returns a percent (float 0-1) determining how shifted towards the mouse the camera follow should be
        // 0 follows the target, 1 follows the mouse, and 0.5 follows the exact midpoint between
        
        if (current_weapon == null) {
            Debug.LogWarning("no weapon");
            return 0f;
        }

        return attack_controller.aim_percent * current_weapon.aim_zoom;
    }

    private IWeapon current_weapon {
        get {
            if (attack_controller == null) {
                Debug.LogWarning("no attack controller!");
                return null;
            }
            return attack_controller.current_weapon;
        }
    }

    public float max_zoom_range {
        get {
            float zoom_range;
            if (current_weapon == null) {
                zoom_range = ZOOM_DISTANCE_CONSTANT;
            }
            else {
                zoom_range = current_weapon.max_zoom_range;;
            }
            return zoom_range;
        }
    }

    public const float ZOOM_OUT_CONSTANT = 10f;
    public float zoom_out_distance {
        //  height to raise the camera to zoom out when aiming.
        get {
            // return 0f;
            return ShiftToMousePercent() * ZOOM_OUT_CONSTANT;
        }
    }

    private Vector3 mouse_position {
        get {
            Vector3 mouse_pos = InputSystem.current.MouseWorldPosition();
            if (! InputSystem.IsNullPoint(mouse_pos)) {
                last_mouse_position = mouse_pos;
            }
            return last_mouse_position + this.offset;
        }
    }

    void Start()
    {
        UpdatePosition();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        SetDebug();
    }

    public float debug_shift_to_mouse = 0f;
    public float debug_zoom_out_distance = 0f;
    public Vector3 debug_last_mouse_pos;
    private void SetDebug() {
        debug_shift_to_mouse = ShiftToMousePercent();
        debug_zoom_out_distance = zoom_out_distance;
        debug_last_mouse_pos = last_mouse_position;
    }
}
