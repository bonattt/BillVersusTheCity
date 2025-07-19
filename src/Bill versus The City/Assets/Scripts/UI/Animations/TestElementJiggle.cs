

using UnityEngine;
using UnityEngine.UIElements;

public class TestElementJiggle : MonoBehaviour {

    public UIDocument ui_doc;
    public string element_name;
    private VisualElement target_element;

    public float start_delay_seconds = 5f;

    void Update() {
        if (Time.unscaledTime > start_delay_seconds) {
            StartJiggle();
            Destroy(this);
        }
    }


    public void StartJiggle() {
        target_element = ui_doc.rootVisualElement.Q<VisualElement>(element_name);
        ElementJiggle jiggle = GetComponent<ElementJiggle>();
        jiggle.SetTarget(target_element);
        jiggle.StartJiggle();
    }
}