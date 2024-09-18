using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float x_offset = 0f;
    public float y_offset = 10f;
    public float z_offset = 0f; 
    public float slerp = 0.7f;
    public Transform target;
    public AttackController attack_controller;

    private Vector3 last_mouse_position = new Vector3(0f, 0f, 0f);
    
    public Vector3 offset {
        get {
            return new Vector3(x_offset, y_offset, z_offset);
        }
    }

    private Vector3 FollowTargetPosition() {
        return target.position + this.offset;
    }

    private void UpdatePosition() {
        Vector3 target_pos = FollowTargetPosition();
        float percent_mouse = ShiftToMousePercent();
        Vector3 mid_point = (target_pos * (1 - percent_mouse)) + (mouse_position * percent_mouse);
        mid_point = new Vector3(mid_point.x, y_offset, mid_point.z);
        transform.position = Vector3.Slerp(transform.position, mid_point, this.slerp * Time.deltaTime);
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

    private Vector3 mouse_position {
        get {
            Vector3 mouse_pos = InputSystem.current.MouseWorldPosition();
            if (mouse_pos != InputSystem.NULL_POINT) {
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
    private void SetDebug() {
        debug_shift_to_mouse = ShiftToMousePercent();
    }
}
