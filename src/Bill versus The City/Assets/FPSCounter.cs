using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class FPSCounter : MonoBehaviour
{
    public UIDocument ui_doc;
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
        float fps = Mathf.Round(1 / Time.deltaTime);
        fps_label.text = $"{fps} FPS";
    }
}
