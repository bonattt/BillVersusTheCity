using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimativeAttackEffect : MonoBehaviour
{
    public float start_scale = 0.001f;
    public float max_scale = 0.1f;
    public float scale_rate = 1;

    void Start() {
        transform.localScale = new Vector3(start_scale, start_scale, start_scale);
    }

    void Update() {
        float frame_scale = scale_rate * Time.deltaTime;
        Vector3 scale_vector = new Vector3(frame_scale, frame_scale, frame_scale);
        transform.localScale += scale_vector;
        if (transform.localScale.x >= max_scale) {
            Destroy(this.gameObject);
        }

    }
}
