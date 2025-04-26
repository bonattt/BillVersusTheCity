using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPlayerMoney : MonoBehaviour
{
    public int dollars = -1;
    void Update()
    {
        dollars = PlayerCharacter.inst.inventory.dollars;
    }
}
