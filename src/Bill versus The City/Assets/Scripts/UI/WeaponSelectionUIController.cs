using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class WeaponSelectionUIController : MonoBehaviour, ISubMenu
{
    /// <summary>
    ///  UI Controller that displays the player's availible AND currently equipped weapons
    /// </summary>
    
    public UIDocument ui_doc;
    private VisualElement root, left_content, right_content;
    private Button continue_button, cancel_button;

    private PlayerAttackController attack_ctrl;

    public bool confirm_selection = true;


    void Start() {
        root = ui_doc.rootVisualElement;
        left_content = root.Q<VisualElement>("LeftContent").Q<VisualElement>("Buttons");
        right_content = root.Q<VisualElement>("RightContent").Q<VisualElement>("Buttons");

        VisualElement footer = root.Q<VisualElement>("footer");
        continue_button = footer.Q<Button>("AcceptButton");
        cancel_button = footer.Q<Button>("CancelButton");

        continue_button.clicked += ContinueButtonClicked;
        if (cancel_button != null) {
            // TODO ---
        }
    }

    public void ContinueButtonClicked() {
        if (confirm_selection) {
            YesNoPopupController popup = MenuManager.inst.OpenNewPopup();
            popup.header_text = "Are you sure?";
            popup.content_text = "Continue with these Weapons?";
            popup.confirm_text = "Yes";
            popup.reject_text = "No";
            
            popup.confirm_button.clicked += ApplySelection;
            // popup.cancel_button.clicked += PauseMenuController.ExitGame;  // TODO --- move this helper somewhere more appropriate
            popup.UpdateLabels();
        } else {
            ApplySelection();
        }
    }

    public void ApplySelection() {

    }

    public void MenuNavigation() {
        // TODO --- 
    }


    // private void UpdateToolbelt(int? slot) {
    //     for (int i = 0; i < element_list.childCount; i++)
    //     {
    //         WeaponIcon child = (WeaponIcon) element_list[i];  // Get child by index
    //         child.AddToClassList("weapon_slot_container");
    //         if (attack_ctrl.weapon_slots_enabled[i]) {
    //             IWeapon weapon = attack_ctrl.weapon_slots[i];
    //             child.weapon = weapon;
    //             if (i == slot) {
    //                 child.SelectSlot();
    //             } else {
    //                 child.DeselectSlot();
    //             }
    //         }
    //         else {
    //             child.DisableSlot();
    //         }
            
    //     }
    // }

    // private static VisualElement GetWeaponIcon(IWeapon weapon) {
    //     VisualElement element = new VisualElement();
    //     element.AddToClassList("weapon_slot");
    //     element.style.backgroundImage = new StyleBackground(weapon.item_icon);
        
    //     // Texture2D texture = SpriteToTexture2D(weapon.item_icon);
    //     // element.style.backgroundImage = new StyleBackground(texture);
    //     return element;
    // }

    // private static Texture2D SpriteToTexture2D(Sprite sprite)
    // {
    //     // Helper from ChatGPT 
    //     // method to convert a Sprite to Texture2D

    //     if (sprite == null) return null;

    //     Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
    //     Color[] pixels = sprite.texture.GetPixels(
    //         (int)sprite.textureRect.x, 
    //         (int)sprite.textureRect.y, 
    //         (int)sprite.textureRect.width, 
    //         (int)sprite.textureRect.height
    //     );
    //     texture.SetPixels(pixels);
    //     texture.Apply();

    //     return texture;
    // }

}
