using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public IWeapon weapon { get; set; }
    public float time_to_live = 30f;
    private float start_time;
    private Rigidbody rb; 

    // Start is called before the first frame update
    void Start()
    {
        start_time = Time.time;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    // Update is called once per frame
    void Update()
    {
        if (start_time + time_to_live <= Time.time) {
            Destroy(this.gameObject);
        }
    }
}
