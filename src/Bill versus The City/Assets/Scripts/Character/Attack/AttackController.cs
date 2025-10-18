using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum AttackStartPosition {
    bullet,
    thrown,
}

public class AttackController : MonoBehaviour, IWeaponManager, IAttackController {
    public ScriptableObject initialize_weapon;

    public IWeapon current_weapon { get => current_gun; set { current_gun = (IFirearm)value; } }
    public IFirearm current_gun { get; set; }
    public IMeleeWeapon current_melee {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
    }
    public IAttackTarget attacker = null;

    public bool switch_weapons_blocked { get; set; }

    public bool start_weapon_loaded = true;

    [Tooltip("normal attacks will start at this position.")]
    public Transform _shoot_from; // bullets start here
    public Transform shoot_from { get => _shoot_from; } // bullets start here

    [Tooltip("attacks fired in arcs, which should clear cover even when crouching, are fired from here.")]
    public Transform _throw_from;
    public Transform throw_from { get => _throw_from; }

    public Transform attack_start_position {
        get {
            if (current_gun != null && current_gun.attack_start_position == AttackStartPosition.thrown) {
                return throw_from;
            }
            return shoot_from;
        }
    }

    public GameObject _default_bullet_prefab;
    public GameObject bullet_prefab {
        get {
            if (current_gun != null && current_gun.bullet_prefab != null) {
                return current_gun.bullet_prefab;
            }
            return _default_bullet_prefab;
        }
    }

    public float inaccuracy_modifier = 0f;

    // set the script to be inactive while game is paused
    private bool _is_active = true;
    public bool is_active {
        get { return _is_active; }
        set {
            // avoids shooting because you clicked on the last frame of dialouge
            _is_active = value;
        }
    }

    // how fast bullets move when fired
    public float bullet_speed {
        get {
            if (current_gun == null) {
                Debug.LogWarning("no weapon selected!");
                return 35f;
            }
            return current_gun.bullet_speed;
        }
    }
    // how long must pass from the previous shot before you can shoot again
    public float shot_cooldown {
        get {
            if (current_gun == null) {
                Debug.LogWarning("no weapon selected!");
                return 0.5f;
            }
            if (current_gun.auto_fire) {
                return current_gun.full_auto_fire_rate;
            }
            return current_gun.semi_auto_fire_rate;
        }
    }

    // tracks when the last shot was fired, for handling rate-of-fire
    private float _last_shot_at = 0f;

    //////// IMPLEMENT `IWeaponManagerSubscriber`
    protected List<IWeaponManagerSubscriber> subscribers = new List<IWeaponManagerSubscriber>();
    public void Subscribe(IWeaponManagerSubscriber sub) => subscribers.Add(sub);
    public void Unsubscribe(IWeaponManagerSubscriber sub) => subscribers.Remove(sub);
    public virtual void UpdateSubscribers() {
        foreach (IWeaponManagerSubscriber sub in subscribers) {
            sub.UpdateWeapon(null, current_gun);
        }
    }

    public bool HasAmmo() =>  current_gun.current_ammo > 0;
    public bool CanAttack() => current_gun != null && HasAmmo() && is_active;

    public virtual int? current_slot {
        get { return null; }
    }

    private float? start_aim_at = null;
    public bool is_aiming { get { return start_aim_at != null; } }

    public float aim_percent {
        get {
            if (start_aim_at == null) { return 0f; }

            float aimed_for = Time.time - ((float)start_aim_at);

            float time_to_aim;
            if (current_gun == null) {
                time_to_aim = float.PositiveInfinity;
            } else {
                time_to_aim = current_gun.time_to_aim;
            }
            return Mathf.Clamp(
                (aimed_for / time_to_aim), 0f, 1f
            );
        }
    }

    public float _current_recoil = 0f;
    public float current_recoil {
        get { return _current_recoil; }
        set {
            if (value <= 0) {
                _current_recoil = 0;
            } else if (value >= current_gun.recoil_max) {
                _current_recoil = current_gun.recoil_max;
            } else {
                _current_recoil = value;
            }
        }
    }

    public float current_inaccuracy {
        get {
            if (current_gun == null) { return -1f; }
            if (start_aim_at == null) {
                return current_gun.initial_inaccuracy;
            }
            return (current_gun.aimed_inaccuracy * aim_percent) + (current_gun.initial_inaccuracy * (1 - aim_percent));
        }
    }
    private bool _aim_this_frame = false;

    protected virtual void Start() {
        switch_weapons_blocked = false;
        if (initialize_weapon != null && current_gun == null) {
            current_gun = (IFirearm)Instantiate(initialize_weapon);
        }
        if (start_weapon_loaded && current_gun != null) {
            current_gun.current_ammo = current_gun.ammo_capacity;
        } else if (current_gun != null) {
            current_gun.current_ammo = 0;
        }
        UpdateSubscribers();
    }

    protected virtual void Update() {
        if (!is_active) { return; } // do nothing while controller disabled

        attacker = GetComponent<CharCtrl>();
        if (attacker == null) {
            Debug.LogWarning("attacker is null!");
        }
        UpdateRecoil();
        UpdateDebugFields();
    }

    private void UpdateRecoil() {
        float recoil_decay;
        if (current_gun == null) {
            recoil_decay = 0;
        } else {
            recoil_decay = current_gun.recoil_recovery;
        }
        if (_aim_this_frame) {
            recoil_decay *= 2;
        }
        current_recoil -= recoil_decay * Time.deltaTime;
    }

