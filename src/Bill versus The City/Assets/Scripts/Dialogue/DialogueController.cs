
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using UnityEngine;
using UnityEngine.UIElements;

public class DialogueController : MonoBehaviour { //, ISubMenu {
    // class controlling a single dialogue session

    private DialogueFile dialogue_file;

    public UIDocument ui_doc;
    private VisualElement root, left_portraits, right_portraits;

    private Label dialogue_text;

    public void StartDialogue(string file_path) {
        dialogue_file = new DialogueFile(file_path);
        dialogue_file.ParseFile();
    }

    public void NextDialogueStep() {
        List<IDialogueAction> actions = dialogue_file.GetNextActionsList();
        if (actions.Count == 0) {
            // no actions, dialogue is over
            MenuManager.inst.CloseMenu();
            return;
        }
        foreach (IDialogueAction a in actions) {
            a.ResolveDialogue(this);
        }
    }

    void Start() {
        root = ui_doc.rootVisualElement;
        left_portraits = root.Q<VisualElement>("LeftPortraits");
        right_portraits = root.Q<VisualElement>("RightPortraits");
        dialogue_text = root.Q<Label>("DialogueText");
        left_portraits.Clear();
        right_portraits.Clear();

        // TODO --- replace this with an actual blocking system
        VisualElement portrait1 = GetEmptyPortrait();
        SetPortraitImage(portrait1, "bill");
        left_portraits.Add(portrait1);

        VisualElement portrait2 = GetEmptyPortrait();
        SetPortraitImage(portrait2, "gangsta");
        right_portraits.Add(portrait2);
        
        NextDialogueStep();  // assumeds `StartDialogue(file_path)` was called before first frame with dialouge open 
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

    public void SetSpeakerName(string speaker_name) {
        if (speaker_name == null) {
            // TODO --- no speaker, just narration
        }
        
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;  // TODO --- investigate this deeper
        // use underscores for spaces in the name of the speaker. Title Case names
        speaker_name = textInfo.ToTitleCase(speaker_name.ToLower().Replace("_", " "));
        Debug.LogWarning($"Not Implelented: SetSpeakerName('{speaker_name}')");
        // TODO --- implement speaker_name
    }

    public void SetText(string new_dialouge) {
        dialogue_text.text = new_dialouge;
    }
    
    // public void MenuNavigation() {
    //     // TODO --- 
    // }
}