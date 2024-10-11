using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearEnemiesWinCondition : MonoBehaviour, IGenericObserver
{
    // Start is called before the first frame update
    void Start()
    {
        EnemiesManager.inst.Subscribe(this);   
    }

    public bool WinCondition() {
        return EnemiesManager.inst.remaining_enemies == 0 && EnemiesManager.inst.total_enemies != 0;
    }

    void OnDestroy() {
        EnemiesManager.inst.Unusubscribe(this);  
    }
    
    public void UpdateObserver(IGenericObservable observable) {
        if (WinCondition()) {
            MenuManager.inst.WinGamePopup();
            Destroy(this);
        }
    }
}
