

using UnityEngine;

public class DropItemPickups : MonoBehaviour, ICharStatusSubscriber {
    // Script to control droping item pickups when an enemy dies.

    public bool drop_weapon = false;
    public int drop_dollars = 0;

    public bool drop_ammo = false;
    public GameObject ammo_pickup_prefab, dollars_pickup_prefab, weapon_pickup_prefab;
    private AttackController _attack_controller;
    protected AttackController attack_controller {
        get {
            if (_attack_controller == null) {
                _attack_controller = GetComponent<AttackController>();
            }
            return _attack_controller;
        }
    }

    //////////// ICharStatusSubscriber ///////////////
    public void StatusUpdated(ICharacterStatus status) { /* DO NOTHING */ }
    public void OnDamage(ICharacterStatus status) { /* DO NOTHING */ }
    public void OnHeal(ICharacterStatus status) { /* DO NOTHING */ }

    public void OnDeath(ICharacterStatus status) { /* Do nothing */ }

    public void DelayedOnDeath(ICharacterStatus status) { SpawnPickups(); }

    public void OnDeathCleanup(ICharacterStatus status) { /* Do nothing */ }
    
    //////////// END of ICharStatusSubscriber ///////////////

    private ICharacterStatus status;
    void Start() {
        status = GetComponent<ICharacterStatus>();
        status.Subscribe(this);
    }
    void OnDestroy()
    {
        status.Unsubscribe(this);
    }
    
    protected void SpawnPickups() {
        SpawnAmmoPickup();
        SpawnWeaponPickup();
        SpawnDollarsPickup();
    }

    protected void SpawnAmmoPickup() {
        // spawns a weapon pickup where the enemy is standing
        if (!drop_ammo) {
            return;
        }
        if (ammo_pickup_prefab == null || attack_controller.current_gun == null) {
            Debug.LogWarning("no weapon to drop ammo from!");
            return;
        }

        GameObject obj = Instantiate(ammo_pickup_prefab);
        obj.transform.position = transform.position + new Vector3(0f, 0f, 0.25f);
        AmmoPickupInteraction pickup = obj.GetComponent<AmmoPickupInteraction>();
        pickup.ammo_type = attack_controller.current_gun.ammo_type;
        pickup.ammo_amount = attack_controller.current_gun.ammo_drop_size;
    }

    protected void SpawnWeaponPickup() {
        // spawns a weapon pickup where the enemy is standing
        if (!drop_weapon) {
            return;
        }
        if (weapon_pickup_prefab == null || attack_controller.current_gun == null) {
            Debug.LogWarning("no weapon to drop as pickup!");
            return;
        }

        GameObject obj = Instantiate(weapon_pickup_prefab);
        obj.transform.position = transform.position + new Vector3(0f, 0f, -0.25f);;
        WeaponPickupInteraction pickup = obj.GetComponent<WeaponPickupInteraction>();
        pickup.pickup_weapon = attack_controller.current_gun;
    }

    protected void SpawnDollarsPickup() {
        // spawns a money pickup where the enemy is standing
        if (drop_dollars <= 0) {
            return;
        }
        if (dollars_pickup_prefab == null) {
            Debug.LogError("dollars pickup prefab not set!!");
            return;
        }

        GameObject obj = Instantiate(dollars_pickup_prefab);
        obj.transform.position = transform.position + new Vector3(0f, 0f, -0.25f);;
        MoneyPickupInteraction pickup = obj.GetComponent<MoneyPickupInteraction>();
        pickup.dollars = drop_dollars;
    }
}