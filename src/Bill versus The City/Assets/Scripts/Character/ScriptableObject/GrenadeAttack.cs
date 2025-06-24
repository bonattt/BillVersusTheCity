using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Thrown Weapon", menuName = "Data/ThrownAttack")]
public class ThrownAttack : ScriptableObject, IFirearm {

    /////////////////// implements IItem ///////////////////
    public string _item_id;
    public string item_id { get => _item_id; }
    public string _item_name;
    public string item_name { get => _item_name; }
    public Sprite _item_icon;
    public Sprite item_icon { get => _item_icon; }

    /////////////////// implements IWeapon ///////////////////
    public float weapon_damage_min { get => 0; }
    public float weapon_damage_max { get => 0; }
    public float armor_effectiveness { get => 1; }
    public string _attack_sound;
    public string attack_sound { get => _attack_sound; }
    public GameObject grenade_prefab;
    private GameObject grenade;


    /////////////////// implements IFirearm ///////////////////
    public WeaponSlot weapon_slot { get => WeaponSlot.pickup; }
    public WeaponClass weapon_class { get => WeaponClass.handgun; }
    // ammo
    public AmmoType ammo_type { get => AmmoType.grenade; }
    public int _ammo_capacity = 5;
    public int ammo_capacity { get => _ammo_capacity; }
    public int reload_amount { get => 0; }
    public float reload_time { get => 0; }
    public int current_ammo { get; set; }
    public int _ammo_drop_size = 2;
    public int ammo_drop_size { get => _ammo_drop_size; }

    // rate of fire
    public FiringMode firing_mode { get => FiringMode.semi_auto; }
    public bool auto_fire { get => false; }
    public float semi_auto_fire_rate { get => 1f; }
    public float full_auto_fire_rate { get => 1f; }
    public int n_shots { get => 1; }

    // accuracy
    public float aimed_inaccuracy { get => 0f; }
    public float initial_inaccuracy { get => 0f; }
    public float time_to_aim { get => 0f; }
    public float recoil_inaccuracy { get => 0f; }
    public float recoil_max { get => 0f; }
    public float recoil_recovery { get => 0f; }
    public float recoil_shake { get => 0f; }

    // damage
    public float bullet_speed { get => 0f; }
    // public float weapon_damage_min { get; } // moved to IWeapon from IFirearm
    // public float weapon_damage_max { get; } // moved to IWeapon from IFirearm
    // public float armor_effectiveness { get; } // moved to IWeapon from IFirearm
    public float damage_falloff_rate { get => 0f; }

    // aiming
    public float aim_zoom { get => 0f; }
    public float aim_move_speed { get => 0.5f; }
    public float max_zoom_range { get => 0f; }

    // effects
    // public string attack_sound { get; } // moved to IWeapon from IFirearm
    public GameObject bullet_prefab { get => grenade_prefab; }
    public string empty_gunshot_sound { get => ""; }
    public string reload_start_sound { get => ""; }
    public string reload_complete_sound { get => ""; }

    public bool HasWeaponSettings() => false;
    public void NextWeaponSetting() { /* do nothing */ }
    public void PreviousWeaponSetting() { /* do nothing */ }
    public IFirearm CopyFirearm() {
        return Instantiate(this);
    }

    public void AttackClicked(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker) {
        SpawnNewGrenade(attack_start_point, attacker);
    }
    public void AttackHold(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker) {
        // do nothing
    }

    public void AttackReleased(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker) {
        ThrowGrenade(attack_direction, attack_start_point);
    }

    private void ThrowGrenade(Vector3 attack_direction, Vector3 attack_start_point) {
        Debug.LogWarning("ThrowGrenade!!!"); // TODO --- remove debug
        grenade.transform.parent = null;
        grenade.transform.position = attack_start_point;

        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb == null) {
            rb = grenade.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
        float throw_force = 25f; // TODO --- make this dynamic
        rb.AddForce(attack_direction.normalized * throw_force, ForceMode.VelocityChange);
        grenade = null;

    }

    private GameObject SpawnNewGrenade(Vector3 attack_start_point, IAttackTarget attacker) {
        grenade = Instantiate(grenade_prefab);
        grenade.transform.position = attack_start_point;
        // grenade.transform.rotation = attacker.GetHitTarget().transform.rotation;
        grenade.transform.parent = attacker.GetHitTarget().transform;

        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb != null) {
            Destroy(rb);
        }
        return grenade;
    }

    public IWeapon CopyWeapon() {
        return Instantiate(this);
    }
}
