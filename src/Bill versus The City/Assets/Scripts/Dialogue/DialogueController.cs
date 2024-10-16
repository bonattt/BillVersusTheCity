

using UnityEngine;
using UnityEngine.UIElements;

public class DialogueController : MonoBehaviour { //, ISubMenu {
    // class controlling a single dialogue session

    private string dialogue_file_path;

    public UIDocument ui_doc;
    private VisualElement root, left_portraits, right_portraits;

    public void StartDialogue(string file_path) {
        dialogue_file_path = file_path;
    }

    void Start() {
        root = ui_doc.rootVisualElement;
        left_portraits = root.Q<VisualElement>("LeftPortraits");
        right_portraits = root.Q<VisualElement>("RightPortraits");
        left_portraits.Clear();
        right_portraits.Clear();

        VisualElement portrait1 = GetEmptyPortrait();
        SetPortraitImage(portrait1, "bill");
        left_portraits.Add(portrait1);

        VisualElement portrait2 = GetEmptyPortrait();
        SetPortraitImage(portrait2, "gangsta");
        right_portraits.Add(portrait2);
        
    }
    
    public static void SetPortraitImage(VisualElement portrait, string character_name) {
        Texture2D portrait_image = PortraitSystem.GetPortrait(character_name);
        _SetPortraitImage(portrait, portrait_image, character_name);
    }

    public static void SetPortraitImage(VisualElement portrait, string character_name, string pose) {
        Texture2D portrait_image = PortraitSystem.GetPortrait(character_name, pose);
        _SetPortraitImage(portrait, portrait_image, character_name);
    }

    private static void _SetPortraitImage(VisualElement portrait, Texture2D image, string character_name) {
        Label name_label = portrait.Q<Label>();
        name_label.text = character_name;
        portrait.style.backgroundImage = image;
    }

    public static VisualElement GetEmptyPortrait() {
        // gets a portrait, with no image set
        VisualElement portrait = new VisualElement();
        portrait.AddToClassList("dialogue_portrait");

        Label character_name = new Label();
        portrait.Add(character_name);
        character_name.text = "new character";
        character_name.AddToClassList("portrait_name_label");

        return portrait;
    }
    
    // public void MenuNavigation() {
    //     // TODO --- 
    // }
}