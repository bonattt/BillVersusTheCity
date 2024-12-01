using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponSelectionUIController : MonoBehaviour
{
    /// <summary>
    ///  UI Controller that displays the player's availible AND currently equipped weapons
    /// </summary>
    
    public UIDocument ui_doc;
    private VisualElement root, left_content, right_content;
    private Button continue_button, cancel_button;

    public List<ScriptableObject> rifle_selection, handgun_selection;
    private List<IWeapon> _rifle_selection = new List<IWeapon>();
    private List<IWeapon> _handgun_selection = new List<IWeapon>();

    public bool confirm_selection = true;
    public bool allow_cancel = false;

    void Start() {
        SetupUI();
        UpdateContents();
        // SelectElementFrom(left_content, _rifle_selection[0]);
        // SelectElementFrom(right_content, _handgun_selection[0]);
    }

    private void LoadWeaponsFromInspector(List<ScriptableObject> source, List<IWeapon> weapons) {
        foreach(ScriptableObject scriptable_weapon in source) {
            weapons.Add((IWeapon) scriptable_weapon);
        }
    }

    public void SetupUI() {
        LoadWeaponsFromInspector(handgun_selection, _handgun_selection);
        LoadWeaponsFromInspector(rifle_selection, _rifle_selection);

        root = ui_doc.rootVisualElement;
        left_content = root.Q<VisualElement>("LeftContent").Q<VisualElement>("Buttons");
        right_content = root.Q<VisualElement>("RightContent").Q<VisualElement>("Buttons");

        VisualElement footer = root.Q<VisualElement>("Footer");
        continue_button = footer.Q<Button>("AcceptButton");
        cancel_button = footer.Q<Button>("CancelButton");

        continue_button.clicked += ContinueButtonClicked;
        if (!allow_cancel) {
            cancel_button.parent.Remove(cancel_button);
            cancel_button = null;
        } else {
            cancel_button.clicked += CancelButtonClicked;
        }
    }

    public void ContinueButtonClicked() {
        // if (confirm_selection) {
        //     YesNoPopupController popup = MenuManager.inst.OpenNewPopup();
        //     popup.header_text = "Are you sure?";
        //     popup.content_text = "Continue with these Weapons?";
        //     popup.confirm_text = "Yes";
        //     popup.reject_text = "No";
            
        //     popup.confirm_button.clicked += ApplySelection;
        //     popup.confirm_button.clicked += MenuManager.inst.CloseMenu;
        //     // popup.cancel_button.clicked += PauseMenuController.ExitGame;  // TODO --- move this helper somewhere more appropriate
        //     popup.UpdateLabels();
        // } 
        try {
            ApplySelection();
            MenuManager.inst.CloseMenu();
        } catch (WeaponNotSelectedException) {
            Debug.LogError("TODO --- handle WeaponNotSelectedException error!");
        }
    }

    public void UpdateContents() {
        Debug.Log("UpdateContents");
        PopulateContents(right_content, _handgun_selection);
        PopulateContents(left_content, _rifle_selection);
    }

    public void WeaponListingOnClick(ClickEvent _) {
        
    }

    private void PopulateContents(VisualElement content_element, List<IWeapon> content) {
        content_element.Clear();
        bool first = true;
        foreach (IWeapon weapon in content) {
            WeaponListing list_item = new WeaponListing(weapon);
            // list_item.name = weapon.item_name;
            // list_item.AddToClassList("weapon_select_list_item");
            
            // WeaponIcon icon = new WeaponIcon(weapon);
            // icon.name = "WeaponIcon";
            // icon.DeselectSlot();

            // Label label = new Label();
            // label.name = "Label";
            // label.text = weapon.item_name;
            
            // list_item.Add(icon);
            // list_item.Add(label);
            content_element.Add(list_item);
            if (first) {
                first = false;
                list_item.SelectSlot();
            } else {
                list_item.DeselectSlot();
            }
        }
    }

    // public void SelectElementFrom(VisualElement parent, IWeapon weapon) {
    //     foreach(VisualElement child in parent.Children()) {
    //         try {
    //             IWeaponUI listing = (IWeaponUI) child;
    //             if (listing.weapon == weapon) {
    //                 SelectElementFrom(parent, listing);
    //                 return;
    //             }
    //         } catch (InvalidCastException) {
    //             // do nothing with non WeaponSlot elements
    //         }
    //     }
    //     Debug.LogError("SelectElementFrom called on weapon not contained in parent!");
    // }

    // public void SelectElementFrom(VisualElement parent, IWeaponUI selected) {
    //     bool success = false;
    //     foreach (VisualElement child in parent.Children()) {
    //         try {
    //             IWeaponUI slot = (IWeaponUI) child;
    //             if (slot == selected) {
    //                 slot.SelectSlot();
    //                 success = true;
    //             } else {
    //                 slot.DeselectSlot();
    //             }
    //         } catch (InvalidCastException) {
    //             // do nothing with non WeaponSlot elements
    //         }
    //     }

    //     if (!success) {
    //         Debug.LogError("SelectElementFrom called on weapon not contained in parent!");
    //     }
    // }

    public IWeaponUI GetSelectedElementFrom(VisualElement parent) {

        foreach (VisualElement child in parent.Children()) {
            try {
                IWeaponUI slot = (IWeaponUI) child;
                if (slot.is_selected) {
                    return slot;
                }
            } catch (InvalidCastException) {
                // do nothing with non WeaponSlot elements
            }
        }
        return null;
    }

    public void CancelButtonClicked() {
        MenuManager.inst.CloseMenu();
    }

    public void ApplySelection() {
        IWeaponUI selected_rifle = GetSelectedElementFrom(left_content);
        IWeaponUI selected_handgun = GetSelectedElementFrom(right_content);
        if (selected_rifle != null) {
            PlayerCharacter.inst.inventory.rifle = selected_rifle.weapon.CopyWeapon();
            PlayerCharacter.inst.inventory.rifle.current_ammo = PlayerCharacter.inst.inventory.rifle.ammo_capacity;
        } else {
            throw new WeaponNotSelectedException("rifle not selected!");
        }

        if (selected_handgun != null) {
            PlayerCharacter.inst.inventory.handgun = selected_handgun.weapon.CopyWeapon();
            PlayerCharacter.inst.inventory.handgun.current_ammo = PlayerCharacter.inst.inventory.handgun.ammo_capacity;
        } else {
            throw new WeaponNotSelectedException("handgun not selected!");
        }

        PlayerCharacter.inst.inventory.pickup = null;
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

[System.Serializable]
public class WeaponNotSelectedException : System.Exception
{
    // public WeaponNotSelectedException() { }
    public WeaponNotSelectedException(string message) : base(message) { }
    // public WeaponNotSelectedException(string message, System.Exception inner) : base(message, inner) { }
    // protected WeaponNotSelectedException(
    //     System.Runtime.Serialization.SerializationInfo info,
    //     System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
