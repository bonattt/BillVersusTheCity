using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GrenadeFuseUI : MonoBehaviour {
    public UIDocument ui_doc;
    public GrenadeFuse fuse;
    private RadialProgress progress_dial;
    void Start() {
        progress_dial = ui_doc.rootVisualElement.Q<RadialProgress>();
    }

    void Update() {
        UpdateProgressDial();
    }

    private void UpdateProgressDial() {
        progress_dial.progress = 100 * fuse.fuse_progress;
    }

}