    protected void AimOnSwitch() {
        // handles aim when the player switches weapons
        if (start_aim_at == null) { return; /* not aiming, do nothing */ }
        StopAim();
        StartAim();
    }

    public void StartAim() {
        _aim_this_frame = true;
        if (start_aim_at == null) {
            start_aim_at = Time.time;
        }
    }
    public void StopAim() {
        start_aim_at = null;
    }
    public void AttackReleased(Vector3 attack_direction) {
        if (current_gun == null) {
            Debug.LogWarning("AttackReleased on null gun");
            return;
        }
        float inaccuracy = current_inaccuracy + current_recoil;
        current_gun.AttackReleased(attack_direction, attack_start_position.position, inaccuracy, attacker);
        UpdateSubscribers();
    }

    public void StartAttack(Vector3 attack_direction) {
        // fires an attack with the current weapon
        if (
            (_last_shot_at + shot_cooldown <= Time.time)
            && (!InputSystem.IsNullPoint(attack_direction))
        ) {
            if (HasAmmo()) {
                _FireAttack(attack_direction);
            } else {
                _EmptyAttack();
            }
        }
    }

    public void AttackHold(Vector3 attack_direction) {
        if (
            (_last_shot_at + shot_cooldown <= Time.time)
            && (!InputSystem.IsNullPoint(attack_direction))
        ) {
            if (HasAmmo()) {
                _FireAttack(attack_direction, hold: true);
            } else {
                _EmptyAttack();
            }
        }
    }

    private void _EmptyAttack() {
        AttackResolver.AttackEmpty(current_gun, transform.position);
    }

    private void _FireAttack(Vector3 attack_direction, bool hold = false) {
        _last_shot_at = Time.time;

        float inaccuracy = current_inaccuracy + current_recoil;
        bool attack_fired;
        if (hold) {
            attack_fired = current_gun.AttackHold(attack_direction, attack_start_position.position, inaccuracy, attacker);
        } else {
            attack_fired = current_gun.AttackClicked(attack_direction, attack_start_position.position, inaccuracy, attacker);
        }
        // TODO --- implement debug setting for not needing reload 
        if (attacker.is_player && attack_fired && GameSettings.inst.debug_settings.GetBool("no_reload")) {
            // if the attacker is the player, and has reload disabled, don't decriment ammo count 
            current_gun.current_ammo += 1;
        }
        // Bullet bullet = null;
        // for (int i = 0; i < current_gun.n_shots; i++) {
        //     GameObject bullet_obj = Instantiate(bullet_prefab);

        //     bullet_obj.transform.position = GetShootPoint(i);
        //     Vector3 velocity = GetAttackVector(attack_direction, i);
        //     bullet_obj.GetComponent<Rigidbody>().velocity = velocity;

        //     bullet = bullet_obj.GetComponent<Bullet>();
        //     bullet.weapon = current_gun;
        //     bullet.attack_damage_max = current_gun.weapon_damage_max;
        //     bullet.attack_damage_min = current_gun.weapon_damage_min;
        //     bullet.armor_effectiveness = current_gun.armor_effectiveness;
        //     bullet.damage_falloff_rate = current_gun.damage_falloff_rate;
        //     bullet.attacker = attacker;
        // }
        // current_recoil += current_gun.recoil_inaccuracy;
        // current_gun.current_ammo -= 1;
        // if (bullet != null) {
        //     AttackResolver.AttackStart(bullet, attack_direction, attack_start_point.position, is_melee_attack:false); // Only create a shot effects once for a shotgun
        // }
        // else {
        //     Debug.LogWarning("bullet is null!");
        // }
        UpdateSubscribers();
        // #if UNITY_EDITOR
        //     EditorApplication.isPaused = true;
        // #endif
    }

    // private Vector3 GetShootPoint(int i) {
    //     // takes a loop counter, and returns the start point for a bullet. If loop counter is 0, the start point is always the same, 
    //     // but if loop counter is not 0, a random offset is applied.
    //     if (i == 0) {
    //         return attack_start_point.position;
    //     }
    //     // radom variance to make shotguns feel better
    //     float variance = 0.25f;
    //     Vector3 offset = new Vector3(UnityEngine.Random.Range(0, variance), 0, UnityEngine.Random.Range(0, variance));
    //     return attack_start_point.position + offset;
    // }

    // private Vector3 GetAttackVector(Vector3 attack_direction, int i) {
    //     Vector3 base_vector = attack_direction.normalized * bullet_speed;

    //     float inaccuracy = current_inaccuracy + current_recoil;
    //     float deviation = UnityEngine.Random.Range(-inaccuracy, inaccuracy);

    //     Vector3 rotation_axis = Vector3.up;
    //     Vector3 rotated_direction = Quaternion.AngleAxis(deviation, rotation_axis) * base_vector;

    //     float multiplier;
    //     if(i == 0) {
    //         multiplier = 1;
    //     } else {
    //         multiplier = UnityEngine.Random.Range(0.9f, 1.1f);
    //     }

    //     return rotated_direction * multiplier;
    // }


    /// DEBUG CODE
    public int debug_ammo_count = 0;

    protected virtual void UpdateDebugFields() {
        debug_ammo_count = (current_gun == null) ? -1 : current_gun.current_ammo;
    }
}
