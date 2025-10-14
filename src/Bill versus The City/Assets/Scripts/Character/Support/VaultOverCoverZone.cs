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
    
    // private Vector3 _jump_direction;
    public Vector3 jump_direction {
        get {
            if (jump_direction_calculation == JumpDirectionSetting.automatic) {
                if (calculated_jump_direction == Vector3.zero) {
                    SetCalculatedJumpDirection();
                }
                return calculated_jump_direction;
            } else if (jump_direction_calculation == JumpDirectionSetting.manual) {
                return manual_jump_direction;
            } else {
                Debug.LogError($"unhandled jump_direction_calculation setting '{jump_direction_calculation}'");
                return Vector3.zero;
            }
        }
    }
    // public Vector3 jump_area_position = new Vector3(0f, 0f, 0f);
    // public Vector3 jump_area_size = new Vector3(0.85f, 0.65f, 5f);

    [Tooltip("Set whether jump direction is set manually or automatically calculated based on the collider.")]
    public JumpDirectionSetting jump_direction_calculation = JumpDirectionSetting.automatic;
    [Tooltip("if jump direction is set to manual calculation, it can be set here.")]
    public Vector3 manual_jump_direction;

    [Tooltip("Box Collider that defines the area a player will be able to jump from.")]
    [SerializeField]
    private BoxCollider target_collider;
    public Color gizmo_color = Color.yellow;

    [Tooltip("The absolute value of the dot-product between player-direction and jump-direction must be greater than this for a jump to trigger.")]
    public float angle_threshold = 0.75f;

    void Start() {
        if (jump_direction.y != 0) { Debug.LogWarning($"{gameObject.name}.jump_direction {jump_direction} contains a non-zero Y component!"); }
        SetCalculatedJumpDirection();
    }

    private Vector3 calculated_jump_direction = Vector3.zero;
    // private float cached_euler_rotation = float.NaN; // cache the euler rotation so you can recalculate if the rotation changes
    private void SetCalculatedJumpDirection() {
        calculated_jump_direction = GetCalculatedJumpDirection();
        // cached_euler_rotation = transform.rotation.eulerAngles.y;
    }

    public Vector3 GetCalculatedJumpDirection() {
        float x_width = target_collider.bounds.size.x;
        float z_width = target_collider.bounds.size.z;

        Vector3 result;
        if (x_width > z_width) {
            result = Vector3.forward;
        } else {
            result = Vector3.right;
        }
        Vector3 euler_rotation = transform.rotation.eulerAngles;
        int rotations = (int)(euler_rotation.y / 90);
        if ((euler_rotation.y / 90) != rotations) {
            Debug.LogWarning($"VaultOverCoverZone rotation calculation only supports 90* incriments, not '{euler_rotation.y}'");
        }
        return result;
    }

    void OnDrawGizmos() {
        calculated_jump_direction = GetCalculatedJumpDirection();
        if (gizmo_color == Color.clear) { return; }

        Vector3 center = transform.position;

        Gizmos.color = gizmo_color;
        Vector3 start = center - (jump_direction * (jump_length / 2));
        Vector3 end = center + (jump_direction * (jump_length / 2));
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

    public bool IsInJumpPosition(Vector3 player_position, Vector3 direction) {
        // check if player is moving in the right direction
        if (!ValidJumpAngle(player_position, direction)) {
            return false; 
        }
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
        return (direction * jump_direction).normalized;
    }

    public bool PositionInZone(Vector3 position) {
        return target_collider.bounds.Contains(position);
    }

    public bool ValidJumpAngle(Vector3 player_position, Vector3 direction) {
        return IsFacingTowards(player_position, direction)
            && Mathf.Abs(Vector3.Dot(direction.normalized, jump_direction.normalized)) > angle_threshold; // checks that the jump is paralell to the jump direction, within a threshold
    }
    
    public bool IsFacingTowards(Vector3 start_position, Vector3 direction) {
        // Takes a start position and direction, and deterimines if the resulting jump is going over the obstacle,
        //   or away from the obstacle in the opposite directio
        if (jump_direction.x != 0 && jump_direction.z != 0) { Debug.LogError($"only 1D jump directions are supported! '{jump_direction}' is invalid!"); }

        float isolated_start, center, isolated_direction;
        if (jump_direction.x != 0) {
            isolated_start = start_position.x;
            center = transform.position.x;
            isolated_direction = direction.normalized.x;
        } else if (jump_direction.z != 0) {
            isolated_start = start_position.z;
            center = transform.position.z;
            isolated_direction = direction.normalized.z;
        } else {
            Debug.LogError($"jump direction '{jump_direction}' needs either and X or Z component!");
            return false;
        }
        return _IsFacingTowards(isolated_direction, isolated_start, center);
    }
    
    private bool _IsFacingTowards(float isolated_direction, float start_position, float center) {
        // takes floats representing the relevent component of Vector3's to determine of the player is
        //  facing the correct direction to actually jump over the obsctacle from their current possition
        if (isolated_direction == 0) { return false; }
        if (start_position == center) { return false; } // TODO --- maybe raise an exception
        float correct_direction = center - start_position;

        float sign_check = correct_direction * isolated_direction; // will be possitve if signs match, negative is signs differ
        return sign_check > 0; // TODO --- implement
    }
}


public enum JumpDirectionSetting {
    manual, // jump direction is set via script
    automatic, // jump direction is calculated from the collider
}