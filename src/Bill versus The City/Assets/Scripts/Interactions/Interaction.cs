using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    // public List<MonoBehaviour> effects;
    public float interaction_range = 1f;
    public bool interaction_targeted = false;
    // public bool interaction_targeted {
    //     get { return _interaction_targeted; }
    //     set {
    //         _interaction_targeted = value;
    //         // TODO ---
    //     }
    // }
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

    void Update() {
        UpdateDebug();
    }

    void OnDestroy() {
        // cleans up the interaction if it is tracked by the interactor when this obejct is destroyed.
        PlayerInteractor.inst.InteractionDestroyed(this);
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

    /// ////////////////////////////// DEBUG ///////////////////////////////// <summary>
    public float debug_distance_to_interactor = 0f;
    private void UpdateDebug() {
        debug_distance_to_interactor = PlayerInteractor.inst.InteractionDistance(this);
    }
    
}
