using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class StairsInteraction : MonoBehaviour, IInteractionEffect
{
    [Tooltip("Transform containing the position of where to teleport the player to upon interaction.")]
    public Transform move_to;
    
    public void Interact(GameObject actor) {
        PlayerCharacter.inst.TeleportPlayerTo(move_to.position);
    }
}
