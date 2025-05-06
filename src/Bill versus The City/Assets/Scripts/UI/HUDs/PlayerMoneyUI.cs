

using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMoneyUI : MonoBehaviour {
    private UIDocument ui_doc;
    private Label dollars_label;

    void Start() {
        ui_doc = GetComponent<UIDocument>();
        dollars_label = ui_doc.rootVisualElement.Q<Label>("DollarsLabel");
        UpdateUI();
    }

    void Update() {
        UpdateUI();
    }

    private void UpdateUI() {
        dollars_label.text = $"Money: ${PlayerCharacter.inst.inventory.total_dollars}.00";
    }
}