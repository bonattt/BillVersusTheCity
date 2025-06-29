using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GrenadeFuseUI : MonoBehaviour {
    public UIDocument ui_doc;
    public GrenadeFuse fuse;
    private RadialProgress progress_dial;
    public bool visible_on_start = true;
    void Start() {
        InitializeUI();
        SetVisibility(visible_on_start);
    }

    private void InitializeUI() {
        // initialize field references to UIDocument
        progress_dial = ui_doc.rootVisualElement.Q<RadialProgress>();
    }

    public void SetVisibility(bool visibility) {
        if (progress_dial == null) {
            Debug.Log("Start not called yet, setting visibility to `visibility_on_start`");
            visible_on_start = visibility;
            return;
        }
        if (visibility) {
            progress_dial.style.visibility = Visibility.Visible;
        } else {
            progress_dial.style.visibility = Visibility.Hidden;
        }
    }

    void Update() {
        UpdateProgressDial();
    }

    private void UpdateProgressDial() {
        progress_dial.progress = 100 * fuse.fuse_progress;
    }
    
    public static ShowGrenadeFuse show_grenade_fuse {
        get => GameSettings.inst.debug_settings.show_grenade_fuse;
    }

    public static bool ShowFuseUIWhenThrown() {
        return show_grenade_fuse == ShowGrenadeFuse.always || show_grenade_fuse == ShowGrenadeFuse.after_throw;
    }

    public static bool ShowFuseUIWhenHeld() {
        return show_grenade_fuse == ShowGrenadeFuse.always
                || show_grenade_fuse == ShowGrenadeFuse.while_held
                || show_grenade_fuse == ShowGrenadeFuse.default_value;
    }

}
