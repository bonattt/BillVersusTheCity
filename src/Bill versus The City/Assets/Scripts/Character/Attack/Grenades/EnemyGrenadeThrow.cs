

using UnityEditor;
using UnityEngine;

public class EnemyGrenadeThrow : MonoBehaviour, IPerceptionSubscriber
{
    /* script for handling an enemy */
    public bool throw_on_next_sighting;
    public ThrownAttack grenade_attack;
    public EnemyPerception perception;

    private bool infinite_ammo = true;

    private bool release_throw_next_frame = false;

    private IAttackTarget _attacker;
    public IAttackTarget attacker {
        get {
            if (_attacker == null) {
                _attacker = GetComponent<IAttackTarget>();
            }
            return _attacker;
        }
    }
    

    private float _threw_at = float.NegativeInfinity;
    public bool is_throwing { 
        // if true, the script is still on cooldown from making a grenade throw.
        get => grenade_attack == null ? false : _threw_at + grenade_attack.semi_auto_fire_rate < Time.time;
    }

    private GameObject grenade_thrown;

    void Start() {
        perception.Subscribe(this);
        grenade_attack.current_ammo = grenade_attack.ammo_capacity;
    }

    public void UpdatePerceptionState(PerceptionState previous_state, PerceptionState new_state) {
        if (throw_on_next_sighting && new_state == PerceptionState.seeing && previous_state != PerceptionState.seeing) {
            ThrowGrenadeAt(PlayerCharacter.inst.player_transform);
            throw_on_next_sighting = false;
        }
    }
    
    private Vector3 attack_direction;
    public void ThrowGrenadeAt(ManualCharacterMovement player) => ThrowGrenadeAt(player.transform.position);
    public void ThrowGrenadeAt(Transform t) => ThrowGrenadeAt(t.position);
    public void ThrowGrenadeAt(Vector3 position) {
        if (grenade_attack.current_ammo <= 0 && !infinite_ammo) {
            Debug.LogWarning($"no ammo for enemy grenade throw! {grenade_attack.current_ammo} / {grenade_attack.ammo_capacity}");
            return;
        } 
        if (infinite_ammo) { grenade_attack.current_ammo += 1; } // if ammo is infinite, increment ammo by 1 before attacking
        Vector3 attack_start_point = transform.position;
        attack_direction = position - attack_start_point;
        float inaccuracy = 0;
        grenade_attack.throw_target_position = ThrowTargetPosition.player_position;
        grenade_attack.AttackClicked(attack_direction, attack_start_point, inaccuracy, attacker);
        release_throw_next_frame = true;
    }

    void Update() {
        if (release_throw_next_frame) {
            release_throw_next_frame = false;
            Vector3 attack_start_point = transform.position;
            float inaccuracy = 0;
            grenade_attack.AttackReleased(attack_direction, attack_start_point, inaccuracy, attacker);
            grenade_thrown = grenade_attack.last_projectile;
            _threw_at = Time.time;
        }
    }

}