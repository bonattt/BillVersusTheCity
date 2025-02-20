using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointGizmos : MonoBehaviour
{
    public bool always_draw_gizmos = true;
    public Color gizmo_color = Color.cyan;
    public float gizmo_size = 0.75f;
    public WaypointSystem waypoints;

    private void OnDrawGizmos()
    {
        // if (always_draw_gizmos) {
        DrawAllGizmos();
        // }
    }


    // private void OnDrawGizmosSelected()
    // {
    //     if (!always_draw_gizmos) {
    //         DrawAllGizmos();
    //     }
    // }

    private void DrawAllGizmos() {
        if (waypoints == null) { 
            Debug.LogWarning("waypoint system not set, cannot draw gizmos!");
            return;
        }
        foreach (Transform t in waypoints.waypoints) {
            DrawWaypointGizmo(t);
        }
    }

    public void DrawWaypointGizmo(Transform waypoint) {
        Gizmos.color = gizmo_color;
        Gizmos.DrawWireCube(waypoint.position, new Vector3(gizmo_size, gizmo_size, gizmo_size));
    }
}
