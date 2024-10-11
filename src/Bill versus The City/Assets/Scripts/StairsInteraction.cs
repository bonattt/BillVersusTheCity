using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class StairsInteraction : MonoBehaviour, IInteractionEffect
{
    public Transform move_to;
    
    public void Interact(GameObject actor) {
        Vector3 cam_offset = Camera.main.transform.position - PlayerMovement.inst.transform.position;
        CharacterController char_ctrl = PlayerMovement.inst.GetComponent<CharacterController>();
        char_ctrl.enabled = false;
        PlayerMovement.inst.transform.position = move_to.position;
        char_ctrl.enabled = true;
        Camera.main.transform.position = move_to.position + cam_offset;
    }
}
