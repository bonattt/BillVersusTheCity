

using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float destroy_after_seconds = 5f;
    void Start() {
        Destroy(gameObject, destroy_after_seconds);
    }
}