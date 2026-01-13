using UnityEngine;

public interface IAttackHitEffect
{
    // visual effect when a gun hits a character
    public void DisplayDamageEffect(GameObject hit, Vector3 hit_location, IAttack attack);
}


public interface IMeleeAttackEffect
{
    public void DisplayMeleeEffect(Vector3 position, Vector3 direction, IAttack attack);
}


public interface IAttackShootEffect
{
    // visual effect a gun is fired
    public void DisplayEffect(Vector3 shoot_point, IAttack attack);
}

public interface IAttackMissEffect {
    // visual effect when an attack hits a wall
    public void DisplayEffect(Vector3 miss_point, IAttack attack);
}

public interface IWeaponEffect {
    // visual effect which doesn't require an attack
    public void DisplayWeaponEffect(Vector3 point, IWeapon weapon);
}

public interface IFirearmEffect {
    // visual effect which doesn't require an attack
    public void DisplayFirearmEffect(Vector3 point, IFirearm weapon);
}

public interface INextAttackReadyEffect {
    public void DisplayNextAttackReadyEffect(Vector3 point, IWeapon weapon);
}
