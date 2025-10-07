using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultOverCoverZone : MonoBehaviour {
    // creates a zone around cover which a player character can jump over

    [Tooltip("this setting configures how far a player needs to jump to get over this obstacle.")]
    public float jump_length = 1f;
    public Vector3 jump_direction = Vector3.forward;
    // public Vector3 jump_area_position = new Vector3(0f, 0f, 0f);
    // public Vector3 jump_area_size = new Vector3(0.85f, 0.65f, 5f);

    [Tooltip("Box Collider that defines the area a player will be able to jump from.")]
    [SerializeField]
    private BoxCollider target_collider;
    public Color gizmo_color = Color.yellow;

    [Tooltip("The absolute value of the dot-product between player-direction and jump-direction must be greater than this for a jump to trigger.")]
    public float angle_threshold = 0.75f;

    void Start() {
        if (jump_direction.y != 0) { Debug.LogWarning($"{gameObject.name}.jump_direction {jump_direction} contains a non-zero Y component!"); }
    }

    void OnDrawGizmos() {
        if (gizmo_color == Color.clear) { return; }

        Vector3 center = transform.position;
        // Gizmos.color = gizmo_color;
        // Gizmos.DrawWireCube(center, jump_area_size);

        Gizmos.color = gizmo_color;
        Vector3 start = center - jump_direction;
        Vector3 end = center + jump_direction;
        Gizmos.DrawLine(start, end);

        // Draws additional lines to the side of the original side, assuming the Jump Vector has no Y component (the game is top-down, so this is a safe assumption)
        Vector3 perpendicular = new Vector3(Mathf.Abs(jump_direction.z), 0f, Mathf.Abs(jump_direction.x)).normalized;
        Gizmos.DrawLine(start - perpendicular, end - perpendicular);
        Gizmos.DrawLine(start + perpendicular, end + perpendicular);

    }

    public bool IsInJumpPosition(Vector3 player_position, Vector3 move_direction) {
        // check if player is moving in the right direction
        if (ValidJumpAngle(move_direction)) { return false; }
        // check if the player is in the zone
        if (PositionInZone(player_position)) {
            return true;
        }
        return false;
    }

    public bool PositionInZone(Vector3 position) {
        return target_collider.bounds.Contains(position);
    }

    public bool ValidJumpAngle(Vector3 move_direction) {
        return Mathf.Abs(Vector3.Dot(move_direction, jump_direction)) < angle_threshold;
    }
}
