

using UnityEngine;

public class CharacterCrouchColliderAdjuster : MonoBehaviour
{
    /* adjusts the height of a target CharacterController based on the crouch precent of a target CharCtrl */
    
    public CharacterController collider_; // unity script
    public ManualCharacterMovement character; // my script

    private float starting_height;
    public float crouched_height = 1f;

    void Start() {
        starting_height = collider_.height;
    }

    void Update() {
        float new_height = (crouched_height * character.crouch_percent) + ((1 - character.crouch_percent) * starting_height);
        collider_.height = new_height;
    }

}
