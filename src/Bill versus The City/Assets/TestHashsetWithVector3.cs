using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHashsetWithVector3 : MonoBehaviour
{
    public Transform test_included;
    public bool vector_is_included, transform_is_included;
    public List<Transform> init_set;
    public HashSet<Vector3> vectors;
    public HashSet<Transform> transforms;

    void Start() {
        vectors = new HashSet<Vector3>();
        transforms = new HashSet<Transform>();
        foreach (Transform t in init_set) {
            transforms.Add(t);
            vectors.Add(t.position);
        }
    }

    void Update() {
        transform_is_included = transforms.Contains(test_included);
        vector_is_included = vectors.Contains(test_included.position);
    }
}
