using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    // public List<MonoBehaviour> effects;
    public float interaction_range = 1f;

    public float DistanceTo(Transform actor) {
        return Vector3.Distance(transform.position, actor.position);
    }

    public bool IsInRange(Transform actor) {
        float distance = DistanceTo(actor);
        return distance <= interaction_range;
    }

    public void InteractWith(GameObject actor) {
        foreach (IInteractionEffect e in GetComponents<IInteractionEffect>()) {
            e.Interact(actor);
        }
    }
    
}
