using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GenericObjectiveUI : MonoBehaviour, IGenericObserver
{

    public UIDocument document;

    public string text_value = "Generic Victory Condition!";
    private Label text_display;

    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = document.rootVisualElement;
        text_display = root.Q<Label>("EnemiesLabel");
        EnemiesManager.inst.Subscribe(this);

        UpdateText();
    }

    public void UpdateObserver(IGenericObservable observable) {
        UpdateText();
    }

    public void UpdateText() {
        text_display.text = text_value;
    }
}
