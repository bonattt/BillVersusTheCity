

using UnityEngine;

public class DoorController : MonoBehaviour, IInteractionEffect, IGameEventEffect {

    public bool is_door_open = false;
    public GameObject door_model;

    public bool effect_completed { get => false; }
    public void ActivateEffect() {
        ToggleDoorOpen();
    }

    public void Interact(GameObject actor) {
        ToggleDoorOpen();
    }

    void Start() {
        _SetDoorOpen(is_door_open);
    }

    public void ToggleDoorOpen() {
        _SetDoorOpen(!is_door_open);
    }
    private void _SetDoorOpen(bool will_be_open) {
        if (will_be_open) {
            OpenDoor();
        } else {
            CloseDoor();
        }
    }

    public void OpenDoor() {
        door_model.SetActive(false);
        is_door_open = true;
    }

    public void CloseDoor() {
        door_model.SetActive(true);
        is_door_open = false;
    }

}