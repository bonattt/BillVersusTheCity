using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeleeAttackController : MonoBehaviour, IAttackController {
    public IWeapon current_weapon { get => current_melee; set { current_melee = (IMeleeWeapon)value; } }
    public IFirearm current_gun {
        get => null;
        set { throw new NotImplementedException(); }
    }
    public MeleeWeapon _init_weapon;
    public IMeleeWeapon _current_melee;
    public IMeleeWeapon current_melee { 
        get => _current_melee;
        set {
            Debug.LogWarning($"set current_melee: '{_current_melee}' => '{value}'"); // TODO --- remove debug
            _current_melee = value;
        }
    }
    public bool switch_weapons_blocked { get; set; }

    public float total_attack_time { get => ((IMeleeWeapon)current_weapon).total_attack_time; }
    public IAttackTarget attacker = null;

    public MeleeAttack current_attack;

    public Transform _attack_start_point;
    public Transform attack_start_point { get => _attack_start_point; }

    // set the script to be inactive while game is paused
    private bool _is_active = true;
    public bool is_active {
        get { return _is_active; }
        set {
            // avoids shooting because you clicked on the last frame of dialouge
            _is_active = value;
        }
    }

    // tracks when the last shot was fired, for handling rate-of-fire
    private float _last_attack_at = -10f;
    [SerializeField]
    private Vector3 attack_direction;

    public virtual int? current_slot {
        get { return null; }
    }

    public void StartAim() { /* do nothing */ }
    public void StopAim() { /* do nothing */ }
    public float aim_percent { get => 0f; }

    public bool CanAttack() {
        return current_melee != null && attack_stage == MeleeAttackStage.none && is_active;
    }

    void Start() {
        attacker = GetComponent<IAttackTarget>();
        if (attacker == null) Debug.LogError("attacker is null!!");
        current_melee = _init_weapon.CopyMeleeWeapon();
    }

    protected virtual void Update() {
        if (!is_active) { return; } // do nothing while controller disabled

        attacker = GetComponent<CharCtrl>();
        if (attacker == null) {
            Debug.LogWarning("attacker is null!");
        }
        UpdateAttackInProgress();
        UpdateDebug();
    }

    public MeleeAttackStage attack_stage = MeleeAttackStage.none;
    private void UpdateAttackInProgress() {
        MeleeAttackStage next_stage = GetNextState();
        if (attack_stage == MeleeAttackStage.attack || next_stage == MeleeAttackStage.attack) {
            ResolveAttack();
        }
        attack_stage = next_stage;
        if (current_attack != null && Time.time > _last_attack_at + current_attack.melee_weapon.total_attack_time) {
            current_attack = null;
        }
    }

    private MeleeAttackStage GetNextState() {
        float windup_until = _last_attack_at + current_melee.attack_windup;
        float attack_until = windup_until + current_melee.attack_duration;
        float recovery_until = attack_until + current_melee.attack_recovery;
        float cooldown_until = recovery_until + current_melee.attack_cooldown;
        if (Time.time < _last_attack_at) {
            return MeleeAttackStage.none; // no attack started
        }
        if (Time.time > _last_attack_at && Time.time < windup_until) {
            return MeleeAttackStage.windup;
        } else if (Time.time > windup_until && Time.time < attack_until) {
            return MeleeAttackStage.attack;
        } else if (Time.time > attack_until && Time.time < recovery_until) {
            return MeleeAttackStage.recovery;
        } else if (Time.time > recovery_until && Time.time < cooldown_until) {
            return MeleeAttackStage.cooldown;
        } else if (Time.time > cooldown_until) {
            return MeleeAttackStage.none;
        }
        // Debug.LogError($"Unexpected attack timing state. time: {Time.time}, _last_attack_at: {_last_attack_at}");
        return MeleeAttackStage.none;
    }

    private void ResolveAttack() {
        // checks if an active attack is hitting anything, and deals damage to anything hit by the attack
        Debug.LogWarning("ResolveAttack!!"); // TODO --- remove debug
        RaycastHit[] melee_hits = Physics.RaycastAll(attack_start_point.position, attack_direction, current_melee.attack_reach);
        Debug.DrawRay(attack_start_point.position, attack_direction.normalized * current_melee.attack_reach, Color.green);
        foreach (RaycastHit hit in melee_hits) {
            IAttackTarget hit_target = hit.collider.gameObject.GetComponent<IAttackTarget>();
            if (hit_target != null && !current_attack.hit_targets.Contains(hit_target)) {
                current_attack.hit_targets.Add(hit_target);
                AttackResolver.ResolveAttackHit(current_attack, hit_target, hit.point);
            }
        }
    }
    public void AttackReleased(Vector3 attack_direction) {
        // do nothing
    }

    public void AttackHold(Vector3 attack_direction) {
        StartAttack(attack_direction);
    }
    public void StartAttack(Vector3 new_attack_direction) {
        // fires an attack with the current weapon
        if (
            (_last_attack_at + total_attack_time < Time.time)
            && (!InputSystem.IsNullPoint(new_attack_direction))
        ) {
            _StartAttack(new_attack_direction);
        }
    }

    private void _StartAttack(Vector3 new_attack_direction, bool hold = false)
    {
        float inaccuracy = 0f;
        if (
            (_last_attack_at + total_attack_time<Time.time)
            && (!InputSystem.IsNullPoint(new_attack_direction))
        ) {
            if (hold) {
                current_melee.AttackClicked(new_attack_direction, attack_start_point.position, inaccuracy, attacker);
            } else {
                current_melee.AttackHold(new_attack_direction, attack_start_point.position, inaccuracy, attacker);
            }
        }
        if (current_attack == null) current_attack = new MeleeAttack();
        else Debug.LogError("attack made while current attack isn't nulled yet!");
        current_attack.melee_weapon = current_melee;
        current_attack.ignore_armor = false;
        current_attack.hit_targets.Add(attacker); // prevent from hitting self with melee attack

        _last_attack_at = Time.time;
        this.attack_direction = new_attack_direction;
        AttackResolver.AttackStart(current_attack, this.attack_direction, attack_start_point.position, is_melee_attack: true);

        // // TODO --- refactor, make this an effect
        // GameObject prefab = (GameObject)Resources.Load(AttackResolver.PLACEHOLDER_MELEE_EFFECTS_PREFAB);
        // GameObject effect = Instantiate(prefab);
        // effect.transform.position = attack_start_point.position;
        // effect.transform.rotation = transform.rotation;
        // Debug.LogWarning("TODO --- melee attack hard codes visual effect!! should be an effect");
        // // TODO ---
    }

    ////////// DEBUG //////////

    public string debug__current_attack = "";
    void UpdateDebug()
    {
        if (current_attack == null) { debug__current_attack = ""; }
        else { debug__current_attack = $"attacl: {current_attack}"; }
    }
}


public enum MeleeAttackStage
{
    none,
    windup,
    attack,
    recovery,
    cooldown
}