using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNavMeshRaycast : MonoBehaviour
{
    public Vector3 test_point;

    void Update()
    {
        TestPoints(transform.position, test_point);
    }

    public void TestPoints(Vector3 start, Vector3 end) {
        NavMeshHit hit;
        Color line_color;
        // tests for raycast hits IF both points are on the navmesh
        if (NavMesh.Raycast(start, end, out hit, NavMesh.AllAreas)) {
            line_color = Color.green;
        }
        else {
            line_color = Color.red;
        }
        Debug.DrawLine(start, end, line_color, Time.deltaTime * 2);
    }
}
