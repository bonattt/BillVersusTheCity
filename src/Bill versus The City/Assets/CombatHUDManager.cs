using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHUDManager : MonoBehaviour
{
    
    public static CombatHUDManager inst { get; private set; }

    void Awake() {
        inst = this;
    }
}
