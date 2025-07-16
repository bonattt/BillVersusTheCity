

using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class PlayerInventoryDebugger : MonoBehaviour {

    public int? debug__slot_selected;
    public int debug__dollars, debug__total_dollars;
    public string debug__rifle, debug__handgun, debug__pickup, debug__last_longgun_equipped, debug__last_handgun_equipped;
    private PlayerInventory inventory {
        get => PlayerCharacter.inst.inventory;
    }

    void Update() {
        debug__slot_selected = inventory.slot_selected;
        debug__dollars = inventory.dollars;
        debug__total_dollars = inventory.total_dollars;
        debug__rifle = WeaponDisplay(inventory.rifle);
        debug__handgun = WeaponDisplay(inventory.handgun);
        debug__pickup = WeaponDisplay(inventory.pickup);
        debug__last_longgun_equipped = WeaponDisplay(inventory.last_rifle_equipped);
        debug__last_handgun_equipped = WeaponDisplay(inventory.last_handgun_equipped);
    }

    private static string WeaponDisplay(IFirearm weapon) {
        return weapon != null ? weapon.item_id : "null";
    }
}