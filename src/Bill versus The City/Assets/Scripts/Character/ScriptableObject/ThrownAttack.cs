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
    public bool is_consumable { get => true; }


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
        if (current_ammo < 1) {
            Debug.LogError("grenade attack without ammo!");
            return;
        }
        ThrowGrenade(attack_direction, attack_start_point);
    }

    private void ThrowGrenade(Vector3 attack_direction, Vector3 attack_start_point) {
        if (grenade == null) {
            Debug.LogError("ThrowGrenade called without a primed grenade!");
            return;
        }
        grenade.transform.parent = null;
        grenade.transform.position = attack_start_point;

        GrenadeFuseUI fuse_ui = grenade.GetComponentInChildren<GrenadeFuseUI>();
        fuse_ui.SetVisibility(GrenadeFuseUI.ShowFuseUIWhenThrown());
        Debug.LogWarning($"show UI when thrown: {GrenadeFuseUI.ShowFuseUIWhenThrown()}, {GameSettings.inst.debug_settings.show_grenade_fuse}"); // TODO --- remove debug 

        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb == null) {
            rb = grenade.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
        // Vector3 mouse_pos = attack_start_point + (attack_direction.normalized * 3);
        Vector3 mouse_pos = InputSystem.current.MouseWorldPosition();
        if (float.IsNaN(mouse_pos.x)) {
            Debug.LogError("mouse position is NaN!");
        }
        Vector3 throw_velocity = CalculateThrowVelocity(attack_start_point, mouse_pos);
        // float MAX_THROW_FORCE = 15f;
        // if (throw_velocity.magnitude > MAX_THROW_FORCE) {
        //     throw_velocity = throw_velocity.normalized * MAX_THROW_FORCE;
        // }
        rb.AddForce(throw_velocity, ForceMode.VelocityChange);
        grenade = null;
        current_ammo -= 1;
    }

    // public float GetThrowForce(Vector3 throw_from, Vector3 target_position) {
    //     target_position = new Vector3(target_position.x, 0, target_position.z);

    //     Vector3 direction = (target_position - throw_from).normalized;
    //     Debug.DrawRay(throw_from, direction * 10, Color.yellow, 10f, depthTest: true);
    // }

    public Vector3 CalculateThrowVelocity(Vector3 start, Vector3 target) {
        Vector3 horizontal = new Vector3(target.x - start.x, 0, target.z - start.z);
        float distance = horizontal.magnitude;
        float height = start.y - target.y; // since target is at y = 0

        float throw_angle_degrees = 30f;
        float angle_rad = throw_angle_degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angle_rad);
        float sin = Mathf.Sin(angle_rad);

        // Calculate initial speed using kinematic equation
        float gravity = Physics.gravity.y;
        float numerator = Mathf.Abs(gravity * distance * distance);
        float denominator = 2f * (distance * Mathf.Tan(angle_rad) + height) * cos * cos;

        if (denominator <= 0) return Vector3.zero; // No valid solution

        float speed = Mathf.Sqrt(numerator / denominator);
        // Debug.LogWarning($"CALCULATE THROW VELOCITY speed: {Mathf.Sqrt(numerator / denominator)} = sqrt({numerator} / {denominator})"); // TODO --- remove debug

        // Decompose velocity into vector form
        Vector3 dir = horizontal.normalized;
        Vector3 velocity = dir * speed * cos;
        velocity.y = speed * sin;
        // Debug.LogWarning($"CALCULATE THROW VELOCITY dir: {dir} velocity {velocity}, velocity.y = {speed * sin} (speed * sin = {speed} * {sin})"); // TODO --- remove debug
        return velocity;
    }
    
    // public float GetTraverseDegreesToTarget(Vector3 target_position) {
    //     // returns the traverse degrees required to point the cannon in the direction of the given target
    //     target_position = new Vector3(target_position.x, 0, target_position.z);
    //     Vector3 position = new Vector3(cannon_base.position.x, 0, cannon_base.position.z);
    //     Vector3 direction = (target_position - position).normalized;
    //     Debug.DrawRay(cannon_base.position, direction * 10, Color.yellow, 10f, depthTest: true);
    //     float angle = Vector3.Angle(neutral_forward, direction);

    //     float dot = Vector3.Dot(direction, neutral_right);
    //     int sign;
    //     if (dot >= 0) {
    //         sign = 1;
    //     } else {
    //         sign = -1;
    //     }


    //     if (angle > 180) {
    //         angle -= 360;
    //     }
    //     return angle * sign;
    // }

    private GameObject SpawnNewGrenade(Vector3 attack_start_point, IAttackTarget attacker) {
        grenade = Instantiate(grenade_prefab);
        grenade.transform.position = attack_start_point;
        // grenade.transform.rotation = attacker.GetHitTarget().transform.rotation;
        grenade.transform.parent = attacker.GetHitTarget().transform;

        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb != null) {
            Destroy(rb);
        }
        GrenadeFuseUI fuse_ui = grenade.GetComponentInChildren<GrenadeFuseUI>();
        fuse_ui.SetVisibility(GrenadeFuseUI.ShowFuseUIWhenHeld());
        Debug.LogWarning($"show UI when held: {GrenadeFuseUI.ShowFuseUIWhenHeld()}, {GameSettings.inst.debug_settings.show_grenade_fuse}"); // TODO --- remove debug 

        return grenade;
    }

    public IWeapon CopyWeapon() {
        return Instantiate(this);
    }
}
