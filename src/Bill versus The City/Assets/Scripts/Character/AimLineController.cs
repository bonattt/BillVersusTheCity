using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimLineController : MonoBehaviour
{
    public float line_width = 0.05f;
    public float line_length = 25f;
    // public float y_pos = 0.05f;
    public Transform start_at;
    

    private LineRenderer lineRenderer;

    public Color base_color = Color.red;
    public PlayerAttackController attack_controller;
    public PlayerMovement player_movement;

    public LayerMask layer_mask;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // Two points for a line
        lineRenderer.startWidth = line_width;
        lineRenderer.endWidth = line_width;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Set the line's start and end points
        DrawLineTo(0, 0);
    }

    private void DrawLineTo(float x, float z) {
        // player character not initialized this frame, ignore
        if (PlayerCharacter.inst.player_object == null) { return; } 

        Vector3 start_pos = start_at.position;  // PlayerCharacter.inst.player_object.transform.position;
        // start_pos = new Vector3(start_pos.x, y_pos, start_pos.z);
        float y_pos = start_pos.y;
        Vector3 base_end_pos = new Vector3(x, y_pos, z);
        
        // get a unit vector in the direction of the mouse position
        Vector3 unit_vector = (base_end_pos - start_pos).normalized;

        // extend the unit vector from the mouse to `line_length`
        Vector3 direction = unit_vector * line_length;
        
        Vector3 end_pos;
        RaycastHit hit;
        // if there is a wall in the way, don't draw aim-lines past it
        if (Physics.Raycast(start_pos, unit_vector, out hit, float.PositiveInfinity, layer_mask)) {
            end_pos = hit.point;
        } 
        else {
            end_pos = direction + start_pos;
        }
        lineRenderer.SetPosition(0, start_pos); // Start
        lineRenderer.SetPosition(1, end_pos); // End
    }
    
    public Color GetColor() {
        // gets the color for this frame, including an alpha value based on current player aim.
        float alpha = attack_controller.aim_percent;
        return new Color(base_color.r, base_color.g, base_color.b, alpha);
    }

    public Color GetBlankColor() {
        // gets a transparent color
        return new Color(base_color.r, base_color.g, base_color.b, 0);
    }

    void Update()
    {
        if (!player_movement.is_alive) {
            // if the player is dead, erase the line
            lineRenderer.material.color = GetBlankColor();
            return;
        }
        else if (! player_movement.is_active) {
            return; // do nothing when the game is pasued
        }
        Color new_color = GetColor();
        if (lineRenderer.material.color.a == 0 && new_color.a == 0) {
            // optimization: if the line is not shown, and will still not be shown this frame, do not draw it
            return;
        }

        Vector3 mouse_pos = InputSystem.current.MouseWorldPosition();
        DrawLineTo(mouse_pos.x, mouse_pos.z);
        lineRenderer.material.color = new_color;
    }
}
