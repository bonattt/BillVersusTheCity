using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeleeAttackController : MonoBehaviour
{
    public MeleeWeapon current_weapon;

    public float total_attack_time { get => ((IMeleeWeapon)current_weapon).total_attack_time; }
    public IAttackTarget attacker = null;

    public MeleeAttack current_attack;

    public float inaccuracy_modifier = 0f;

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
    private float _last_attack_at = 0f;
    private Vector3 attack_direction; 
    
    public virtual int? current_slot
    {
        get { return null; }
    }

    protected virtual void Update()
    {
        if (!is_active) { return; } // do nothing while controller disabled

        attacker = GetComponent<CharCtrl>();
        if (attacker == null)
        {
            Debug.LogWarning("attacker is null!");
        }
        UpdateAttackInProgress();
    }

    public MeleeAttackStage attack_stage = MeleeAttackStage.none;
    private void UpdateAttackInProgress()
    {
        MeleeAttackStage next_stage = GetNextState();
        if (attack_stage != next_stage) { Debug.LogWarning($"attack state {attack_stage} --> {next_stage}"); } // TODO --- remove debug

        if (attack_stage == MeleeAttackStage.attack || next_stage == MeleeAttackStage.attack) {
            ResolveAttack();
        }
        attack_stage = next_stage;
        if (attack_stage == MeleeAttackStage.none)
        {
            current_attack = null;
            attack_direction = Vector3.zero;
        }
    }

    private MeleeAttackStage GetNextState() {
        float windup_until = _last_attack_at + current_weapon.attack_windup;
        float attack_until = windup_until + current_weapon.attack_duration;
        float recovery_until = attack_until + current_weapon.attack_recovery;
        float cooldown_until = recovery_until + current_weapon.attack_cooldown;
        if (Time.time < _last_attack_at) {
            return MeleeAttackStage.none; // no attack started
        } if (Time.time > _last_attack_at && Time.time < windup_until) {
            return MeleeAttackStage.windup; 
        } else if (Time.time > windup_until && Time.time < attack_until) {
            return MeleeAttackStage.attack;
        } else if (Time.time > attack_until && Time.time < recovery_until) {
            return MeleeAttackStage.recovery;
        } else if (Time.time > recovery_until && Time.time < cooldown_until) {
            return MeleeAttackStage.cooldown;
        } else if (Time.time > recovery_until && Time.time < cooldown_until) {
            return MeleeAttackStage.none;
        }
        Debug.LogError($"attack timing state");
        return MeleeAttackStage.none;
    }

    private void ResolveAttack()
    {
        RaycastHit hit;
        if (Physics.Raycast(attack_start_point.position, attack_direction, out hit, current_weapon.attack_reach))
        {
            IAttackTarget hit_target = hit.collider.gameObject.GetComponent<IAttackTarget>();
            if (hit_target != null && !current_attack.hit_targets.Contains(hit_target))
            {
                AttackResolver.ResolveAttackHit(current_attack, hit_target, hit.point);
                current_attack.hit_targets.Add(hit_target);
                Debug.LogWarning($"melee attack hit {hit_target}"); // TODO --- remove debug
            }
        }
    }

    public void FireAttack(Vector3 attack_direction)
    {
        // fires an attack with the current weapon
        if (
            (_last_attack_at + total_attack_time <= Time.time)
            && (!InputSystem.IsNullPoint(attack_direction))
        )
        {
            _FireAttack(attack_direction);
        }
    }

    private void _FireAttack(Vector3 attack_direction)
    {
        current_attack = new MeleeAttack();
        current_attack.melee_weapon = current_weapon;
        current_attack.ignore_armor = false;

        _last_attack_at = Time.time;
        this.attack_direction = attack_direction;
        AttackResolver.AttackStart(current_attack, attack_start_point.position);
        // TODO --- trigger attack effects
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