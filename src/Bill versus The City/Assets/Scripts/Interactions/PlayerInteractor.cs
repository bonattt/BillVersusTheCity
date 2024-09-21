using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{

    public bool debug_can_interact = false;

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
            if (interaction.IsInRange(transform)) {
                interaction.InteractWith(gameObject);
            }
            else {
                Debug.Log($"interaction out of range! (range {interaction.DistanceTo(transform)})");
            }
        }
    }

    private Interaction GetInteraction() {
        GameObject obj = InputSystem.current.GetHoveredObject();
        if (obj != null) {
            Interaction interaction = obj.GetComponent<Interaction>();
            if (interaction == null) { Debug.Log("hovered object is null"); } 
            else { Debug.Log("hovered object is null"); } 
            return interaction;
        }
        Debug.Log("no hovered object!");
        return null;
    }
}
