using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrudeCrouchAnimator : MonoBehaviour
{

    public Transform target_object;
    public float crouched_y = 0.25f;
    public PlayerMovement player;
    private float uncrouched_y;
    
    // Start is called before the first frame update
    void Start()
    {
        uncrouched_y = target_object.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        float y_position = (crouched_y * player.crouch_percent) + (uncrouched_y * (1 - player.crouch_percent));
        target_object.localPosition = new Vector3(0, y_position, 0);

        float rotation = 90 * player.crouch_percent;
        target_object.rotation = Quaternion.Euler(new Vector3(0, 0, rotation));
    }
}
