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
        progress_dial = ui_doc.rootVisualElement.Q<RadialProgress>();
        SetVisibility(visible_on_start);
    }

    public void SetVisibility(bool visibility) {
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

}
