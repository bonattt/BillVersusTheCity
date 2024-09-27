using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeSettings : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake() {
        if (GameSettings.inst == null) {
            new GameSettings();
        }
        Destroy(this);
    }
}
