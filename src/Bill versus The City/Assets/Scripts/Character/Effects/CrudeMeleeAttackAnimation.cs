

using UnityEngine;

public class CrudeMeleeAttackAnimation : MonoBehaviour
{
    public float duration_seconds = 1;
    public float speed = 5f;
    public Vector3 debug_move;

    void Start()
    {
        Destroy(gameObject, duration_seconds);
    }

    void Update()
    {
        Vector3 move = transform.forward * speed * Time.deltaTime;
        debug_move = move;
        Debug.DrawRay(transform.position, move * 50f, Color.red); // TODO --- remove debug
        transform.position += move;
    }
}