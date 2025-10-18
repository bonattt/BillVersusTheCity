using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ICharacterStatus { // : IAttackTarget { // TODO --- make CharacterStatus handle attacks, instead of movement
    public float health { get; set; }
    public float max_health { get; set; }
    public bool adjusting_difficulty { get; } // flag set to true while values are adjusted for difficulty, to avoid triggering on-damage effects when a character's health is adjusted for difficulty level
    public IArmor armor { get; }

    public void ApplyNewArmor(IArmor armor_template); // create new armor from IArmor as a template
    public void ApplyNewArmor(ScriptableObject armor_template); // create new armor from ScriptableObject template
    public void ApplyExistingArmor(IArmor existing_armor); // set an existing armor, and preserve stats
    public void RemoveArmor(); // set armor to null

    public void Subscribe(ICharStatusSubscriber sub);
    public void Unsubscribe(ICharStatusSubscriber sub);
    public void UpdateStatus();
}


public interface ICharStatusSubscriber {
    public void StatusUpdated(ICharacterStatus status);
    public void OnDamage(ICharacterStatus status);
    public void OnHeal(ICharacterStatus status);
    public void OnDeath(ICharacterStatus status) { /* do nothing by default */ } // triggers immediately on death
    public void DelayedOnDeath(ICharacterStatus status) { /* do nothing by default */ } // triggers after a death animation finishes playing
    public void OnDeathCleanup(ICharacterStatus status) { /* do nothing by default */ } // triggers some time after death to despawn the character
}


public interface IAttack {
    // TODO
    public IAttackTarget attacker { get; }
    public IWeapon weapon { get; }
    public float damage_falloff { get; }
    public float attack_damage_min { get; }
    public float attack_damage_max { get; }
    public float armor_effectiveness { get; }
    public bool ignore_armor { get; }
    public float final_health_damage { get; set; }
    public float final_armor_damage { get; set; }

}

public class MeleeAttack : IAttack {
    public IAttackTarget attacker { get; set; }
    public IWeapon weapon {
        get => melee_weapon;
        set {
            melee_weapon = (IMeleeWeapon)value;
        }
    }
    public IMeleeWeapon melee_weapon { get; set; }
    public float attack_damage_min { get => melee_weapon.weapon_damage_min; }
    public float attack_damage_max { get => melee_weapon.weapon_damage_max; }
    public float armor_effectiveness { get => melee_weapon.armor_effectiveness; }
    public bool ignore_armor { get; set; }
    public float final_health_damage { get; set; }
    public float final_armor_damage { get; set; }
    public float damage_falloff { get => 0; }

    private HashSet<IAttackTarget> _hit_targets = new HashSet<IAttackTarget>();
    public HashSet<IAttackTarget> hit_targets { get => _hit_targets; }

}

public class FirearmAttack : IAttack
{

    public IAttackTarget attacker { get; set; }
    public IWeapon weapon {
        get => firearm;
        set {
            firearm = (IFirearm)value;
        }
    }
    public IFirearm firearm { get; set; }
    public float attack_damage_min { get; set; }
    public float attack_damage_max { get; set; }
    public float armor_effectiveness { get; set; }
    public bool ignore_armor { get; set; }
    public float final_health_damage { get; set; }
    public float final_armor_damage { get; set; }
    public float damage_falloff { get; set; }
}

public interface IArmor : IItem, IDifficultyAdjusted {
    // maximum HP of the armor
    public float armor_max_durability { get; }

    // current HP of the armor
    public float armor_durability { get; set; }

    // reduces overall damage
    public float armor_hardness { get; }

    // percentage of damage dealt to the wearer of the armor
    public float armor_protection { get; }
    public IArmor CopyArmor();
}

public interface ITrackedProjectile {
    public bool is_threat { get; } // if true, enemies will be supressed by this bullet
    public float threat_level { get; }
    public Transform location { get; }
}

public interface IBullet : IAttack, ITrackedProjectile {
    public void ResolveCollision(GameObject hit, Vector3 point);
}

public interface ICharacterMovement {
    
}

public interface IAttackTarget {
    // TODO
    public bool is_player { get; }
    public ICharacterStatus GetStatus(); // TODO --- remove this, make ICharacterStatus extend this interface instead. 
    public GameObject GetHitTarget(); // game object for handling effects when a target is hit
    public Transform GetAimTarget(); // return a transform to aim at when the character is targetted with attacks
    public void OnAttackHitRecieved(IAttack attack);
    public void OnAttackHitDealt(IAttack attack, IAttackTarget target);
}

public interface IItem {
    public string item_id { get; }
    public string item_name { get; }
    public Sprite item_icon { get; }
}


public interface IWeapon : IItem {
    public float weapon_damage_min { get; }
    public float weapon_damage_max { get; }
    public float armor_effectiveness { get; }
    public string attack_sound { get; }

    public bool AttackClicked(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker); // returns if shot fired
    public bool AttackHold(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker); // returns if shot fired
    public bool AttackReleased(Vector3 attack_direction, Vector3 attack_start_point, float inaccuracy, IAttackTarget attacker);
    public IWeapon CopyWeapon();
}

