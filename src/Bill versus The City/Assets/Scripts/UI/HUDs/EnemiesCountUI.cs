using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemiesCountUI : MonoBehaviour, IGenericObserver
{

    public UIDocument document;

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
        if (EnemiesManager.inst.remaining_enemies <= 0) {
            text_display.text = "(Return to your Truck!)";
        } else {
            text_display.text = $"Enemies: {EnemiesManager.inst.remaining_enemies} / {EnemiesManager.inst.total_enemies}";
        }
    }
}
