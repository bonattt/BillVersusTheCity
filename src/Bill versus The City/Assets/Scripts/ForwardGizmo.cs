
using UnityEngine;

public class ForwardGizmo : MonoBehaviour {

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Vector3 start = new Vector3(transform.position.x, 1f, transform.position.z);
        Vector3 end = start + transform.forward;
        Gizmos.DrawLine(start, end);
    }
}