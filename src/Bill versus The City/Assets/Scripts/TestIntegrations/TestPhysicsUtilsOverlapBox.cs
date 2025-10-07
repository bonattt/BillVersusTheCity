using UnityEngine;

public class TestPhysicsUtilsOverlapBox : MonoBehaviour
{
    public Vector3 target_position = new Vector3(0f, 0f, 0f);
    private float point_gizmo_radius = 10f;
    public Vector3 box_size = new Vector3(5f, 2f, 3f); 
    public Vector3 box_position = new Vector3(0f, 0f, 0f); 
    public bool overlaps = false;

    private bool PointPasses()
    {
        return PhysicsUtils.OverlapBox(target_position, box_position, box_size);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(box_position, box_size);

        if (PointPasses())
        {
            overlaps = true;
            Gizmos.color = Color.green;
        }
        else
        {
            overlaps = false;
            Gizmos.color = Color.red;
        }
        Gizmos.DrawSphere(target_position, point_gizmo_radius);
    }
}