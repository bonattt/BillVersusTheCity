using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{

    public LayerMask layer_mask; // = LayerMask.GetMask("Interaction");
    public bool debug_can_interact = false;
    public Transform interaction_distance_from;  // measure how the closet interaction from this point
    public float interaction_range = 10f;

    private List<Interaction> near_interactions = new List<Interaction>();

    // set script to inactive while game is paused    
    private bool _is_active = true;
    public bool is_active {
        get { return _is_active; }
        set {
            _is_active = value;
        }
    }

    public static PlayerInteractor inst { get; private set; }

    void Awake() {
        inst = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (! is_active) { return; } // do nothing while script is disabled
        Interaction interaction = GetInteraction();
        if (interaction != null) {
            debug_can_interact = true;
        } else {
            debug_can_interact = false;
        }
        if (interaction != null) { interaction.interaction_targeted = true; }

        if (interaction != null && InputSystem.current.InteractInput()) {
            if (CanInteractWith(interaction)) {
                interaction.InteractWith(gameObject);
            }
            else {
                Debug.Log($"interaction out of range! (range {interaction.DistanceTo(transform)})");
            }
        }
        UpdateDebug();
    }

    public bool CanInteractWith(Interaction interactable) {
        return interactable.interaction_enabled && IsInInteractionRange(interactable);
    }

    private bool IsInInteractionRange(Interaction interaction) {
        return InteractionDistance(interaction) <= interaction_range;
    }

    private Interaction GetInteraction() {
        // returns the interaction a player would interact with if they input an interact action this frame

        // GameObject obj = InputSystem.current.GetHoveredObject(layer_mask);
        // if (obj != null) {
        //     Interaction interaction = obj.GetComponent<Interaction>();
        //     if (interaction == null) { Debug.LogWarning("hovered object is null"); } 
        //     return interaction;
        // }
        Interaction nearest_interaction = null;
        float nearest_distance = float.PositiveInfinity;
        foreach (Interaction interaction in near_interactions) {
            interaction.interaction_targeted = false;
            if (! CanInteractWith(interaction)) { continue; } // skip interactions that cannot be interacted with
            float distance_to = InteractionDistance(interaction);
            if (distance_to < nearest_distance) {
                nearest_interaction = interaction;
                nearest_distance = distance_to;
            }
        }
        return nearest_interaction;
    }

    public float InteractionDistance(Interaction interaction) {
        return HorizontalDistance(interaction_distance_from.position, interaction.transform.position);
    }

    public static float HorizontalDistance(Vector3 a, Vector3 b) {
        // returns the distance between 2 points with the Y component removed
        return Vector3.Distance(new Vector3(a.x, 0f, a.z), new Vector3(b.x, 0f, b.z));
    }

    void OnTriggerEnter(Collider c) {
        Interaction interaction = GetInteractionFromCollider(c);
        if (interaction != null) {
            interaction.interaction_targeted = false;
            interaction.interaction_tracked = true;
            near_interactions.Add(interaction);
        }
    }

    void OnTriggerExit(Collider c) {
        Interaction interaction = GetInteractionFromCollider(c);
        if (interaction != null) {
            interaction.interaction_targeted = false;
            interaction.interaction_tracked = false;
            near_interactions.Remove(interaction);
        }
    }

    public void InteractionDestroyed(Interaction interaction) {
        // called by OnDestroy on interaction
        near_interactions.Remove(interaction);
    }

    private static Interaction GetInteractionFromCollider(Collider c) {
        // Tries to GetComponent<Interaction> from a collider, checking parents of the
        // collider's object until it reaches the root parent or finds an nteraction
        Transform other = c.transform;
        Interaction interaction = other.GetComponent<Interaction>();
        while (interaction == null && other.parent != null) {
            other = other.parent;
            interaction = other.GetComponent<Interaction>();
        }
        return interaction;
    }

    ///////////////////////// DEBUG FIELDS ///////////////////////////
    
    public int debug_n_interactions = 0;
    private void UpdateDebug() {
        debug_n_interactions = near_interactions.Count;
    }
}
