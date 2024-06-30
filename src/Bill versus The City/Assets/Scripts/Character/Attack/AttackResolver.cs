using UnityEngine;

public static class AttackResolver {

    public const string PLACEHOLDER_ATTACK_PREFAB = "MuzzelFlashEffect";
    public const string PLACEHOLDER_ATTACK_HIT_PREFAB = "BloodSplatterEffect";
    public const string PLACEHOLDER_ATTACK_MISS_PREFAB = "MuzzelFlashEffectGray";

    private static IAttackHitEffect[] DAMAGE_EFFECTS = new IAttackHitEffect[]{
        new SpawnPrefabEffect(PLACEHOLDER_ATTACK_HIT_PREFAB)
    };

    private static IAttackShootEffect[] SHOOT_EFFECTS = new IAttackShootEffect[]{
        new SpawnPrefabEffect(PLACEHOLDER_ATTACK_PREFAB)
    };

    private static IAttackMissEffect[] MISS_EFFECTS = new IAttackMissEffect[]{
        new SpawnPrefabEffect(PLACEHOLDER_ATTACK_MISS_PREFAB)
    };

    public static void ResolveAttackHit(IAttack attack, 
            IAttackTarget target, Vector3 hit_location) {
        ICharacterStatus status = target.GetStatus();
        float attack_damage = Random.Range(attack.attack_damage_min, 
                attack.attack_damage_max);
        status.health -= attack_damage;
        // TODO --- implement armor

        
        foreach (IAttackHitEffect effect in DAMAGE_EFFECTS) {
            effect.DisplayDamageEffect(
                target.GetHitTarget(), hit_location, attack_damage);
        }
    }

    public static void AttackMiss(IAttack attack, Vector3 location) {

        foreach (IAttackMissEffect effect in MISS_EFFECTS) {
            effect.DisplayEffect(location);
        }
    }

    public static void AttackStart(IAttack attack, Vector3 location) {
        // Displays attack effects for firing a weapon

        foreach (IAttackShootEffect effect in SHOOT_EFFECTS) {
            effect.DisplayEffect(location);
        }
    }
}