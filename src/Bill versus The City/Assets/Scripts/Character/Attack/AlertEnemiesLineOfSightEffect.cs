using UnityEngine;

public class AlertEnemiesLineOfSightEffect : IAttackHitEffect, 
        IAttackShootEffect, IAttackMissEffect, IWeaponEffect {
    
    public Vector3 offset = new Vector3(0f, 0f, 0f);

    public AlertEnemiesLineOfSightEffect() {
        // do nothing
    }
    public void DisplayDamageEffect(GameObject hit_target,
            Vector3 hit_location, IAttack attack) {
        AlertNearbyEnemies(hit_location);
    }

    public void DisplayWeaponEffect(Vector3 point, IWeapon weapon) {
        AlertNearbyEnemies(point);
    }

    public void DisplayEffect(Vector3 hit_location, IAttack attack) {
        AlertNearbyEnemies(hit_location);
    }


    public void AlertNearbyEnemies(Vector3 point) {
        EnemiesManager.inst.AlertEnemiesNear(point);
    }
}
