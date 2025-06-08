using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderVictoryEvent : AbstractInteractionGameEvent
{
    protected override void Effect() {
        MenuManager.inst.WinGamePopup();
    }
}
