using UnityEngine;

public class HearableGunshot : IAttackHitEffect, 
        IAttackShootEffect, IAttackMissEffect, IFirearmEffect {

    public float range = 10f;
    public float alarm_level = 2f;

    public HearableGunshot() { /* do nothing */ }
    public HearableGunshot(float range, float alarm_level)
    {
        this.range = range;
        this.alarm_level = alarm_level;
    }
    public void DisplayDamageEffect(GameObject hit_target,
            Vector3 hit_location, IAttack attack) {
        CreateSound(hit_location);
    }

    public void DisplayFirearmEffect(Vector3 point, IFirearm weapon) {
        CreateSound(point);
    }

    public void DisplayEffect(Vector3 hit_location, IAttack attack) {
        CreateSound(hit_location);
    }


    public void CreateSound(Vector3 point)
    {
        GameSound new_sound = new GameSound(point, range, alarm_level);
        EnemyHearingManager.inst.NewSound(new_sound);
    }
}
