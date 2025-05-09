using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubTravelMenuEvent : MonoBehaviour, IGameEventEffect, IInteractionEffect
{
    public HubTravelEvent travel_effect;
    public void ActivateEffect() {
        Effect();
    }
    
    public void Interact(GameObject actor) {
        Effect();
    }


    private void Effect() {
        HubTravelMenuCtrl menu = MenuManager.inst.OpenHubTravelMenu();
        menu.travel_effect = travel_effect;
    }
}
