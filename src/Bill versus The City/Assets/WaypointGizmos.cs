using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class WaypointGizmos : MonoBehaviour
{
    public bool draw_gizmos = true;
    public Color gizmo_color = Color.cyan;
    public float gizmo_size = 0.75f;
    // public float click_tolerance = 0.5f;
    public WaypointSystem waypoints;

    void OnDrawGizmos()
    {
        if (!draw_gizmos) { return; }
        DrawAllGizmos();
    }

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
        // prevent the need for including some UnityEditor code in the build. Gizmos shouldn't be used in a build anyways
        # if UNITY_EDITOR
            Gizmos.color = gizmo_color;
            Gizmos.DrawWireSphere(waypoint.position, gizmo_size);
            
            Handles.color = Color.red;
            Gizmos.color = Color.red;
            Handles.Label(waypoint.position + Vector3.up * 0.5f, $"{waypoint.gameObject.name}");
        # endif
        // // Create a handle that allows clicking
        // if (Handles.Button(waypoint.position, Quaternion.identity, gizmo_size, gizmo_size*2, Handles.SphereHandleCap)) {
        //     Debug.LogWarning($"clicked on waypoint {waypoint.gameObject.name}");
        //     Selection.activeGameObject = waypoint.gameObject;
        // } 
    }
}
