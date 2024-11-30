using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class WeaponIcon : VisualElement, IWeaponUI {

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
    

    public WeaponIcon() : this(null) {
        
    }

    public WeaponIcon(IWeapon weapon) : base() {
        this.weapon = weapon;
        this.AddToClassList(BASE_STYLE_CLASS);
    }

    public void SelectSlot() {
        _is_selected = true;
        this.AddToClassList(ENABLED_STYLE_CLASS);
        this.AddToClassList(SELECTED_STYLE_CLASS);
    }

    public void DeselectSlot() {
        _is_selected = false;
        AddToClassList(ENABLED_STYLE_CLASS);
        RemoveFromClassList(SELECTED_STYLE_CLASS);
    }

    public void DisableSlot() {
        _is_selected = false;
        RemoveFromClassList(ENABLED_STYLE_CLASS);
        RemoveFromClassList(SELECTED_STYLE_CLASS);
    }
}