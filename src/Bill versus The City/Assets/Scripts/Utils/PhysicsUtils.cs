using UnityEngine;

public static class PhysicsUtils {
    public static bool OverlapBox(Vector3 point, Vector3 box_center, Vector3 box_size) {
        // checks if a given point overlaps a given box, without involing unity physics or requiring a collider.
        Vector3 half = box_size * 0.5f;
        return (point.x >= box_center.x - half.x && point.x <= box_center.x + half.x) &&
            (point.y >= box_center.y - half.y && point.y <= box_center.y + half.y) &&
            (point.z >= box_center.z - half.z && point.z <= box_center.z + half.z);
    }

    

    private const float float_equality_threshold = 0.001f;
    private static bool FloatThresholdEqual(float a, float b, float threshhold=float_equality_threshold) {
        return Mathf.Abs(a - b) < threshhold;
    }

    public static bool VectorEquals(Vector3 a, Vector3 b) {
        return FloatThresholdEqual(a.x, b.x)
            && FloatThresholdEqual(a.y, b.y)
            && FloatThresholdEqual(a.z, b.z); 
    }
}
