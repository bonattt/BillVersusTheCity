

using UnityEngine;

public class GunRangeCombatZone : MonoBehaviour {

    public LevelConfig level_config;
    public Transform target;
    public float x_size, z_size;
    public float y_rot_min, y_rot_max;
    public bool debug_facing_direction, debug_in_zone;

    // TODO --- remove debug: uncomment below
    // void Update() {
    //     if (TargetInZone() && TargetFacingDirection()) {
    //         level_config.combat_enabled = true;
    //     } else {
    //         level_config.combat_enabled = false;
    //     }
    //     debug_in_zone = TargetInZone();
    //     debug_facing_direction = TargetFacingDirection();
    // }

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