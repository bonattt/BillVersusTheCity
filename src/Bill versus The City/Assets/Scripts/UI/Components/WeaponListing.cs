using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class WeaponListing : VisualElement, IWeaponUI {

    public const string BASE_STYLE_CLASS = "weapon_slot_container";
    public const string ENABLED_STYLE_CLASS = "weapon_slot_container_enabled";
    public const string SELECTED_STYLE_CLASS = "weapon_slot_container_selected";

    private WeaponIcon _weapon_icon;
    private Label _weapon_label;
    private bool _is_disabled = false;
    public WeaponIcon weapon_icon {
        get { return _weapon_icon; }
    }

    public IWeapon weapon {
        get {
            return _weapon_icon.weapon;
        }
        set {
            _weapon_icon.weapon = value;
            if (value != null && !_is_disabled) {
                SetContents();
            } else {
                ClearContents();
            }
        }
    }

    public bool is_selected {
        get {
            return weapon_icon.is_selected;
        }
    }
    

    public WeaponListing() : this(null) { /* overloaded constructor: do nothing here */ }
    public WeaponListing(IWeapon weapon) : base() {
        this.AddToClassList("weapon_select_list_item");
        
        _weapon_icon = new WeaponIcon(weapon);
        _weapon_icon.name = "WeaponIcon";
        _weapon_icon.DeselectSlot();

        _weapon_label = new Label();
        _weapon_label.name = "Label";
        _weapon_label.text = weapon.item_name;
        
        this.Add(_weapon_icon);
        this.Add(_weapon_label);
        RegisterCallback<ClickEvent>(OnClick);
    }

    public void OnClick(ClickEvent _) {
        foreach(VisualElement child in this.parent.Children()) {
            try {
                IWeaponUI weapon_ui = (IWeaponUI) child;
                if (weapon_ui == this) {
                    weapon_ui.SelectSlot();
                } else {
                    weapon_ui.DeselectSlot();
                }
            } catch (InvalidCastException) {
                // skip non IWeaponUI elements
            }
        }
    }

    public void SelectSlot() {
        _is_disabled = false;
        weapon_icon.SelectSlot();
        SetContents();
    }

    public void DeselectSlot() {
        _is_disabled = false;
        weapon_icon.DeselectSlot();
        SetContents();
    }

    public void DisableSlot() {
        _is_disabled = true;
        weapon_icon.DisableSlot();
        ClearContents();
    }

    private void SetContents() {
        _weapon_label.text = this.weapon.item_name;
    }

    private void ClearContents() {
        _weapon_label.text = "";
    }
}

public interface IWeaponUI {
    public bool is_selected { get; }
    
    public IWeapon weapon { get; set; }
    public void SelectSlot();
    public void DeselectSlot();
    public void DisableSlot();
}