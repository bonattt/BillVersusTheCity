using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ICharacterStatus {
    public float health { get; set; }
    public float max_health { get; set; }
    public float armor { get; set; }
    public float armor_hardness { get; }

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

public interface IBullet : IAttack {
    public void ResolveHit(GameObject hit, Vector3 point);
}

public interface IAttackTarget {
    // TODO
    public ICharacterStatus GetStatus();
    public GameObject GetHitTarget();
}


public interface IWeapon {
    // TODO

    
}
