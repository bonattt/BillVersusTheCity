using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInteraction : MonoBehaviour, IInteractionEffect
{
    public string msg = "debug interaction!";

    public void Interact(GameObject actor) {
        Debug.Log(msg);
    }
}
