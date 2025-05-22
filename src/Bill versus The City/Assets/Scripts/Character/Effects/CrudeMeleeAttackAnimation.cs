

using UnityEngine;

public class CrudeMeleeAttackAnimation : MonoBehaviour
{
    public float duration_seconds = 1;
    public float speed = 5f;

    void Start()
    {
        Destroy(gameObject, duration_seconds);
    }

    void Update()
    {
        Vector3 move = transform.forward * speed * Time.deltaTime;
        transform.Translate(move);
    }
}