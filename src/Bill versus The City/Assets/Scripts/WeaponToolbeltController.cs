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
            child.AddToClassList("weapon_slot_container");
            if (attack_ctrl.weapon_slots_enabled[i]) {
                IWeapon weapon = attack_ctrl.weapon_slots[i];
                child.Clear();
                if (weapon != null) {
                    child.style.backgroundImage = new StyleBackground(weapon.item_icon);
                } else {
                    child.style.backgroundImage = null;
                }

                if (i == slot) {
                    child.AddToClassList("weapon_slot_container_enabled");
                    child.AddToClassList("weapon_slot_container_selected");
                } else {
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

    private static VisualElement GetWeaponIcon(IWeapon weapon) {
        VisualElement element = new VisualElement();
        element.AddToClassList("weapon_slot");
        element.style.backgroundImage = new StyleBackground(weapon.item_icon);
        
        // Texture2D texture = SpriteToTexture2D(weapon.item_icon);
        // element.style.backgroundImage = new StyleBackground(texture);
        return element;
    }

    private static Texture2D SpriteToTexture2D(Sprite sprite)
    {
        // Helper from ChatGPT 
        // method to convert a Sprite to Texture2D

        if (sprite == null) return null;

        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        Color[] pixels = sprite.texture.GetPixels(
            (int)sprite.textureRect.x, 
            (int)sprite.textureRect.y, 
            (int)sprite.textureRect.width, 
            (int)sprite.textureRect.height
        );
        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

}
