using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamageCalculator : MonoBehaviour
{
    public ExplosionAttack explosion;
    public ManualCharacterMovement player;

    private ICharacterStatus player_health;

    public bool will_kill;

    public float distance_to_player, damage_to_player, max_health, current_health, kill_range;

    public List<string> damage_output;
    public List<string> falloff_output;

    public static readonly List<int> ranges = new List<int>(){
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10
    };

    void Start() {
        player_health = player.GetComponent<ICharacterStatus>();
        if (player_health == null) { Debug.LogError("Player Health missing!"); }
        explosion.attack_from = Vector3.zero;
        damage_output = GetDamageOutput(explosion);
        falloff_output = GetFalloffOutput(explosion);
    }

    public static List<string> GetDamageOutput(IAttack attack) {
        List<string> damage_output = new List<string>();
        foreach(int r in ranges) {
            string range_str = r >= 10 ? $"{r}" : $"0{r}";
            Vector3 hit_location = new Vector3(r, 0, 0);  // attack_from set to <0,0,0>
            float min = AttackResolver.MinDamage(attack, hit_location);
            float mean = AttackResolver.MeanDamage(attack, hit_location);
            float max = AttackResolver.MaxDamage(attack, hit_location);
            damage_output.Add($"{range_str}: {min} / {mean} / {max}");
            if (max <= 1f) {
                break;
            }
        }
        return damage_output;
    }

    public static List<string> GetFalloffOutput(IAttack attack) {
        List<string> damage_output = new List<string>();
        foreach(int r in ranges) {
            string range_str = r >= 10 ? $"{r}" : $"0{r}";
            Vector3 hit_location = new Vector3(r, 0, 0);  // attack_from set to <0,0,0>
            float falloff = AttackResolver.GetDamageFalloff(attack, hit_location);
            damage_output.Add($"{range_str}: {falloff}");
        }
        return damage_output;
    }

    public static float GetDamageFalloff(ExplosionAttack explosion, float range) {
        return AttackResolver.GetDamageFalloff(explosion, range);
    }

    public float DistanceToKill() {
        UpdatePlayerHealth();
        // calculates at what distance the grenade will kill the player in one hit
        float e = Mathf.Exp(1); // constant `e` ~2.7182...
        return Mathf.Log(max_health / explosion.damage_falloff_rate, e); // natural log of (health / rate)
    }

    void Update() {
        CheckPlayerDamage();
    }

    private void CheckPlayerDamage() {
        kill_range = DistanceToKill();
        distance_to_player = PhysicsUtils.FlatDistance(player.transform.position, transform.position);
        explosion.attack_from = transform.position;
        damage_to_player = AttackResolver.MeanDamage(explosion, player.transform.position);
        will_kill = DamageWillKill(damage_to_player);
        DrawLineToPlayer();
    }

    public bool DamageWillKill(float damage) {
        UpdatePlayerHealth();
        return damage >= current_health;
    }


    private void UpdatePlayerHealth() {
        if (player_health.armor != null) {
            max_health = player_health.max_health + player_health.armor.armor_max_durability;
            current_health = player_health.health + player_health.armor.armor_durability;
        } else {
            max_health = player_health.max_health;
            current_health = player_health.health;
        }
    }

    private void DrawLineToPlayer() {
        Color line_color;
        if (DamageWillKill(damage_to_player)) {
            line_color = Color.red;
        } else if (damage_to_player > 150) {
            line_color = new Color(1f, 0.65f, 0); // RGB Orange
        } else if (damage_to_player > 75f) {
            line_color = Color.yellow;
        } else if (damage_to_player > 25f) {
            line_color = Color.cyan;
        } else if (damage_to_player > 1f) {
            line_color = Color.gray;
        } else {
            return; // do not draw
        }
        Vector3 draw_to = player.transform.position;
        draw_to = new Vector3(draw_to.x, transform.position.y, draw_to.z);
        Debug.DrawLine(transform.position, draw_to, line_color);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.33f);
    }
}
