using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class GamePlaySettings : AbstractSettingsModule {
    private float _mouse_sensitivity = 1f;
    public const float MIN_MOUSE_SENSITIVITY = 0.01f;
    public const string MOUSE_SENSITIVITY = "mouse_sensitivity";
    public float mouse_sensitivity {
        get { return _mouse_sensitivity; }
        set {
            if (value <= MIN_MOUSE_SENSITIVITY) {
                Debug.LogWarning($"mouse_sensitivity less than minimum {value} --> {MIN_MOUSE_SENSITIVITY}");
                _mouse_sensitivity = MIN_MOUSE_SENSITIVITY;
            } else {
                _mouse_sensitivity = value;
            }
            UpdateSubscribers(MOUSE_SENSITIVITY);
        }
    }
}