public interface IAttackController
{
    public float aim_percent { get; }
    public IWeapon current_weapon { get; set; }
    public IMeleeWeapon current_melee { get; set; }
    public IFirearm current_gun { get; set; }
    public Transform shoot_from { get; }
    public bool switch_weapons_blocked { get; set; }
    public void StartAttack(Vector3 attack_direction);
    public void AttackHold(Vector3 attack_direction);
    public void AttackReleased(Vector3 attack_direction);
    public void StartAim();
    public void StopAim();
    public bool CanAttack();
}

public interface IMeleeWeapon : IWeapon
{
    public float attack_reach { get; }
    public float attack_windup { get; } // after an attack is triggered, it takes `attack_windup` seconds for the attack to actually deal any damage
    public float attack_duration { get; } // once the attack windup finishes, it will deal damage once to anything within range during `attack_duration` seconds
    public float attack_recovery { get; } // once `attack_duration` finishes, the attack animation will continue for `attack_recovery` seconds before 
    public float attack_cooldown { get; } // time that must pass after a attack completes it's recovery time before another attack can be made
    public float total_attack_time { get => attack_windup + attack_duration + attack_recovery + attack_cooldown; }
    public IMeleeWeapon CopyMeleeWeapon();
}

public interface IFirearm : IWeapon
{
    public AttackStartPosition attack_start_position { get; }
    // equipment
    public WeaponSlot weapon_slot { get; }
    public WeaponClass weapon_class { get; }
    // ammo
    public AmmoType ammo_type { get; }
    public int ammo_capacity { get; }
    public int reload_amount { get; }
    public float reload_time { get; }
    public int current_ammo { get; set; }
    public int ammo_drop_size { get; }
    public bool is_consumable { get; } // if true, the weapon is the same as it's ammo.
                                       // (eg. is_consumable = true, a grenade or throwing knife, the weapon IS the ammo for the gun.)
                                       // (eg. is_consumable = false, a rifle needs a separate item (bullets) to shoot)

    // rate of fire
    public FiringMode firing_mode { get; }
    public bool auto_fire { get; }
    public float semi_auto_fire_rate { get; }
    public float full_auto_fire_rate { get; }
    public int n_shots { get; }

    // accuracy
    public float aimed_inaccuracy { get; }
    public float initial_inaccuracy { get; }
    public float time_to_aim { get; }
    public float recoil_inaccuracy { get; }
    public float recoil_max { get; }
    public float recoil_recovery { get; }
    public float recoil_shake { get; }

    // damage
    public float bullet_speed { get; }
    // public float weapon_damage_min { get; } // moved to IWeapon from IFirearm
    // public float weapon_damage_max { get; } // moved to IWeapon from IFirearm
    // public float armor_effectiveness { get; } // moved to IWeapon from IFirearm
    public float damage_falloff_rate { get; }

    // aiming
    public float aim_zoom { get; }
    public float aim_move_speed { get; }
    public float max_zoom_range { get; }

    // effects
    // public string attack_sound { get; } // moved to IWeapon from IFirearm
    public GameObject bullet_prefab { get; }
    public string empty_gunshot_sound { get; }
    public string reload_start_sound { get; }
    public string reload_complete_sound { get; }

    public bool HasWeaponSettings();
    public void NextWeaponSetting();
    public void PreviousWeaponSetting();
    public IFirearm CopyFirearm();
}

public interface IPurchase {
    public int purchase_cost { get; }
    public void ApplyPurchase(PlayerInventory inv);
}

public enum FiringMode {
    semi_auto,
    full_auto,
    select 
}


public enum AmmoType {
    handgun, // 9mm for handguns or SMGs
    magnum,  // .357 magnum for revolvers
    rifle,   // .223 for AR-15
    shotgun,  // buckshot shotgun shells
    rocket,
    grenade,
}

public interface IWeaponManager
{
    // implements Observable for updating UI when the player switches
    // weapons, or updates ammo
    public int? current_slot { get; }
    public IFirearm current_gun { get; }
    public float current_inaccuracy { get; }
    public bool is_aiming { get; }
    public void StartAim();
    public void StopAim();
    public void Subscribe(IWeaponManagerSubscriber sub);
    public void Unsubscribe(IWeaponManagerSubscriber sub);
    public void UpdateSubscribers();
}

public interface IWeaponManagerSubscriber
{
    public void UpdateWeapon(int? slot, IFirearm weapon);
}

public interface IReloadManager {
    public bool reloading { get; }
    public float reload_time { get; }
    public float reload_progress { get; }
    public bool is_active { get; set; }
    public void UpdateReloadStarted(IFirearm weapon);
    public void UpdateReloadFinished(IFirearm weapon);
    public void UpdateReloadCancelled(IFirearm weapon);
    public void Subscribe(IReloadSubscriber sub);
    public void Unsubscribe(IReloadSubscriber sub);
}

public interface IGenericObserver {
    public void UpdateObserver(IGenericObservable observable);
}

public interface IGenericObservable {
    public void Subscribe(IGenericObserver sub);
    public void Unusubscribe(IGenericObserver sub);
}

public interface IReloadSubscriber {
    public void StartReload(IReloadManager manager, IFirearm weapon);
    public void ReloadFinished(IReloadManager manager, IFirearm weapon);
    public void ReloadCancelled(IReloadManager manager, IFirearm weapon);
}
