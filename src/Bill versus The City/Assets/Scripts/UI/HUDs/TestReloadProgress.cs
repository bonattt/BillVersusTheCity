
using UnityEngine;
using UnityEngine.UIElements;

public class TestReloadProgress : MonoBehaviour {

    public UIDocument ui_document;
    private RadialProgress arc;
    public string reload_ui_class_prefix = "test";

    public float progress = 0f;
    public float progress_rate = 0.25f;
    public float max_progress = 1f;


    void Start() {
        arc = ui_document.rootVisualElement.Q<RadialProgress>();
        arc.progress_label_type = RadialProgressText.custom_text;
        string reload_ui_class = $"{reload_ui_class_prefix}_reload_progress";
        arc.AddToClassList(reload_ui_class);
        arc.Q<Label>().AddToClassList($"{reload_ui_class}_label");
    }

    void Update() {
        if (progress >= max_progress) {
            progress = 0f;
        }
        progress += Time.deltaTime * progress_rate;

        arc.progress = progress;
    }

}