using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class WaypointSystemTest : MonoBehaviour {

    public bool check_for_cover = true;
    public Vector3 start_position;
    public Vector3 cover_from_pos;
    public Transform cover_from, start;
    public Color test_ray = Color.green;

    public Transform selected_point;

    void Update() {
        if (cover_from != null) {
            cover_from_pos = cover_from.position;
        }
        if (start != null) {
            start_position = start.position;
        }

        if (check_for_cover) {
            selected_point = WaypointSystem.inst.GetClosestCoverPosition(start_position, cover_from_pos);
        } else {
            selected_point = WaypointSystem.inst.GetClosestWaypoint(start_position);
        }

        if (selected_point != null) {
            Debug.DrawLine(start_position, selected_point.position, test_ray, Time.deltaTime);
        }
    }

}