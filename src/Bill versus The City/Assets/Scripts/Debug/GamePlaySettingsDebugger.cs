using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class GamePlaySettingsDebugger : MonoBehaviour {

    public bool write = false;

    public float mouse_sensitivity = 1f;
    
    void Update() {
        GamePlaySettings settings = GameSettings.inst.game_play_settings;
        if (write) {
            settings.mouse_sensitivity = this.mouse_sensitivity;
        } else {
            mouse_sensitivity = settings.mouse_sensitivity;
        }
    }
}