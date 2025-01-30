using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGetNDirections : MonoBehaviour
{

    public Vector3 origin;
    public List<Vector3> output_rays;
    public int n_directions = 4;
    public float ray_length = 5f;
    public Color ray_color = Color.cyan;

    // Update is called once per frame
    void Update()
    {
        output_rays = NavMeshUtils.GetNDirections(n_directions);
        foreach(Vector3 ray in output_rays) {
            Debug.DrawRay(transform.position, ray.normalized * ray_length, ray_color, Time.deltaTime * 1.5f);
        }
    }
}
