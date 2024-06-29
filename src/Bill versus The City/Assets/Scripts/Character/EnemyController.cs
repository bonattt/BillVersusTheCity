using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class EnemyController : CharCtrl
{

    public override Vector3 MoveVector() {
        return new Vector3(0f, 0f, 0f);
    }

    public override Vector3 LookTarget() {
        return new Vector3(0, 0, 0);
    }

    protected override void CharacterDeath() {
        base.CharacterDeath();
        Destroy(gameObject);
    }
}
