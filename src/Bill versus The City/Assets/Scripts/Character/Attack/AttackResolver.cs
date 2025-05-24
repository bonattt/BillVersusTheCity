using System;
using UnityEngine;

public static class AttackResolver {

    public const string PLACEHOLDER_ATTACK_PREFAB = "MuzzelFlashEffect";
    public const string PLACEHOLDER_ATTACK_HIT_PREFAB = "BloodSplatterEffect";
    public const string PLACEHOLDER_ATTACK_MISS_PREFAB = "MuzzelFlashEffectGray";
    public const string PLACEHOLDER_MELEE_EFFECTS_PREFAB = "MeleeAttackEffect";
    public const string PLACEHOLDER_DAMAGE_SOUND_EFFECT = "damage_chiptone";
    public const string EMPTY_GUNSHOT_SOUND_PATH = "empty_gunshot";
    public const string DEFAULT_GUNSHOT = "gunshot_default";
    public const string DEFAULT_MELEE_SOUND = "melee_default";

    private static IAttackHitEffect[] DAMAGE_EFFECTS = new IAttackHitEffect[]{
        new SpawnPrefabEffect(PLACEHOLDER_ATTACK_HIT_PREFAB),
        new SoundEffect(PLACEHOLDER_DAMAGE_SOUND_EFFECT),
        // new AlertEnemiesLineOfSightEffect(),
        new HearableGunshot(),
    };

    private static IAttackHitEffect[] DEBUG_DAMAGE_EFFECTS = new IAttackHitEffect[]{
        new SpawnDamageNumberEffect(),
        new SpawnDamageNumberEffect(true, Color.gray, Color.black)
    };

    private static IAttackShootEffect[] SHOOT_EFFECTS = new IAttackShootEffect[]{
        new SpawnPrefabEffect(PLACEHOLDER_ATTACK_PREFAB),
        new GunshotSoundEffect(),
        new HearableGunshot(),
        // new AlertEnemiesLineOfSightEffect()
    };

    private static IAttackShootEffect[] DEBUG_SHOOT_EFFECTS = new IAttackShootEffect[] { };

    private static IMeleeAttackEffect[] MELEE_EFFECTS = new IMeleeAttackEffect[]{
        new SpawnPrefabEffect(PLACEHOLDER_MELEE_EFFECTS_PREFAB), // TODO --- implement rotation in effects, so this can be an effect again
        new MeleeAttackSoundEffect(),
    };
    private static IMeleeAttackEffect[] DEBUG_MELEE_EFFECTS = new IMeleeAttackEffect[] { };

    private static IAttackMissEffect[] MISS_EFFECTS = new IAttackMissEffect[]{
        new SpawnPrefabEffect(PLACEHOLDER_ATTACK_MISS_PREFAB),
        new AlertEnemiesLineOfSightEffect(),
        new HearableGunshot(),
    };
    private static IAttackMissEffect[] DEBUG_MISS_EFFECTS = new IAttackMissEffect[]{};
    private static IFirearmEffect[] EMPTY_SHOOT_EFFECT = new IFirearmEffect[]{
        new EmptyGunshotSoundEffect()
    };
    private static IFirearmEffect[] DEBUG_EMPTY_SHOOT_EFFECT = new IFirearmEffect[]{};

    public static void ResolveAttackHit(IAttack attack, 
            IAttackTarget target, Vector3 hit_location) {
        ICharacterStatus status = target.GetStatus();
        float health_damage;
        float armor_damage = 0f;
        
        if (attack.ignore_armor || status.armor == null) {
            health_damage = ResolveGunDamageUnarmored(attack, status);
        }
        else if (status.armor.armor_durability <= 0) {
            health_damage = ResolveGunDamageUnarmored(attack, status);
        }
        else {
            (health_damage, armor_damage) = ResolveGunDamageArmored(attack, status, hit_location);
        }
        attack.final_health_damage = health_damage;
        attack.final_armor_damage = armor_damage;
        foreach (IAttackHitEffect effect in GetDamageEffects()) {
            effect.DisplayDamageEffect(
                target.GetHitTarget(), hit_location, attack);
        }
        target.OnAttackHitRecieved(attack);
        attack.attacker.OnAttackHitDealt(attack, target);
    }

    private static IAttackHitEffect[] GetDamageEffects() {
        if (!GameSettings.inst.debug_settings.show_damage_numbers) {
            return DAMAGE_EFFECTS;
        }
        return ConcatinateArrays(DAMAGE_EFFECTS, DEBUG_DAMAGE_EFFECTS);
    }

    private static IAttackShootEffect[] GetShootEffects() {
        if (!GameSettings.inst.debug_settings.show_damage_numbers) {
            return SHOOT_EFFECTS;
        }
        return ConcatinateArrays(SHOOT_EFFECTS, DEBUG_SHOOT_EFFECTS);
    }

