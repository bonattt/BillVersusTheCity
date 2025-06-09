

using UnityEngine;

public class GunRangeCombatZone : MonoBehaviour {

    public LevelConfig level_config;
    public Transform target;
    public float x_size, z_size;
    public float y_rot_min, y_rot_max;
    public bool facing_correct_direction;

    [Tooltip("is the player detected in the zone this frame. (published for debugging)")]
    [SerializeField]
    private bool player_in_zone = false;

    private bool in_effect = false;
    private bool in_effect_last_frame = false;

    void Update() {
        in_effect_last_frame = in_effect;
        player_in_zone = TargetInZone();
        facing_correct_direction = TargetFacingDirection();
        in_effect = player_in_zone && facing_correct_direction;
        // only make updates when the player actually enters or leaves a combat enabled state
        if (in_effect != in_effect_last_frame) {
            if (in_effect) {
                level_config.combat_enabled = true;
            } else {
                level_config.combat_enabled = false;
            }
        }

        facing_correct_direction = TargetFacingDirection();
    }

    public bool TargetFacingDirection() {
        float rotation = target.rotation.eulerAngles.y;
        // adjust rotation to be a positive number
        while (rotation < 0) {
            rotation += 360;
        }
        return rotation < y_rot_max && rotation > y_rot_min;
    }

    public bool TargetInZone() {
        return target.position.x <= (transform.position.x + (x_size/2)) 
            && target.position.x >= (transform.position.x - (x_size/2))
            && target.position.z <= (transform.position.z + (z_size/2))
            && target.position.z >= (transform.position.z - (z_size/2));
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Vector3 cube_shape = new Vector3(x_size, 1, z_size);
        Gizmos.DrawWireCube(transform.position, cube_shape);
    }

}