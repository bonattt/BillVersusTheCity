using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ICharacterStatus {
    public float health { get; set; }
    public float max_health { get; set; }
    public IArmor armor { get; }

    public void ApplyNewArmor(ScriptableObject armor_template); // create new armor from ScriptableObject template
    public void ApplyExistingArmor(IArmor existing_armor); // set an existing armor, and preserve stats
    public void RemoveArmor(); // set armor to null

    public void Subscribe(ICharStatusSubscriber sub);
    public void Unsubscribe(ICharStatusSubscriber sub);
    public void UpdateStatus();
}


public interface ICharStatusSubscriber {
    public void StatusUpdated(ICharacterStatus status);
    public void OnDeath(ICharacterStatus status) { /* do nothing by default */ } // triggers immediately on death
    public void DelayedOnDeath(ICharacterStatus status) { /* do nothing by default */ } // triggers after a death animation finishes playing
    public void OnDeathCleanup(ICharacterStatus status) { /* do nothing by default */ } // triggers some time after death to despawn the character
}


public interface IAttack {
    // TODO
    public IAttackTarget attacker { get; }
    public IWeapon weapon { get; }
    public float attack_damage_min { get; }
    public float attack_damage_max { get; }
    public float armor_penetration { get; }
    public bool ignore_armor { get; }
    public float final_health_damage { get; set; }
    public float final_armor_damage { get; set; }

}

public class GenericAttack : IAttack {
    
    public IAttackTarget attacker { get; set; }
    public IWeapon weapon { get; set; }
    public float attack_damage_min { get; set; }
    public float attack_damage_max { get; set; }
    public float armor_penetration { get; set; }
    public bool ignore_armor { get; set; }
    public float final_health_damage { get; set; }
    public float final_armor_damage { get; set; }
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
}

public interface IBullet : IAttack {
    public void ResolveHit(GameObject hit, Vector3 point);
}

public interface IAttackTarget {
    // TODO
    public ICharacterStatus GetStatus();
    public GameObject GetHitTarget(); // game object for handling effects when a target is hit
    public Transform GetAimTarget(); // return a transform to aim at when the character is targetted with attacks
    public void OnAttackHitRecieved(IAttack attack);
    public void OnAttackHitDealt(IAttack attack, IAttackTarget target);
}

public interface IItem {
    public string item_name { get; }
    public Sprite item_icon { get; }
}


public interface IWeapon : IItem {
    // equipment
    public WeaponSlot weapon_slot { get; }
    public WeaponClass weapon_class { get; }
    // ammo
    public AmmoType ammo_type { get; }
    public int ammo_capacity { get; }
    public int reload_amount { get; }
    public float reload_time { get; }
    public int current_ammo { get; set; }

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

    public float bullet_speed { get; }
    public float weapon_damage_min { get; }
    public float weapon_damage_max { get; }
    public float armor_penetration { get; }

    // aiming
    public float aim_zoom { get; }
    public float aim_move_speed { get; }
    public float max_zoom_range { get; }
    
    // effects
    public string gunshot_sound { get; }
    public string empty_gunshot_sound { get; }
    public string reload_start_sound { get; }
    public string reload_complete_sound { get; }

    public bool HasWeaponSettings();
    public void NextWeaponSetting();
    public void PreviousWeaponSetting();
    public IWeapon CopyWeapon();
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
    shotgun  // buckshot shotgun shells
}

public interface IWeaponManager {
    // implements Observable for updating UI when the player switches
    // weapons, or updates ammo
    public int? current_slot { get; }
    public IWeapon current_weapon { get; }
    public float current_inaccuracy { get; }
    public bool is_aiming { get; }
    public void StartAim();
    public void StopAim();
    public void Subscribe(IWeaponManagerSubscriber sub);
    public void Unsubscribe(IWeaponManagerSubscriber sub);
}

public interface IWeaponManagerSubscriber {
    public void UpdateWeapon(int? slot, IWeapon weapon);
}

public interface IReloadManager {
    public bool reloading { get; }
    public float reload_time { get; }
    public float reload_progress { get; }
    public bool is_active { get; set; }
    public void UpdateStartReload(IWeapon weapon);
    public void UpdateFinishReload(IWeapon weapon);
    public void UpdateCancelReload(IWeapon weapon);
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
    public void StartReload(IReloadManager manager, IWeapon weapon);
    public void FinishReload(IReloadManager manager, IWeapon weapon);
    public void CancelReload(IReloadManager manager, IWeapon weapon);
}