    private static IMeleeAttackEffect[] GetMeleeEffects() {
        if (!GameSettings.inst.debug_settings.show_damage_numbers) {
            return MELEE_EFFECTS;
        }
        return ConcatinateArrays(MELEE_EFFECTS, DEBUG_MELEE_EFFECTS);
    }

    private static IAttackMissEffect[] GetMissEffects() {
        if (!GameSettings.inst.debug_settings.show_damage_numbers) {
            return MISS_EFFECTS;
        }
        return ConcatinateArrays(MISS_EFFECTS, DEBUG_MISS_EFFECTS);
    }

    private static IFirearmEffect[] GetEmptyShotEffects() {
        if (!GameSettings.inst.debug_settings.show_damage_numbers) {
            return EMPTY_SHOOT_EFFECT;
        }
        return ConcatinateArrays(EMPTY_SHOOT_EFFECT, DEBUG_EMPTY_SHOOT_EFFECT);
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
        float random_damage = UnityEngine.Random.Range(attack.attack_damage_min, attack.attack_damage_max);
        random_damage -= attack.damage_falloff;
        return Mathf.Max(1f, random_damage);
    } 

    public static float CalculateDamageSplit(IAttack attack, IArmor armor) {
        float og_split = armor.armor_protection - attack.armor_effectiveness;
        float split = Mathf.Max(0.95f, Mathf.Min(0.05f, og_split));
        if (og_split != split) {
            // Debug.LogWarning($"unbalanced damage split: {og_split} => {split}");  // TODO --- uncomment warning!
        }
        return split;
    }

    public static (float, float, float) CalculateArmorDamage(IAttack attack, IArmor armor) {
        float total_attack_damage = RandomDamage(attack);
        float damage_split = CalculateDamageSplit(attack, armor);
        float attack_damage = total_attack_damage * (1 - damage_split);
        float base_armor_damage = total_attack_damage * damage_split;
        float armor_damage = base_armor_damage * attack.armor_effectiveness;

        attack_damage = Mathf.Max(1, attack_damage);
        armor_damage = Mathf.Max(1, armor_damage);
      
        // Debug.Log($"base_attack_damage: {total_attack_damage}, \ndamage_split: {damage_split}, \nattack_damage: {attack_damage}, \nbase_armor_damage: {base_armor_damage}, \narmor_damage: {armor_damage}");
        // Debug.Log($"{total_attack_damage} => {attack_damage} / {base_armor_damage}");
        if (total_attack_damage <= 0) { Debug.LogWarning($"negative total_attack_damage: {total_attack_damage}"); } 
        if (attack_damage <= 0) { Debug.LogWarning($"negative attack_damage: {attack_damage}"); } 
        if (armor_damage <= 0) { Debug.LogWarning($"negative armor_damage: {armor_damage}"); } 
        return (total_attack_damage, attack_damage, armor_damage);
    }

    public static (float, float) ResolveGunDamageArmored(IAttack attack, 
            ICharacterStatus status, Vector3 hit_location) {
        (float total_attack_damage, float health_damage, float armor_damage) = CalculateArmorDamage(attack, status.armor);
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
        status.health -= health_damage + overflow_damage;
        if(total_attack_damage < (health_damage + overflow_damage)) {
            Debug.LogWarning($"Overflow damage more than base: {total_attack_damage} => {health_damage} + {overflow_damage}");
        }
        if (armor_before >= status.armor.armor_durability) {
            // Debug.LogWarning($"negative damage ({armor_damage})! {armor_before} --> {status.armor.armor_durability}"); // TODO --- uncomment warning!
        }    
        return (health_damage, armor_damage);
    }

    public static void ResolveArmorBreak(IAttack attack, 
            ICharacterStatus status, Vector3 hit_location) {
        // Debug.Log($"Armor '{status.armor}' Broken by {attack}!!!");
        // TODO !
    }

    public static void AttackMiss(IAttack attack, Vector3 location) {

        foreach (IAttackMissEffect effect in GetMissEffects()) {
            effect.DisplayEffect(location, attack);
        }
    }

    public static void AttackEmpty(IFirearm weapon, Vector3 location) {
        foreach (IFirearmEffect effect in GetEmptyShotEffects()) {
            effect.DisplayFirearmEffect(location, weapon);
        }
    }

    public static void AttackStart(IAttack attack, Vector3 attack_direction, Vector3 location, bool is_melee_attack = false)
    {
        // Displays attack effects for firing a weapon
        if (is_melee_attack)
        {
            foreach (IMeleeAttackEffect effect in GetMeleeEffects())
            {
                effect.DisplayMeleeEffect(location, attack_direction, attack);
            }
        }
        else
        {
            foreach (IAttackShootEffect effect in GetShootEffects())
            {
                effect.DisplayEffect(location, attack);
            }
        }
    }
}