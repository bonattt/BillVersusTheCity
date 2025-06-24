using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeFuse : MonoBehaviour {
    public Explosion explosion;
    public float fuse_length_seconds = 5f;
    public bool start_fuse_on_start = true;
    private float fuse_started_at = float.PositiveInfinity;

    public float fuse_progress {
        get {
            float progress = (Time.time - fuse_started_at) / fuse_length_seconds;
            return Mathf.Max(0f, Mathf.Min(progress, 1f));
        }
    }

    // Start is called before the first frame update
    void Start() {
        if (start_fuse_on_start) {
            StartFuse();
        }
        explosion.destroy_on_explode = true;
    }

    public void StartFuse() {
        fuse_started_at = Time.time;
    }

    // Update is called once per frame
    void Update() {
        if (fuse_started_at + fuse_length_seconds < Time.time) {
            explosion.Explode();
        }
        UpdateDebug();
    }

    //////////////////// DEBUG ////////////////////
    public float debug__fuse_progress = -1f;
    public void UpdateDebug() {
        debug__fuse_progress = fuse_progress;
    }
}
