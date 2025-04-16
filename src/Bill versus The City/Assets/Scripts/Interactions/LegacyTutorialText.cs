using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LegacyTutorialText : MonoBehaviour, IInteractionEffect
{
    public float font_size = 20f;
    public string tutorial_name = "";
    public string tutorial_text = "test";
    private Label header, additional_text;
    private Interaction interaction; 
    private InteractableUI interactable_ui;

    void Start() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("Panel");
        interaction = GetComponent<Interaction>();
        interactable_ui = GetComponent<InteractableUI>();

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

    private void UpdateText() {
        if (interaction.interaction_targeted) {
            interactable_ui.name_header = "Hide Tutorial";
        } else {
            if (tutorial_name.Equals("")) {
                interactable_ui.name_header = "Tutorial";
            } else {
                interactable_ui.name_header = $"Tutorial: {tutorial_name}";
            }
        }
        additional_text.text = tutorial_text;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
    }
    
    public void Interact(GameObject actor) {
        Destroy(gameObject);
    }
}
