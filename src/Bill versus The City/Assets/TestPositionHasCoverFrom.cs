using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPositionHasCoverFrom : MonoBehaviour
{
    public Transform cover_from;
    public Vector3 cover_from_pos;

    public Color cover_color = Color.green;
    public Color clear_color = Color.red;

    // Update is called once per frame
    void Update()
    {
        if (cover_from != null) {
            cover_from_pos = cover_from.position;
        }

        foreach(Transform pos in WaypointSystem.inst.waypoints) {
            Color ray_color;
            if (NavMeshUtils.PositionHasCoverFrom(cover_from_pos, pos.position)) {
                ray_color = cover_color;
            } else {
                ray_color = clear_color;
            }
            Debug.DrawLine(cover_from_pos, pos.position, ray_color, Time.deltaTime);
        }

    }
}
