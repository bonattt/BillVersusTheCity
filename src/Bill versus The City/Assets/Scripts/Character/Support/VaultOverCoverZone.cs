using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultOverCoverZone : MonoBehaviour {
    // creates a zone around cover which a player character can jump over

    [Tooltip("For convenience, draw `target_collider` even while it's object is not selected.")]
    [SerializeField]
    private bool draw_collider = true;

    [Tooltip("How far a player needs to jump to get over this obstacle.")]
    public float jump_length = 1f;
    
    [Tooltip("How high a player needs to jump to get over this obstacle.")]
    public float jump_height = 0.75f;
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

        if (!draw_collider) { return; }
        // draw the BoxCollider's gizmo, even if it's not selected
        Matrix4x4 old_matrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 box_size = Vector3.Scale(target_collider.size, transform.lossyScale);
        Vector3 box_center = target_collider.center; // NOTE: do not add transform.position because we are using `transform.localToWorldMatrix`
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(box_center, box_size);
        Gizmos.matrix = old_matrix;
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

    public Vector3 GetVaultDirection(Vector3 move_direction) {
        // takes a move direction, determines if the move is in the direction of a jump, or the opposite direction, and returns the actual move a player should make.
        float direction;
        if (Vector3.Dot(move_direction, jump_direction) > 0) {
            direction = 1f;
        } else {
            direction = -1f;
        }
        return direction * jump_direction;
    }

    public bool PositionInZone(Vector3 position) {
        return target_collider.bounds.Contains(position);
    }

    public bool ValidJumpAngle(Vector3 move_direction) {
        return Mathf.Abs(Vector3.Dot(move_direction, jump_direction)) < angle_threshold;
    }
}
