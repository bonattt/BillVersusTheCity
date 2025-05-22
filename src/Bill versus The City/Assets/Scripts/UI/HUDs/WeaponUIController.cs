using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class WeaponUIController : MonoBehaviour, IWeaponManagerSubscriber, IPlayerObserver
{

    private IWeaponManager target_manager = null;
    public UIDocument ui_doc;

    private VisualElement root;

    private Label weapon_label, ammo_label; 
    void Start()
    {
        // ui_doc = GetComponent<UIDocument>();
        root = ui_doc.rootVisualElement;
        weapon_label  = root.Q<Label>("CurrentWeaponLabel");
        ammo_label = root.Q<Label>("AmmoLabel");

        NewPlayerObject(PlayerCharacter.inst.GetPlayerCombat(this));
    }

    void OnDestroy() {
        PlayerCharacter.inst.UnsubscribeFromPlayer(this);
        target_manager.Unsubscribe(this);
    }
    
    public void NewPlayerObject(PlayerCombat player) {
        if (target_manager != null) {
            target_manager.Unsubscribe(this);
        }
        if (player == null) {
            Debug.Log("new player is null!");
            return;
        }
        target_manager = player.GetComponent<IWeaponManager>();
        target_manager.Subscribe(this);
        SetLabels(target_manager.current_slot, target_manager.current_weapon);
    }

    private void SetLabels(int? current_slot, IFirearm weapon) {
        SetAmmoLabel(weapon);
        SetWeaponLabel(weapon);
    }

    private void SetAmmoLabel(IFirearm weapon) {
        if (ammo_label == null) {
            return;
        }
        if (weapon == null) {
            ammo_label.text = "- / -";
        } else {
            ammo_label.text = $"{weapon.current_ammo} / {weapon.ammo_capacity}";
        }
    }

    private void SetWeaponLabel(IFirearm weapon) { 
        if (weapon_label == null) {
            return;
        }
        if (weapon == null) {
            weapon_label.text = "-";
        } else {
            string additional = "";
            // only apply (auto) to the weapons name if it is full-auto, and actually has more than one setting
            if (weapon.firing_mode == FiringMode.full_auto && weapon.HasWeaponSettings()) {
                additional = " (auto)"; 
            } 
            weapon_label.text = $"{weapon.item_name}{additional}";
        }
    }
    
    public void UpdateWeapon(int? slot, IFirearm weapon) {
        SetLabels(slot, weapon);
    }
}
