using System;
using UnityEngine;

public static class AttackResolver {

    public const string PLACEHOLDER_ATTACK_PREFAB = "MuzzelFlashEffect";
    public const string PLACEHOLDER_ATTACK_HIT_PREFAB = "BloodSplatterEffect";
    public const string PLACEHOLDER_ATTACK_MISS_PREFAB = "MuzzelFlashEffectGray";
    public const string PLACEHOLDER_DAMAGE_SOUND_EFFECT = "damage_chiptone";
    public const string PLACEHOLDER_GUNSHOT = "gunshot";

    private static IAttackHitEffect[] DAMAGE_EFFECTS = new IAttackHitEffect[]{
        new SpawnPrefabEffect(PLACEHOLDER_ATTACK_HIT_PREFAB),
        new SoundEffect(PLACEHOLDER_DAMAGE_SOUND_EFFECT)
    };

    private static IAttackHitEffect[] DEBUG_DAMAGE_EFFECTS = new IAttackHitEffect[]{
        new SpawnDamageNumberEffect()
    };

    private static IAttackShootEffect[] SHOOT_EFFECTS = new IAttackShootEffect[]{
        new SpawnPrefabEffect(PLACEHOLDER_ATTACK_PREFAB),
        new SoundEffect(PLACEHOLDER_DAMAGE_SOUND_EFFECT)
    };
    

    private static IAttackShootEffect[] DEBUG_SHOOT_EFFECTS = new IAttackShootEffect[]{};

    private static IAttackMissEffect[] MISS_EFFECTS = new IAttackMissEffect[]{
        new SpawnPrefabEffect(PLACEHOLDER_ATTACK_MISS_PREFAB)
    };
    private static IAttackMissEffect[] DEBUG_MISS_EFFECTS = new IAttackMissEffect[]{};

    public static void ResolveAttackHit(IAttack attack, 
            IAttackTarget target, Vector3 hit_location) {
        ICharacterStatus status = target.GetStatus();
        float attack_damage;
        
        if (attack.ignore_armor || status.armor == null) {
            attack_damage = ResolveGunDamageUnarmored(attack, status);
        }
        else if (status.armor.armor_durability <= 0) {
            attack_damage = ResolveGunDamageUnarmored(attack, status);
        }
        else {
            attack_damage = ResolveGunDamageArmored(attack, status, hit_location);
        }
        
        foreach (IAttackHitEffect effect in GetDamageEffects()) {
            effect.DisplayDamageEffect(
                target.GetHitTarget(), hit_location, attack_damage);
        }
        target.OnAttackHitRecieved(attack);
        attack.attacker.OnAttackHitDealt(attack, target);
    }

    private static IAttackHitEffect[] GetDamageEffects() {
        if (!DebugMode.inst.debug_enabled) {
            return DAMAGE_EFFECTS;
        }
        return ConcatinateArrays(DAMAGE_EFFECTS, DEBUG_DAMAGE_EFFECTS);
    }

    private static IAttackShootEffect[] GetShootEffects() {
        if (!DebugMode.inst.debug_enabled) {
            return SHOOT_EFFECTS;
        }
        return ConcatinateArrays(SHOOT_EFFECTS, DEBUG_SHOOT_EFFECTS);
    }

    private static IAttackMissEffect[] GetMissEffects() {
        if (!DebugMode.inst.debug_enabled) {
            return MISS_EFFECTS;
        }
        return ConcatinateArrays(MISS_EFFECTS, DEBUG_MISS_EFFECTS);
    }

    private static T[] ConcatinateArrays<T>(T[] firstArray, T[] secondArray){
        T[] result = new T[firstArray.Length + secondArray.Length];
        Array.Copy(firstArray, result, firstArray.Length);
        // Copy the second array to the result array, starting at the end of the first array
        Array.Copy(secondArray, 0, result, firstArray.Length, secondArray.Length);
        return result;
    }

