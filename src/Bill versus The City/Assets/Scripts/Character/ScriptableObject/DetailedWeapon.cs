using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Detailed Weapon", menuName ="Data/DetailedWeapon")]
public class DetailedWeapon : ScriptableObject, IFirearm
{
    public string _name;
    public string _item_id;
    public string item_id { get => _item_id; }
    public Sprite _item_icon;
    
    public WeaponSlot _weapon_slot;
    public WeaponSlot weapon_slot { get { return _weapon_slot; } }
    public WeaponClass _weapon_class;
    public WeaponClass weapon_class { get { return _weapon_class; }}

    public WeaponSetting[] _weapon_settings;
    private int current_setting = 0;
    public WeaponSetting weapon_setting { get { return _weapon_settings[current_setting]; }}

    public BulletAttributes bullet_effect;

    public int _ammo_capacity = 30;
    public int _reload_amount = 30;
    public float _reload_time ;

    // accuracy
    public float _time_to_aim = 1f;
    public float _recoil_inaccuracy = 0.5f;
    public float _recoil_max = 15f;
    public float _recoil_recovery = 30f;
    public float _recoil_shake = 0.1f;

    // aiming
    public float _aim_zoom = 0.5f;
    public float _aim_move_speed = 0.5f;
    public float _max_zoom_range = 5f;

    // effects
    public string _gunshot_sound, _empty_gunshot_sound, _reload_start_sound, _reload_complete_sound;

    public GameObject bullet_prefab { get => bullet_effect.bullet_prefab; }
    public string item_name { get => _name; }
    public Sprite item_icon { get => _item_icon; }

    public AmmoType ammo_type { 
        get { return bullet_effect.ammo_type; } 
    }
    
    public int ammo_drop_size { 
        get { return bullet_effect.ammo_drop_size; }
    }
    public int ammo_capacity { 
        get { return _ammo_capacity; } 
    }
    public int reload_amount { 
        get { return _reload_amount; } 
    }
    public float reload_time { 
        get { return _reload_time; } 
    }
    public int current_ammo { get; set; }

    // rate of fire
    public FiringMode firing_mode { 
        get { return weapon_setting.firing_mode; }
    }
    public bool auto_fire { 
        // check ammo to prevent fully-auto "empty weapon" click
        get { return firing_mode == FiringMode.full_auto && current_ammo > 0; }
    }
    public float semi_auto_fire_rate { 
        get { return weapon_setting.fire_rate; }
    }
    public float full_auto_fire_rate { 
        get { return weapon_setting.fire_rate; }
    }
    public int n_shots {
        get { return bullet_effect.n_shots; }
    }

    // accuracy
    public float aimed_inaccuracy { 
        get { return bullet_effect.aimed_inaccuracy; }
    }
    public float initial_inaccuracy { 
        get { return bullet_effect.initial_inaccuracy; }
    }
    public float time_to_aim { 
        get { return _time_to_aim; }
    }
    public float recoil_inaccuracy { 
        get { return _recoil_inaccuracy; }
    }
    public float recoil_max { 
        get { return _recoil_max; }
    }
    public float recoil_recovery { 
        get { return _recoil_recovery; }
    }
    public float recoil_shake { 
        get { return _recoil_shake; }
    }
    // aiming
    public float aim_zoom {
        get { return _aim_zoom; }
    }
    public float aim_move_speed { 
        get { return _aim_move_speed; }
    }
    public float max_zoom_range { 
        get { return _max_zoom_range; }
    }
    
    // damage
    public float bullet_speed { 
        get { return bullet_effect.bullet_speed; }
    }
    public float weapon_damage_min { 
        get { return bullet_effect.weapon_damage_min; }
    }
    public float weapon_damage_max { 
        get { return bullet_effect.weapon_damage_max; }
    }
    public float armor_effectiveness { 
        get { return bullet_effect.armor_effectiveness; }
    }
    public float damage_falloff_rate {
        get { return bullet_effect.damage_falloff_rate; }
    }

