

using UnityEngine;

public class DoorController : MonoBehaviour, IInteractionEffect, IGameEventEffect {

    public bool is_door_open = false;
    public GameObject door_model;

    public string open_door_sound, close_door_sound;

    public bool effect_completed { get => false; }
    public void ActivateEffect() {
        ToggleDoorOpen();
    }

    public void Interact(GameObject actor) {
        ToggleDoorOpen();
    }

    void Start() {
        InitializeCollider();
        _SetDoorOpen(is_door_open, play_sound: false);
        ObjectTracker.inst.TrackObject(this);
    }

    void OnDestroy() {
        ObjectTracker.inst.RemoveObject(this);
    }

    private void InitializeCollider() {
        // Doors have a collider to keep ammo/weapon drops from overlapping with them. 
        // The orientation of this collider is relative to the screen, and how interaction UI shows up, so it should not change, relative to the door's physical rotation. 
        // This method adjusts the collider to be correct, if the door is rotated.
        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        if (collider == null) {
            Debug.LogWarning("cannot find Door-Interaction CapsuleCollider!");
            return;
        }
        // if (Mathf.Abs(transform.rotation.eulerAngles.y) != 90) {
        //     Debug.LogWarning("this implementation only works if doors are always at 90 degree incriments.");
        // }
        if (IsDoorRotated()) {
            // if the door is rotated, the collider should be set to X-axis, so the collider is still along the X-axis in global space
            collider.direction = 2; // Z-axis
        } else {
            // if the door isn't rotated, just set the collider to X-axis
            collider.direction = 0; // X-axis
        }
    }

    private void ValidateRotationAssumption() {
        // Checks if the door is rotated to a 90-degree incriment, and logs an error if it's not.
        if ((Mathf.Abs(transform.rotation.eulerAngles.y) % 90) > 0) { 
            Debug.LogWarning("this implementation only works if doors are always at 90 degree incriments.");
        }
    }

    private bool IsDoorRotated() {
        // hacky implementation to determine if the door has been roatated.
        // hacky, because it only supports 90* angles of roation.
        // returns true if the door has a +/-90-degree rotation (north-to-south)
        // returns false if the door is "not rotated" (0-degree equivalent, east-to-west)
        ValidateRotationAssumption();
        return (Mathf.Abs(transform.rotation.eulerAngles.y) % 180) > 0;
    }

    public void ToggleDoorOpen() {
        _SetDoorOpen(!is_door_open);
    }
    private void _SetDoorOpen(bool will_be_open, bool play_sound = true) {
        if (will_be_open) {
            OpenDoor(play_sound: play_sound);
        } else {
            CloseDoor(play_sound: play_sound);
        }
    }

    public void OpenDoor(bool play_sound = true) {
        door_model.SetActive(false);
        is_door_open = true;
        if (play_sound) {
            PlayDoorSound(open_door_sound);
        }
    }

    public void CloseDoor(bool play_sound = true) {
        door_model.SetActive(true);
        is_door_open = false;
        if (play_sound) {
            PlayDoorSound(close_door_sound);
        }
    }

    protected void PlayDoorSound(string sound_name) {
        if (sound_name == null || sound_name.Equals("")) {
            // do nothing
            Debug.LogWarning("no sound to play, skipping!");
            return;
        }
        ISFXSounds sounds = SFXLibrary.LoadSound(sound_name);
        SFXSystem.inst.PlaySound(sounds, transform.position);
    }

    void OnDrawGizmos() {
        if (door_model != null) {
            BoxCollider collider = door_model.GetComponent<BoxCollider>();
            Vector3 center = door_model.transform.position + collider.center;
            Vector3 size = Vector3.Scale(door_model.transform.lossyScale, collider.size);
            if (IsDoorRotated()) {
                // rotate the gizmo
                size = new Vector3(size.z, size.y, size.x);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawCube(center, size);
            // draw the collider where it would be, whether it's enabled or not
        } else {
            // no door model, draw an error cross
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + new Vector3(-0.5f, 0, 0), transform.position + new Vector3(0.5f, 0, 0));
            Gizmos.DrawLine(transform.position + new Vector3(0, -0.5f, 0), transform.position + new Vector3(0, 0.5f, 0));
            Gizmos.DrawLine(transform.position + new Vector3(0, 0, -0.5f), transform.position + new Vector3(0, 0, 0.5f));
        }
    }
}