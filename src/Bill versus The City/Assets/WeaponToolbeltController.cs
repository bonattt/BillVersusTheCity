using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class WeaponToolbeltController : MonoBehaviour, IWeaponManagerSubscriber
{
    
    public UIDocument ui_doc;
    private VisualElement element_list;

    public PlayerAttackController attack_ctrl;

    private static readonly string[] ALL_CLASSES = new string[]{
        ".weapon_slot",
        ".weapon_slot_container",
        ".weapon_slot_container_enabled",
        ".weapon_slot_container_selected",
    };

    void Start() {
        element_list = ui_doc.rootVisualElement.Q<VisualElement>("List");
        attack_ctrl.Subscribe(this);
        UpdateToolbelt(null);
    }
    
    public void UpdateWeapon(int? slot, IWeapon weapon) {
        UpdateToolbelt(slot);
    }

    private void UpdateToolbelt(int? slot) {
        for (int i = 0; i < element_list.childCount; i++)
        {
            VisualElement child = element_list[i];  // Get child by index
            if (attack_ctrl.weapon_slots_enabled[i]) {
                if (i == slot) {
                    child.AddToClassList("weapon_slot_container_enabled");
                    child.AddToClassList("weapon_slot_container_selected");
                } else {
                    Debug.Log($"slot {i} != {slot}");
                    child.AddToClassList("weapon_slot_container_enabled");
                    child.RemoveFromClassList("weapon_slot_container_selected");
                }
            }
            else {
                child.RemoveFromClassList("weapon_slot_container_enabled");
                child.RemoveFromClassList("weapon_slot_container_selected");
            }
            
        }
    }

}
