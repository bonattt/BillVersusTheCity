using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimLineController : MonoBehaviour
{
    public float line_width = 0.05f;
    public float line_length = 25f;
    public float y_offset = 0.05f;
    

    private LineRenderer lineRenderer;

    public Color base_color = Color.red;
    public PlayerAttackController attack_controller;

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

        Vector3 start_pos = PlayerCharacter.inst.player_object.transform.position;
        Vector3 base_end_pos = new Vector3(x, y_offset, z);
        
        // get a unit vector in the direction of the mouse position
        Vector3 unit_vector = (base_end_pos - start_pos).normalized;

        // extend the unit vector from the mouse to `line_length`
        Vector3 direction = unit_vector * line_length;
        
        Vector3 end_pos;
        RaycastHit hit;
        // if there is a wall in the way, don't draw aim-lines past it
        if (Physics.Raycast(start_pos, direction, out hit, direction.magnitude, layer_mask)) {
            end_pos = hit.point;
        } 
        else {
            end_pos = direction + start_pos;
        }

        // int n = 10; // iterations of adding the line to itself to extend the line past the mouse
        // end_pos = end_pos + (n * (end_pos - start_pos)); // extend the line to double, so it reaches past the mouse;
        lineRenderer.SetPosition(0, start_pos); // Start
        lineRenderer.SetPosition(1, end_pos); // End
    }
    
    public Color GetColor() {
        // gets the color for this frame, including an alpha value based on current player aim.
        float alpha = attack_controller.aim_percent;
        return new Color(base_color.r, base_color.g, base_color.b, alpha);
    }

    void Update()
    {
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
