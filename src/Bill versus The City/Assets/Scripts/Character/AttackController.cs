using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour, IWeaponManager
{
    // public const float RECOIL_DECAY = 15f;
    // public const float MAX_RECOIL = 15f;
    public ScriptableObject initialize_weapon;
    public IWeapon current_weapon { get; set; }
    public IAttackTarget attacker = null;

    public bool start_weapon_loaded = true;

    public Transform shoot_point; // bullets start here

    public GameObject bullet_prefab;

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
            if(current_weapon == null) {
                Debug.LogWarning("no weapon selected!");
                return 35f;
            }
            return current_weapon.bullet_speed;
        }
    }
    // how long must pass from the previous shot before you can shoot again
    public float shot_cooldown {
        get {
            if (current_weapon == null) {
                Debug.LogWarning("no weapon selected!");
                return 0.5f;
            }
            if (current_weapon.auto_fire) {
                return current_weapon.full_auto_fire_rate;
            }
            return current_weapon.semi_auto_fire_rate;
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
            sub.UpdateWeapon(null, current_weapon);
        }
    }

    public virtual int? current_slot {
        get { return null; }
    }

    private float? start_aim_at = null;
    public bool is_aiming { get { return start_aim_at != null; }}

    public float aim_percent {
        get {
            if (start_aim_at == null) { return 0f; }
            
            float aimed_for = Time.time - ((float) start_aim_at);

            float time_to_aim;
            if (current_weapon == null) {
                time_to_aim = float.PositiveInfinity;
            } else {
                time_to_aim = current_weapon.time_to_aim;
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
            }
            else if (value >= current_weapon.recoil_max) {
                _current_recoil = current_weapon.recoil_max;
            }
            else {
                _current_recoil = value;
            }
        }
    }

    public float current_inaccuracy { 
        get {
            if (current_weapon == null) { return -1f; }
            if (start_aim_at == null) {
                return current_weapon.initial_inaccuracy;
            }
            return (current_weapon.aimed_inaccuracy * aim_percent) + (current_weapon.initial_inaccuracy * (1 - aim_percent));
        } 
    }
    private bool _aim_this_frame = false;

    void Start() {
       AttackControllerStart();
       // TODO --- make this better
       UpdateSubscribers();
    }

    void Update() {
        if (! is_active) { return; } // do nothing while controller disabled
        
        attacker = GetComponent<CharCtrl>();
        if (attacker == null) {
            Debug.LogWarning("attacker is null!");
        }
        AttackControllerUpdate();
        UpdateRecoil();
        // if (! _aim_this_frame) { StopAim(); }
        // _aim_this_frame = false;
        UpdateDebugFields();
    }

    private void UpdateRecoil() {
        float recoil_decay;
        if (current_weapon == null) {
            recoil_decay = 0;
        } else {
            recoil_decay = current_weapon.recoil_recovery;
        }
        if (_aim_this_frame) {
            recoil_decay *= 2;
        }
        current_recoil -= (recoil_decay * Time.deltaTime);


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

    protected virtual void AttackControllerStart() {
         // allow extending classes to add to `Start`    
        if (initialize_weapon != null && current_weapon == null) {
            current_weapon = (IWeapon) Instantiate(initialize_weapon);
        }
        if (start_weapon_loaded) {
            current_weapon.current_ammo = current_weapon.ammo_capacity;
        } else {
            current_weapon.current_ammo = 0;
        }
    }

    protected virtual void AttackControllerUpdate() {
        // allow extending classes to add to `Update`
    }

    public void FireAttack(Vector3 attack_direction) {
        // fires an attack with the current weapon
        if (
            (_last_shot_at + shot_cooldown <= Time.time)
            &&(! InputSystem.IsNullPoint(attack_direction))
        ) {
            if (current_weapon.current_ammo > 0) {
                _FireAttack(attack_direction);
            }
            else {
                _EmptyAttack();
            }
        }
    }

    private void _EmptyAttack() {
        AttackResolver.AttackEmpty(current_weapon, transform.position);
    }

    private void _FireAttack(Vector3 attack_direction) {
        _last_shot_at = Time.time;
        Bullet bullet = null;
        for (int i = 0; i < current_weapon.n_shots; i++) {
            GameObject bullet_obj = Instantiate(bullet_prefab) as GameObject;

            bullet_obj.transform.position = GetShootPoint(i);
            Vector3 velocity = GetAttackVector(attack_direction, i);
            bullet_obj.GetComponent<Rigidbody>().velocity = velocity;

            bullet = bullet_obj.GetComponent<Bullet>();
            bullet.weapon = current_weapon;
            bullet.attack_damage_max = current_weapon.weapon_damage_max;
            bullet.attack_damage_min = current_weapon.weapon_damage_min;
            bullet.armor_effectiveness = current_weapon.armor_effectiveness;
            bullet.damage_falloff_rate = current_weapon.damage_falloff_rate;
            bullet.attacker = attacker;
        }
        current_recoil += current_weapon.recoil_inaccuracy;
        current_weapon.current_ammo -= 1;
        if (bullet != null) {
            AttackResolver.AttackStart(bullet, shoot_point.position); // Only create a shot effects once for a shotgun
        }
        else {
            Debug.LogWarning("bullet is null!");
        }
        UpdateSubscribers();
    }

    private Vector3 GetShootPoint(int i) {
        // takes a loop counter, and returns the start point for a bullet. If loop counter is 0, the start point is always the same, 
        // but if loop counter is not 0, a random offset is applied.
        if (i == 0) {
            return shoot_point.position;
        }
        // radom variance to make shotguns feel better
        float variance = 0.25f;
        Vector3 offset = new Vector3(Random.Range(0, variance), 0, Random.Range(0, variance));
        return shoot_point.position + offset;
    }

    private Vector3 GetAttackVector(Vector3 attack_direction, int i) {
        Vector3 base_vector = attack_direction.normalized * bullet_speed;

        float inaccuracy = current_inaccuracy + current_recoil;
        float deviation = Random.Range(-inaccuracy, inaccuracy);

        Vector3 rotation_axis = Vector3.up;
        Vector3 rotated_direction = Quaternion.AngleAxis(deviation, rotation_axis) * base_vector;

        float multiplier;
        if(i == 0) {
            multiplier = 1;
        } else {
            multiplier = Random.Range(0.9f, 1.1f);
        }

        return rotated_direction * multiplier;
    }


    /// DEBUG CODE
    public int debug_ammo_count = 0;

    protected virtual void UpdateDebugFields() {
        debug_ammo_count = (current_weapon == null)? -1 : current_weapon.current_ammo;
    }
}
