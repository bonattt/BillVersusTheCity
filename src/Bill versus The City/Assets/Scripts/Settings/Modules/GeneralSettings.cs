using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class GeneralSettings : AbstractSettingsModule {
    
    private bool _show_fps = true;
    public bool show_fps { 
        get { return _show_fps; }
        set {
            _show_fps = value;
            UpdateSubscribers("show_fps");
        }
    }
}