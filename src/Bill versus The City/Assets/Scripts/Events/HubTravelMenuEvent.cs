using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubTravelMenuEvent : AbstractInteractionGameEvent
{
    // event which opens the hub travel menu. (does not implement the hub travel, which is triggered by a button in the menu this opens)
    public HubTravelEvent travel_effect;

    protected override void Effect() {
        HubTravelMenuCtrl menu = MenuManager.inst.OpenHubTravelMenu();
        menu.travel_effect = travel_effect;
    }
}
