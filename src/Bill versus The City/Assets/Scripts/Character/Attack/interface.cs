using UnityEngine;

public interface IAttackHitEffect {
    // visual effect when a gun hits a character
    public void DisplayDamageEffect(GameObject hit, Vector3 hit_location, IAttack attack);
}


public interface IAttackShootEffect {
    // visual effect a gun is fired
    public void DisplayEffect(Vector3 miss_point, IAttack attack);
}

public interface IAttackMissEffect {
    // visual effect when an attack hits a wall
    public void DisplayEffect(Vector3 shoot_point, IAttack attack);
}
