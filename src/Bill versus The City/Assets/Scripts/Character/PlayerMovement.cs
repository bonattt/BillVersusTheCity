using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharCtrl
{
    
    

    public override Vector3 LookTarget() {
        return new Vector3(0f, 0f, 0f);
    }
    
    public override Vector3 MoveVector() {
        return new Vector3(1f, 0f, 1f);
    }
}
