using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHUDManager : MonoBehaviour
{
    // TODO --- remove this    
    public static CombatHUDManager inst { get; private set; }

    void Awake() {
        inst = this;
    }
}