    public static float ResolveGunDamageUnarmored(IAttack attack, 
            ICharacterStatus status) {
        float attack_damage = RandomDamage(attack);
        status.health -= attack_damage;
        return attack_damage;
    }

    public static float RandomDamage(IAttack attack) {
        return Mathf.Max(1f, UnityEngine.Random.Range(attack.attack_damage_min, 
                attack.attack_damage_max));
    } 

    public static float CalculateDamageSplit(IAttack attack, IArmor armor) {
        float og_split = armor.armor_protection - attack.armor_penetration;
        float split = Mathf.Max(0.95f, Mathf.Min(0.05f, og_split));
        if (og_split != split) {
            StaticLogger.Warning($"unbalanced damage split: {og_split} => {split}");
        }
        return split;
    }

    public static (float, float, float) CalculateArmorDamage(IAttack attack, IArmor armor) {
        float total_attack_damage = RandomDamage(attack);
        float damage_split = CalculateDamageSplit(attack, armor);
        float attack_damage = total_attack_damage * (1 - damage_split);
        float base_armor_damage = total_attack_damage * damage_split;
        float armor_damage = base_armor_damage;

        attack_damage = Mathf.Max(1, attack_damage);
        armor_damage = Mathf.Max(1, armor_damage);
      
        // StaticLogger.Log($"base_attack_damage: {total_attack_damage}, \ndamage_split: {damage_split}, \nattack_damage: {attack_damage}, \nbase_armor_damage: {base_armor_damage}, \narmor_damage: {armor_damage}");
        // StaticLogger.Log($"{total_attack_damage} => {attack_damage} / {base_armor_damage}");
        if (total_attack_damage <= 0) { Debug.LogWarning($"negative total_attack_damage: {total_attack_damage}"); } 
        if (attack_damage <= 0) { Debug.LogWarning($"negative attack_damage: {attack_damage}"); } 
        if (armor_damage <= 0) { Debug.LogWarning($"negative armor_damage: {armor_damage}"); } 
        return (total_attack_damage, attack_damage, armor_damage);
    }

    public static float ResolveGunDamageArmored(IAttack attack, 
            ICharacterStatus status, Vector3 hit_location) {
        (float total_attack_damage, float attack_damage, float armor_damage) = CalculateArmorDamage(attack, status.armor);
        float overflow_damage;
        float armor_before = status.armor.armor_durability; // TODO --- remove debug code
        if (armor_damage > status.armor.armor_durability) {
            overflow_damage = armor_damage - status.armor.armor_durability;
            status.armor.armor_durability = 0;
            ResolveArmorBreak(attack, status, hit_location);
        } else {
            overflow_damage = 0;
            status.armor.armor_durability -= armor_damage;
        }
        status.health -= attack_damage + overflow_damage;
        if(total_attack_damage < (attack_damage + overflow_damage)) {
            StaticLogger.Warning($"Overflow damage more than base: {total_attack_damage} => {attack_damage} + {overflow_damage}");
        }
        if (armor_before >= status.armor.armor_durability) {
            StaticLogger.Warning($"negative damage ({armor_damage})! {armor_before} --> {status.armor.armor_durability}");
        }    
        return attack_damage;
    }

    public static void ResolveArmorBreak(IAttack attack, 
            ICharacterStatus status, Vector3 hit_location) {
        // StaticLogger.Log($"Armor '{status.armor}' Broken by {attack}!!!");
        // TODO !
    }

    public static void AttackMiss(IAttack attack, Vector3 location) {

        foreach (IAttackMissEffect effect in GetMissEffects()) {
            effect.DisplayEffect(location);
        }
    }

    public static void AttackStart(IAttack attack, Vector3 location) {
        // Displays attack effects for firing a weapon

        foreach (IAttackShootEffect effect in GetShootEffects()) {
            effect.DisplayEffect(location);
        }
    }
}