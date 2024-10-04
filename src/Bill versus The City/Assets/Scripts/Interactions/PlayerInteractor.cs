using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{

    public LayerMask layer_mask; // = LayerMask.GetMask("Interaction");
    public bool debug_can_interact = false;

    public static PlayerInteractor inst { get; private set; }

    void Awake() {
        inst = this;
    }

    // Update is called once per frame
    void Update()
    {
        Interaction interaction = GetInteraction();
        if (interaction != null) {
            debug_can_interact = true;
        } else {
            debug_can_interact = false;
        }
        if (interaction != null && InputSystem.current.InteractInput()) {
            if (CanInteractWith(interaction)) {
                interaction.InteractWith(gameObject);
            }
            else {
                Debug.Log($"interaction out of range! (range {interaction.DistanceTo(transform)})");
            }
        }
    }

    public bool CanInteractWith(Interaction interactable) {
        return interactable.IsInRange(transform);
    }

    private Interaction GetInteraction() {
        GameObject obj = InputSystem.current.GetHoveredObject(layer_mask);
        if (obj != null) {
            Interaction interaction = obj.GetComponent<Interaction>();
            if (interaction == null) { Debug.Log("hovered object is null"); } 
            else { Debug.Log("hovered object is null"); } 
            return interaction;
        }
        return null;
    }
}
