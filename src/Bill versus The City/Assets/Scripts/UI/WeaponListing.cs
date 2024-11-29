using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class WeaponListing : VisualElement {

    public const string BASE_STYLE_CLASS = "weapon_slot_container";
    public const string ENABLED_STYLE_CLASS = "weapon_slot_container_enabled";
    public const string SELECTED_STYLE_CLASS = "weapon_slot_container_selected";

    private IWeapon _weapon;
    public IWeapon weapon {
        get {
            return _weapon;
        }
        set {
            _weapon = value;
            if (_weapon != null) {
                this.style.backgroundImage = new StyleBackground(weapon.item_icon);
            } else {
                this.style.backgroundImage = null;
            }
        }
    }

    private bool _is_selected = false;
    public bool is_selected {
        get {
            return _is_selected;
        }
    }
    

    public WeaponListing() : this(null) {
        // overloaded constructor: do nothing here
    }

    public WeaponListing(IWeapon weapon) : base() {
        this.weapon = weapon;
        this.AddToClassList(BASE_STYLE_CLASS);
    }

    public void SelectSlot() {
        _is_selected = true;
        this.AddToClassList(ENABLED_STYLE_CLASS);
        this.AddToClassList(SELECTED_STYLE_CLASS);
    }

    public void DeselectSlot() {
        _is_selected = true;
        AddToClassList(ENABLED_STYLE_CLASS);
        RemoveFromClassList(SELECTED_STYLE_CLASS);
    }

    public void DisableSlot() {
        _is_selected = true;
        RemoveFromClassList(ENABLED_STYLE_CLASS);
        RemoveFromClassList(SELECTED_STYLE_CLASS);
    }
}