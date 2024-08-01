using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ICharacterStatus {
    public float health { get; set; }
    public float max_health { get; set; }
    public IArmor armor { get; set; }

    public void Subscribe(ICharStatusSubscriber sub);

    public void Unsubscribe(ICharStatusSubscriber sub);

    public void UpdateStatus();
}


public interface ICharStatusSubscriber {
    public void StatusUpdated(ICharacterStatus status);
}


public interface IAttack {
    // TODO
    public IAttackTarget attacker { get; }
    public float attack_damage_min { get; }
    public float attack_damage_max { get; }
    public float armor_damage { get; }

}

public interface IArmor {
    // maximum HP of the armor
    public float armor_max_durability { get;}

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
    public GameObject GetHitTarget();
}


public interface IWeapon {
    public string weapon_name { get; }
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

    // accuracy
    public float aimed_inaccuracy { get; }
    public float initial_inaccuracy { get; }
    public float aim_speed { get; }
    public float recoil_inaccuracy { get; }
    public float recoil_shake { get; }

    public float bullet_speed { get; }
    public float weapon_damage_min { get; }
    public float weapon_damage_max { get; }
    public float armor_penetration { get; }
}

public enum FiringMode {
    semi_auto,
    full_auto,
    select 
}


public enum AmmoType {
    handgun,
    rifle
}

public interface IWeaponManager {
    // implements Observable for updating UI when the player switches
    // weapons, or updates ammo
    public int? current_slot { get; }
    public IWeapon current_weapon { get; }
    public void Subscribe(IWeaponManagerSubscriber sub);
    public void Unsubscribe(IWeaponManagerSubscriber sub);
}

public interface IWeaponManagerSubscriber {
    public void UpdateWeapon(int? slot, IWeapon weapon);
}
