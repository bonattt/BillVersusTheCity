
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.UIElements;

public class AmmoContainerInteractionText : AbstractInteractionText
{
    public AmmoContainer ammo_container;
    // public UIDocument ui_doc;
    // public float font_size = 20f;

    // private Label additional_text;


    // void Start() {
    //     VisualElement root = ui_doc.rootVisualElement.Q<VisualElement>("Panel");

    //     additional_text = new Label();
    //     additional_text.name = "AdditionalText";
    //     additional_text.style.fontSize = font_size;
    //     additional_text.style.unityTextAlign = TextAnchor.UpperLeft;
    //     additional_text.style.alignSelf = Align.FlexStart;
    //     root.Add(additional_text);
    //     UpdateText();
    // }

    // void Update() {
    //     // this could probably be put somewhere more efficient, but it's low priority
    //     UpdateText();
    // }

    // public void UpdateText() {
    //     additional_text.text = GetText();
    // }

    public override string GetText() {
        
        StringBuilder text = new StringBuilder();
        for (int i = 0; i < AmmoContainer.display_order.Length; i++) {
            AmmoType type = AmmoContainer.display_order[i];
            if (! ammo_container.HasAmmoType(type)) {
                continue;
            }
            text.Append(ammo_container.GetTextDisplay(type));
            // if (i < AmmoContainer.display_order.Length - 1) {
                text.Append("\n");
            // }
        }
        string output = text.ToString();
        if (output.Length == 0) { return output; }
        char last_char = output[output.Length - 1];
        if (last_char == '\n') {
            output = output.Substring(0, output.Length - 1);
        }
        return output;
    }

}
