using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    // public List<MonoBehaviour> effects;
    public float interaction_range = 1f;
    public bool interaction_enabled = true;
    public List<MonoBehaviour> interaction_order = new List<MonoBehaviour>();

    public float DistanceTo(Transform actor) {
        return Vector3.Distance(transform.position, actor.position);
    }

    public bool IsInRange(Transform actor) {
        float distance = DistanceTo(actor);
        return distance <= interaction_range;
    }

    public void InteractWith(GameObject actor) {
        foreach (IInteractionEffect e in GetInteractions()) {
            e.Interact(actor);
        }
    }

    public IEnumerable<IInteractionEffect> GetInteractions() {
        if (interaction_order == null || interaction_order.Count == 0) {
            // by default, just grab all interactions in any order
            return GetComponents<IInteractionEffect>();
        }
        // if an explicit interaction order is defined, use that.
        List<IInteractionEffect> interactions = new List<IInteractionEffect>();
        for(int i = 0; i < interaction_order.Count; i++) {
            try {
                interactions.Add((IInteractionEffect) interaction_order[i]);
            } catch (InvalidCastException) {
                Debug.LogError($"non-interaction script found in interaction order at index {i}: {interaction_order[i]}");
            }
        }
        return interactions;
    }
    
}
