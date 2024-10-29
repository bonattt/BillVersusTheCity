using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHover : MonoBehaviour
{
    public float x_rot = 1f;
    public float y_rot = 1.5f;
    public float z_rot = 2f;

    // Update is called once per frame
    void Update()
    {
        Vector3 rot = new Vector3(x_rot, y_rot, z_rot) * Time.deltaTime;
        Vector3 new_rotation = transform.rotation.eulerAngles + rot;
        transform.rotation = Quaternion.Euler(new_rotation);
    }
}
