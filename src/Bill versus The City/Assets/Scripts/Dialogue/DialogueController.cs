
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueController : MonoBehaviour, ISubMenu {
    // class controlling a single dialogue session

    private DialogueFile dialogue_file;

    public UIDocument ui_doc;
    private VisualElement root, left_portraits, right_portraits;

    private Label dialogue_text;
    private Dictionary<string, (string, StageDirection, StageDirection)> character_blocking = new Dictionary<string, (string, StageDirection, StageDirection)>();

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
        // SetPortrait("bill", StageDirection.left, StageDirection.right);
        // SetPortrait("gangsta", StageDirection.right, StageDirection.left);
        
        NextDialogueStep();  // assumeds `StartDialogue(file_path)` was called before first frame with dialouge open 
    }

    public void MenuNavigation() {
        // TODO --- remove debug (below)
        if (InputSystem.current.MenuCancelInput()) {
            MenuManager.inst.CloseAllMenus();
        }

        // TODO --- remove debug ^^^

        if (InputSystem.current.MenuNextInput()) {
            MenuManager.PlayMenuSound("menu_click");
            NextDialogueStep();
        }
    }
    
    public VisualElement SetPortrait(string character_name, StageDirection side, StageDirection facing) {
        Texture2D portrait_image = PortraitSystem.GetPortrait(character_name);
        return _SetPortrait(portrait_image, character_name, side, facing);
    }

    public VisualElement SetPortrait(string character_name, string pose, StageDirection side, StageDirection facing) {
        Texture2D portrait_image = PortraitSystem.GetPortrait(character_name, pose);
        return _SetPortrait(portrait_image, character_name, side, facing);
    }

    private VisualElement _SetPortrait(Texture2D image, string character_name, StageDirection side, StageDirection facing) {
        // handles setting the portrait for the given character, whether they're in the scene or not
        // TODO --- handle character already in scene
        VisualElement portrait = GetEmptyPortrait(character_name);
        _SetPortraitImage(portrait, image, character_name);
        _SetPortraitSide(portrait, side);
        _SetPortraitFacing(portrait, facing);
        return portrait;
    }

    private static VisualElement _SetPortraitImage(VisualElement portrait, Texture2D image, string character_name) {
        // takes a VisualElement and assigns the given image to that portrait
        Label name_label = portrait.Q<Label>();
        name_label.text = character_name;
        portrait.style.backgroundImage = image;
        return portrait;
    }

    private void _SetPortraitFacing(VisualElement portrait, StageDirection facing) {
        // turns the image in a portrait to face the correct direction
        
        if (facing == StageDirection.left) {
            // display mirror image along the X axis
            portrait.style.scale = new Scale(new Vector3(-1, 1, 1)); 
        }
        else if (facing == StageDirection.right) {
            // do nothing
        } 
        else {
            Debug.LogWarning($"cannot set rotate character portrait '{portrait.name}' to face to the '{facing}'!");
        }
    }

    private void _SetPortraitSide(VisualElement portrait, StageDirection side) {
        // move a portrait to the correct side of the screen\
        // TODO --- handle moving existing portrait
        if (side == StageDirection.left) {
            left_portraits.Add(portrait);
        }
        else if (side == StageDirection.right) {
            right_portraits.Add(portrait);
        } 
        else {
            throw new DialogueControllerException($"cannot set character portrait '{portrait.name}' to the '{side}' side of the screen");
        }
    }

    public static VisualElement GetEmptyPortrait(string name) {
        // gets a portrait, with no image set

        VisualElement portrait = new VisualElement();
        portrait.name = name;
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
}

[System.Serializable]
public class DialogueControllerException : System.Exception
{
    // public DialogueControllerException() { }
    public DialogueControllerException(string message) : base(message) { }
    // public DialogueControllerException(string message, System.Exception inner) : base(message, inner) { }
    // protected DialogueControllerException(
    //     System.Runtime.Serialization.SerializationInfo info,
    //     System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}