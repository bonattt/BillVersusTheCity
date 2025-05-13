using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponSelectionUIController : AbstractCloseEventMenu
{
    /// <summary>
    ///  UI Controller that displays the player's availible AND currently equipped weapons
    /// </summary>
    
    public UIDocument ui_doc;
    private VisualElement root, left_content, right_content;
    private Button continue_button, cancel_button;

    public List<ScriptableObject> rifle_selection, handgun_selection;

    public bool confirm_selection = true;
    public bool allow_cancel = false;
    public bool clear_pickup_weapon = true;

    void Start() {
        SetupUI();
        UpdateContents(); // populate the contents of the visual elements with weapon UI elements 
        SelectEquipped(PlayerCharacter.inst.inventory.rifle, left_content);
        SelectEquipped(PlayerCharacter.inst.inventory.handgun, right_content);
    }

    public void SetupUI() {
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
        try {
            ApplySelection();
            MenuManager.inst.CloseMenu();
        } catch (WeaponNotSelectedException) {
            Debug.LogError("TODO --- handle WeaponNotSelectedException error!"); // note: this should be unreachable
        }
    }

    public void UpdateContents() {
        PopulateContents(left_content, PlayerCharacter.inst.inventory.availible_rifles);
        PopulateContents(right_content, PlayerCharacter.inst.inventory.availible_handguns);
    }

    public override void MenuNavigation() {
        if (allow_cancel && InputSystem.current.MenuCancelInput()) {
            CancelButtonClicked();
        }
    }

    private void SelectEquipped(IWeapon equipped, VisualElement parent) {
        foreach (IWeaponUI child in GetWeaponUIs(parent)) {
            if (equipped != null && child.weapon.item_name.Equals(equipped.item_name)) {
                child.SelectSlot();
            } else {
                child.DeselectSlot();
            }
        }
    }

    private void PopulateContents(VisualElement content_element, List<IWeapon> content) {
        content_element.Clear();
        foreach (IWeapon weapon in content) {
            WeaponListing list_item = new WeaponListing(weapon);
            content_element.Add(list_item);
        }
    }

    public IWeaponUI GetSelectedElementFrom(VisualElement parent) {

        foreach (IWeaponUI child in GetWeaponUIs(parent)) {
            if (child.is_selected) {
                return child;
            }
        }
        return null;
    }

    public static IEnumerable<IWeaponUI> GetWeaponUIs(VisualElement parent) {
        int count = 0;
        foreach(VisualElement child in parent.Children()) {
            if (child is IWeaponUI weapon_ui) {
                count += 1;
                yield return weapon_ui;
            } else {
                Debug.LogWarning($"non IWeaponUI element in weapon UI: {child}");
            }
        }
    }

    public void CancelButtonClicked() {
        MenuManager.inst.CloseMenu();
    }

    public void ApplySelection() {
        IWeaponUI selected_rifle = GetSelectedElementFrom(left_content);
        IWeaponUI selected_handgun = GetSelectedElementFrom(right_content);
        if (selected_rifle != null) {
            IWeapon new_weapon = selected_rifle.weapon.CopyWeapon();
            new_weapon.current_ammo = new_weapon.ammo_capacity;
            PlayerCharacter.inst.inventory.rifle = new_weapon; // NOTE: use property to update subscibers AFTER setting the ammo
        } else {
            throw new WeaponNotSelectedException("rifle not selected!");
        }

        if (selected_handgun != null) {
            IWeapon new_weapon = selected_handgun.weapon.CopyWeapon();
            new_weapon.current_ammo = new_weapon.ammo_capacity;
            PlayerCharacter.inst.inventory.handgun = new_weapon; // NOTE: use property to update subscibers AFTER setting the ammo
        } else {
            throw new WeaponNotSelectedException("handgun not selected!");
        }

        if (clear_pickup_weapon) {
            PlayerCharacter.inst.inventory.pickup = null; 
        }
    }
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
