using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    public Interaction interaction;
    public bool destroy_on_use = true;

    void OnTriggerEnter(Collider c) {
        if (! IsPlayer(c)) {
            return;
        }
        
        interaction.InteractWith(c.gameObject);
        if (destroy_on_use) {
            Destroy(this);
        }
    }


    private bool IsPlayer(Collider c) {
        GameObject other = c.gameObject;
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        while (player == null && other.transform.parent != null) {
            other = other.transform.parent.gameObject;
            player = other.GetComponent<PlayerMovement>();
        }
        return player != null;
    }

}
