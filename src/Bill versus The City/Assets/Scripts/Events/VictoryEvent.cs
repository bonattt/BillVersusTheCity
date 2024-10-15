using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryEvent : MonoBehaviour, IGameEvent
{
    // // Start is called before the first frame update
    // void Start()
    // {
    //     EnemiesManager.inst.Subscribe(this);   
    // }

    // public bool WinCondition() {
    //     return EnemiesManager.inst.remaining_enemies == 0 && EnemiesManager.inst.total_enemies != 0;
    // }

    // void OnDestroy() {
    //     EnemiesManager.inst.Unusubscribe(this);  
    // }
    
    
    public void ActivateEvent() {
        MenuManager.inst.WinGamePopup();
    }
}
