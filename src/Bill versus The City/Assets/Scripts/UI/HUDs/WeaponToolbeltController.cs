using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class WeaponToolbeltController : MonoBehaviour, IWeaponManagerSubscriber, IPlayerObserver
{
    /// <summary>
    ///  UI Controller that displays the player's availible AND currently equipped weapons
    /// </summary>
    
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
        element_list = ui_doc.rootVisualElement.Q<VisualElement>("EquipmentSlots");
        element_list.Clear();
        element_list.Add(new WeaponIcon());
        element_list.Add(new WeaponIcon());
        element_list.Add(new WeaponIcon());
        NewPlayerObject(PlayerCharacter.inst.GetPlayerCombat(this));
    }

    void OnDestroy() {
        PlayerCharacter.inst.UnsubscribeFromPlayer(this);
        if (attack_ctrl != null) {
            attack_ctrl.Unsubscribe(this);
        }
    }
    
    public void NewPlayerObject(PlayerCombat player) {
        // called when a new player replaces the existing player
        if (attack_ctrl != null) {
            attack_ctrl.Unsubscribe(this);
        }
        if (player == null) {
            Debug.Log("new player is null!");
            return;
        }
        attack_ctrl = player.attacks;
        attack_ctrl.Subscribe(this);
        UpdateToolbelt(null);
    }
    
    public void UpdateWeapon(int? slot, IWeapon weapon) {
        // called when changes are made to the observed IWeaponManager
        UpdateToolbelt(slot);
    }

    private void UpdateToolbelt(int? slot) {
        for (int i = 0; i < element_list.childCount; i++) {
            WeaponIcon child = (WeaponIcon) element_list[i];  // Get child by index
            child.AddToClassList("weapon_slot_container");
            if (attack_ctrl.weapon_slots_enabled[i]) {
                IWeapon weapon = attack_ctrl.weapon_slots[i];
                child.weapon = weapon;
                if (i == slot) {
                    child.SelectSlot();
                } else {
                    child.DeselectSlot();
                }
            }
            else {
                child.DisableSlot();
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
