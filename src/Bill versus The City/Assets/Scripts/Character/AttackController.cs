using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour, IWeaponManager
{
    public ScriptableObject initialize_weapon;
    public IWeapon current_weapon { get; set; }
    public IAttackTarget attacker = null;

    public Transform shoot_point; // bullets start here

    public GameObject bullet_prefab;

    public bool use_full_auto = true; // use full auto with select fire weapons

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

    //////// MonoBehaviour
    void Start() {
       AttackControllerStart();
       // TODO --- make this better
       UpdateSubscribers();
    }

    void Update() {
        attacker = GetComponent<CharCtrl>();
        if (attacker == null) {
            Debug.LogWarning("attacker is null!");
        }
        AttackControllerUpdate();

        if (! _aim_this_frame) { StopAim(); }
        _aim_this_frame = false;
    }
    private float? start_aim_at = null;
    public bool is_aiming { get { return start_aim_at != null; }}
    public float current_inaccuracy { 
        get {
            if (current_weapon == null) { return -1f; }
            if (start_aim_at == null) {
                return current_weapon.initial_inaccuracy;
            }
            float aimed_for = Time.time - ((float) start_aim_at);
            float aim_percent = Mathf.Clamp(
                (aimed_for / current_weapon.time_to_aim), 0f, 1f
            );

            return (current_weapon.aimed_inaccuracy * aim_percent) + (current_weapon.initial_inaccuracy * (1 - aim_percent));
        } 
    }
    private bool _aim_this_frame = false;
    public void Aim() {
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
        if (initialize_weapon != null) {
            current_weapon = (IWeapon) Instantiate(initialize_weapon);
        }
        current_weapon.current_ammo = current_weapon.ammo_capacity;
    }

    protected virtual void AttackControllerUpdate() {
        // allow extending classes to add to `Update`
    }

    public void FireAttack(Vector3 attack_direction) {
        // fires an attack with the current weapon
        if (
            (current_weapon.current_ammo > 0)
            &&(_last_shot_at + shot_cooldown <= Time.time)
        ) {
            _FireAttack(attack_direction);
        }
    }

    private void _FireAttack(Vector3 attack_direction) {
        _last_shot_at = Time.time;
        current_weapon.current_ammo -= 1;
        GameObject bullet_obj = Instantiate(bullet_prefab) as GameObject;

        bullet_obj.transform.position = shoot_point.position;
        Vector3 velocity = GetAttackVector(attack_direction);
        bullet_obj.GetComponent<Rigidbody>().velocity = velocity;

        Bullet bullet = bullet_obj.GetComponent<Bullet>();
        bullet.attack_damage_max = current_weapon.weapon_damage_max;
        bullet.attack_damage_min = current_weapon.weapon_damage_min;
        bullet.armor_penetration = current_weapon.armor_penetration;
        bullet.attacker = attacker;

        AttackResolver.AttackStart(bullet, shoot_point.position);

        UpdateSubscribers();
    }

    private Vector3 GetAttackVector(Vector3 attack_direction) {
        Vector3 base_vector = attack_direction.normalized * bullet_speed;
        float deviation = Random.Range(-current_inaccuracy, current_inaccuracy);

        Vector3 rotation_axis = Vector3.up;
        Vector3 rotated_direction = Quaternion.AngleAxis(deviation, rotation_axis) * base_vector;
        Debug.Log("original aim_vector: " + base_vector + ", inaccuracy_vector: " + rotated_direction);
        return rotated_direction;
    }
}
