using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubTravelEvent : MonoBehaviour, IGameEventEffect, IInteractionEffect
{
    public PlayerCombat player;
    public Transform destination;
    public void ActivateEffect() {
        Effect();
    }
    
    public void Interact(GameObject actor) {
        Effect();
    }


    private void Effect() {
        Vector3 cam_offset = Camera.main.transform.position - PlayerCharacter.inst.player_transform.position;
        CharacterController char_ctrl = PlayerCharacter.inst.character_controller;
        char_ctrl.enabled = false;
        PlayerCharacter.inst.player_transform.position = destination.position;
        char_ctrl.enabled = true;
        Camera.main.transform.position = destination.position + cam_offset;
    }
}
