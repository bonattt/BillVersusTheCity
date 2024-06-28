using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float x_offset = 0f;
    public float y_offset = 10f;
    public float z_offset = 0f; 
    public Transform target;
    
    public Vector3 offset {
        get {
            return new Vector3(x_offset, y_offset, z_offset);
        }
    }

    private void UpdatePosition() {
        transform.position = target.position + this.offset;
    } 

    void Start()
    {
        UpdatePosition();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }
}
