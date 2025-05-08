using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubTravelEvent : MonoBehaviour, IGameEventEffect, IInteractionEffect
{
    public void ActivateEffect() {
        Effect();
    }
    
    public void Interact(GameObject actor) {
        Effect();
    }


    private void Effect() {
        MenuManager.inst.OpenHubTravelMenu();
    }
}
