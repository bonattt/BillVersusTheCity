using System.Collections.Generic;
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
    
    public static float FlatDistance(Vector3 first, Vector3 second) {
        // negates the Y-axis component before finding the distance between two points
        first = new Vector3(first.x, 0f, first.z);
        second = new Vector3(second.x, 0f, second.z);
        return Vector3.Distance(first, second);
    }

    public static Vector3 PositionAtHeight(Vector3 base_position, float height) {
        return new Vector3(base_position.x, height, base_position.z);
    }
    public static string LayerMaskToString(LayerMask mask) {
        List<string> layers = new List<string> ();
        for (int i = 0; i < 32; i++)
        {
            if ((mask.value & (1 << i)) != 0)
                layers.Add(LayerMask.LayerToName(i));
        }

        return string.Join(", ", layers);
    }
}