    public string attack_sound {
        get { return _gunshot_sound; }
    }
    
    public string empty_gunshot_sound { 
        get { return _empty_gunshot_sound; }
    }

    public string reload_start_sound {
        get { return _reload_start_sound; }
    }

    public string reload_complete_sound {
        get { return _reload_complete_sound; }
    }

    public override string ToString() {
        return $"DetailedWeapon<{_name}>";
    }

    public bool HasWeaponSettings() {
        // returns true if the weapon has more than one firing mode
        return _weapon_settings.Length > 1;
    }
    
    public void NextWeaponSetting() {
        current_setting += 1;
        if (current_setting >= _weapon_settings.Length) {
            current_setting = 0;
        }
    }

    public void PreviousWeaponSetting() {
        current_setting -= 1;
        if (current_setting < 0) {
            current_setting = _weapon_settings.Length - 1;
        }
    }
    
    public void AttackClicked(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker) {
        FireAttack(attack_direction, attack_start_point, inaccuracy, attacker);
    }
    public void AttackHold(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker) {
        if (!auto_fire) { return; } // if not automatic, do not shoot for hold fire
        FireAttack(attack_direction, attack_start_point, inaccuracy, attacker);
    }

    public void AttackReleased(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker) {
        // do nothing
    }

    private void FireAttack(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker) {
        Bullet bullet = null;
        for (int i = 0; i < n_shots; i++) {
            GameObject bullet_obj = Instantiate(bullet_prefab);

            bullet_obj.transform.position = GetShootPoint(attack_start_point, i);
            Vector3 velocity = GetAttackVector(attack_direction, inaccuracy, i);
            bullet_obj.GetComponent<Rigidbody>().velocity = velocity;

            bullet = bullet_obj.GetComponent<Bullet>();
            bullet.weapon = this;
            bullet.attack_damage_max = weapon_damage_max;
            bullet.attack_damage_min = weapon_damage_min;
            bullet.armor_effectiveness = armor_effectiveness;
            bullet.damage_falloff_rate = damage_falloff_rate;
            bullet.attacker = attacker;
        }
        current_ammo -= 1;
        if (bullet != null) {
            AttackResolver.AttackStart(bullet, attack_direction, attack_start_point, is_melee_attack: false); // Only create a shot effects once for a shotgun
        } else {
            Debug.LogWarning("bullet is null!");
        }
        // #if UNITY_EDITOR
        //     EditorApplication.isPaused = true;
        // #endif
    }
    
    private Vector3 GetShootPoint(Vector3 attack_start_point, int i) {
        // takes a loop counter, and returns the start point for a bullet. If loop counter is 0, the start point is always the same, 
        // but if loop counter is not 0, a random offset is applied.
        if (i == 0) {
            return attack_start_point;
        }
        // radom variance to make shotguns feel better
        float variance = 0.25f;
        Vector3 offset = new Vector3(UnityEngine.Random.Range(0, variance), 0, UnityEngine.Random.Range(0, variance));
        return attack_start_point + offset;
    }
    
    private Vector3 GetAttackVector(Vector3 attack_direction, float inaccuracy, int i) {
        Vector3 base_vector = attack_direction.normalized * bullet_speed;

        float deviation = UnityEngine.Random.Range(-inaccuracy, inaccuracy);

        Vector3 rotation_axis = Vector3.up;
        Vector3 rotated_direction = Quaternion.AngleAxis(deviation, rotation_axis) * base_vector;

        float multiplier;
        if(i == 0) {
            multiplier = 1;
        } else {
            multiplier = UnityEngine.Random.Range(0.9f, 1.1f);
        }

        return rotated_direction * multiplier;
    }

    public IWeapon CopyWeapon() => CopyFirearm();
    public IFirearm CopyFirearm()
    {
        return Instantiate(this);
    }
}
