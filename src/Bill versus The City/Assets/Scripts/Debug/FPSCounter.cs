using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class FPSCounter : MonoBehaviour
{
    public UIDocument ui_doc;
    public float sample_rate_seconds = 0.5f;
    private float sampled_at = 0f;
    private Label fps_label;

    // Start is called before the first frame update
    void Start()
    {
        fps_label = ui_doc.rootVisualElement.Q<Label>("FPS_display");

        UpdateFPS();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFPS();
    }
    
    private void UpdateFPS() {
        if (GameSettings.inst.debug_settings.GetBool("show_fps")) {
            if (sampled_at + sample_rate_seconds < Time.unscaledTime) {
                float fps = GetFPS();
                sampled_at = Time.unscaledTime;
                fps_label.text = $"{fps} FPS";
            } // for readability, only update the FPS once per <sample_rate>
        } else {
            fps_label.text = "";
        }
    }

    public static float GetFPS() {
        return (float) Math.Round(1 / Time.unscaledDeltaTime, 1);
    }
}
