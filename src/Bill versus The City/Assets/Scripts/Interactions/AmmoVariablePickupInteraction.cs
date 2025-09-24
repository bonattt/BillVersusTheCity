using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class AmmoVariablePickupInteraction : MonoBehaviour, IInteractionEffect {
    // variation on the AmmoPickupInteraction where the ammo type is based on what weapon slot the player starts the level with
    public InteractableUI ui;

    [Tooltip("0 designates primary weapon, 1 designates secondary weapon (handgun)")]
    public int target_equipment_slot;
    public float ammo_amount_multiplier = 1f;

    public const int AMMO_COUNT_DEFAULT = 20; // ammo count used if the ammo type cannot be matched. Should never get used
    public int ammo_count_handgun = 20;
    public int ammo_count_magnum = 20;
    public int ammo_count_rifle = 15;
    public int ammo_count_shotgun = 10;
    // below counts are unlikely to be used, so they have been removed from the inspector, to declutter the UI
    private int ammo_count_grenade = 5; // un-likely to be used
    private int ammo_count_rocket = 3; // un-likely to be used

    private Dictionary<AmmoType, int> GetAmmoAmountDict() => new Dictionary<AmmoType, int> {
        {AmmoType.handgun, ammo_count_handgun},
        {AmmoType.magnum, ammo_count_magnum},
        {AmmoType.rifle, ammo_count_rifle},
        {AmmoType.shotgun, ammo_count_shotgun},
        {AmmoType.rocket,ammo_count_grenade},
        {AmmoType.grenade,ammo_count_rocket},
    };

    public int ammo_amount_set { get; private set; } // amount of ammo that has been dynamically set
    public AmmoType ammo_type_set { get; private set; } // type of ammo that has been dynamically set

    private const int AMMO_NOT_SET_FLAG = -1;
    

    public int GetAmmoAmount(AmmoType ammo_type) {
        // takes an ammo type and returns the ammo amount for that weapon.
        int base_amount;
        Dictionary<AmmoType, int> dict = GetAmmoAmountDict();
        if (dict.ContainsKey(ammo_type)) {
            base_amount = dict[ammo_type];
        } else {
            base_amount = AMMO_COUNT_DEFAULT;
            Debug.LogError($"missing AmmoType config for '{ammo_type}', using default value {AMMO_COUNT_DEFAULT}");
        }
        return (int)(base_amount * ammo_amount_multiplier);
    }

    public bool EquipmentIsReady() {
        return Time.timeScale != 0;
    }

    public bool AmmoHasBeenSet() {
        return ammo_amount_set != AMMO_NOT_SET_FLAG;
    }

    void Start() {
        ammo_amount_set = AMMO_NOT_SET_FLAG;
    }

    void Update() {
        if (!AmmoHasBeenSet() && EquipmentIsReady()) {
            SetAmmo();
        } else if (AmmoHasBeenSet()) {
            UpdateUIText(); // TODO --- optimize this to not be called in update???
        }
        
    }

    private void SetAmmo() {
        // sets ammo type and amount based on the weapon the player has equipped in their target slot
        IFirearm target_weapon;
        if (target_equipment_slot == 0) {
            target_weapon = PlayerCharacter.inst.inventory.rifle;
        } else if (target_equipment_slot == 1) {
            target_weapon = PlayerCharacter.inst.inventory.handgun;
        } else if (target_equipment_slot == 2) {
            target_weapon = PlayerCharacter.inst.inventory.pickup;
        } else {
            Debug.LogError($"invalid target weapon slot '{target_equipment_slot}', defaulting to use rifle!");
            target_weapon = PlayerCharacter.inst.inventory.rifle;
        }
        if (target_weapon == null) { Debug.LogError("no weapon equipped to target slot!!"); }
        ammo_type_set = target_weapon.ammo_type;
        ammo_amount_set = GetAmmoAmount(ammo_type_set);
    }

    private void UpdateUIText() {
        if (ui == null) {
            Debug.LogWarning("no ui set for ammo pickup!");
            return;
        }
        if (CanPlayerPickupAmmo()) {
            ui.SetNewText($"Pickup {ammo_amount_set} {ammo_type_set} ammo");
        } else {
            ui.SetNewText($"{ammo_type_set} ammo is full!");
        }
    }

    public bool CanPlayerPickupAmmo() {
        // returns true of the player is able to pickup ammo of this type.
        // if the player doesn't track ammo of this type, or is already full, return false.
        if (PlayerCharacter.inst.reload_ammo == null) { 
            Debug.LogWarning("no player ammo!");
            return false;
        }
        return PlayerCharacter.inst.reload_ammo.HasAmmoType(ammo_type_set) && PlayerCharacter.inst.reload_ammo.AmmoNeeded(ammo_type_set) >= 1;
    }

    public void Interact(GameObject actor) {
        if (CanPlayerPickupAmmo()) {
            MenuManager.PlayMenuSound("menu_click");
            int count = PlayerCharacter.inst.reload_ammo.GetCount(ammo_type_set);
            PlayerCharacter.inst.reload_ammo.SetCount(ammo_type_set, count + ammo_amount_set);
            Destroy(gameObject);
        } 
    }

}
