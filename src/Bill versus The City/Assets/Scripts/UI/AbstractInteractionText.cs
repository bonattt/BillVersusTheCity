
using System.Text;

using UnityEngine;
using UnityEngine.UIElements;

public abstract class AbstractInteractionText : MonoBehaviour
{
    public float font_size = 20f;

    protected Label additional_text;

    void Start() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("Panel");

        additional_text = new Label();
        additional_text.name = "AdditionalText";
        additional_text.style.fontSize = font_size;
        additional_text.style.unityTextAlign = TextAnchor.UpperLeft;
        additional_text.style.alignSelf = Align.FlexStart;
        additional_text.style.overflow = Overflow.Visible; 
        additional_text.style.whiteSpace = WhiteSpace.Normal;  // this is needed for the text overflow to actually work...
        root.Add(additional_text);
        UpdateText();
    }

    void Update() {
        // this could probably be put somewhere more efficient, but it's low priority
        UpdateText();
    }

    public void UpdateText() {
        additional_text.text = GetText();
    }

    public abstract string GetText();
}
