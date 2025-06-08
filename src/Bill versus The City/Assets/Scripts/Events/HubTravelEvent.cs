using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubTravelEvent : AbstractInteractionGameEvent
{
    // event that implements "hub travel" (eg. moving the player between Bill's House and the gun store)
    public PlayerCombat player;
    public Transform destination;

    protected override void Effect() {
        Vector3 cam_offset = Camera.main.transform.position - PlayerCharacter.inst.player_transform.position;
        CharacterController char_ctrl = PlayerCharacter.inst.character_controller;
        char_ctrl.enabled = false;
        PlayerCharacter.inst.player_transform.position = destination.position;
        char_ctrl.enabled = true;
        Camera.main.transform.position = destination.position + cam_offset;
        InputSystem.current.FlushMovement(); // removes momentum from player's movement
    }
}
