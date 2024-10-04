using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class WeaponUIController : MonoBehaviour, IWeaponManagerSubscriber
{

    public GameObject target;
    private IWeaponManager target_manager;
    private UIDocument ui_doc;

    private VisualElement root;

    private Label weapon_label, ammo_label; 
    void Start()
    {
        ui_doc = GetComponent<UIDocument>();
        root = ui_doc.rootVisualElement;
        weapon_label  = root.Q<Label>("CurrentWeaponLabel");
        ammo_label = root.Q<Label>("AmmoLabel");

        target_manager = target.GetComponent<IWeaponManager>();
        target_manager.Subscribe(this);

        SetLabels(target_manager.current_slot, target_manager.current_weapon);
    }

    private void SetLabels(int? current_slot, IWeapon weapon) {
        SetAmmoLabel(weapon);
        SetWeaponLabel(weapon);
    }

    private void SetAmmoLabel(IWeapon weapon) {
        if (ammo_label == null) {
            return;
        }
        if (weapon == null) {
            ammo_label.text = "- / -";
        } else {
            string automatic =  weapon.firing_mode == FiringMode.full_auto ? " (auto)": ""; 
            ammo_label.text = $"{weapon.current_ammo} / {weapon.ammo_capacity}{automatic}";
        }
    }

    private void SetWeaponLabel(IWeapon weapon) { 
        if (weapon_label == null) {
            return;
        }
        if (weapon == null) {
            weapon_label.text = "-";
        } else {
            weapon_label.text = $"{weapon.item_name}";
        }
    }
    
    public void UpdateWeapon(int? slot, IWeapon weapon) {
        SetLabels(slot, weapon);
    }
}
