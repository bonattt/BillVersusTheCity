using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class StairsInteraction : MonoBehaviour, IInteractionEffect
{
    public Transform move_to;
    
    public void Interact(GameObject actor) {
        Vector3 cam_offset = Camera.main.transform.position - PlayerCharacter.inst.player_transform.position;
        CharacterController char_ctrl = PlayerCharacter.inst.character_controller;
        char_ctrl.enabled = false;
        PlayerCharacter.inst.player_transform.position = move_to.position;
        char_ctrl.enabled = true;
        Camera.main.transform.position = move_to.position + cam_offset;
    }
}
