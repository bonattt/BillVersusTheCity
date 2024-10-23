
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DialogueController : MonoBehaviour, ISubMenu {
    // class controlling a single dialogue session

    private DialogueFile dialogue_file;

    public UIDocument ui_doc;
    private VisualElement root, left_portraits, right_portraits, speaker_left_element;
    private Label dialogue_text, speaker_label;
    private Dictionary<string, VisualElement> character_portraits;


    public void StartDialogue(string file_path) {
        character_portraits = new Dictionary<string, VisualElement>();
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
        // speaker_left_element = root.Q<VisualElement>("Speaker");
        speaker_left_element = root.Q<VisualElement>("SpeakerLeft");
        speaker_label = speaker_left_element.Q<Label>();
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
    
    public void RemovePortrait(string character_name) {
        // removed the character portrait from the scene
        if (! character_portraits.ContainsKey(character_name)) {
            Debug.LogWarning($"cannot remove '{character_name}' from dialogug because it is not present!");
            return;
        }
        VisualElement portrait = character_portraits[character_name];
        if (portrait.parent != null) {
            portrait.parent.Remove(portrait);
        }
    }

    public void UpdatePose(string character_name, string pose) {
        // updates just the pose on a character already in a scene

        if (! character_portraits.ContainsKey(character_name)) {
            throw new DialogueActionsException("cannot update the pose for a character that's not in the dialogue!");
        }
        VisualElement portrait = character_portraits[character_name];
        Texture2D image = PortraitSystem.GetPortrait(character_name, pose);
        
        _SetPortraitImage(portrait, image);
    }

    public VisualElement SetPortrait(string character_name, StageDirection side, StageDirection facing) {
        Texture2D portrait_image = null;
        if (! character_portraits.ContainsKey(character_name)) {
            // if the character is not already in the scene, load their default portrait.
            // Otherwise, leave as null to preserve their previous pose
            portrait_image = PortraitSystem.GetPortrait(character_name);;
        }
        return _SetPortrait(portrait_image, character_name, side, facing);
    }

    public VisualElement SetPortrait(string character_name, string pose, StageDirection side, StageDirection facing) {
        Texture2D portrait_image = PortraitSystem.GetPortrait(character_name, pose);
        return _SetPortrait(portrait_image, character_name, side, facing);
    }

    private VisualElement _SetPortrait(Texture2D image, string character_name, StageDirection side, StageDirection facing) {
        // handles setting the portrait for the given character, whether they're in the scene or not
        // TODO --- handle character already in scene
        VisualElement portrait;
        if (character_portraits.ContainsKey(character_name)) {
            // TODO --- implement
            portrait = character_portraits[character_name];
        } 
        else {
            if (image == null) {
                throw new DialogueActionsException($"new characters in a scene cannot use a null image! '{character_name} {side} facing {facing}'");
            }
            // add new character to scene
            portrait = GetEmptyPortrait(character_name);
            character_portraits[character_name] = portrait;
        }
        // pass image as null to preserve the previous image
        if (image != null) {
            _SetPortraitImage(portrait, image);
        }
        _SetPortraitName(portrait, character_name);
        _SetPortraitSide(portrait, side);
        _SetPortraitFacing(portrait, facing);
        return portrait;
    }

    private static void _SetPortraitName(VisualElement portrait, string character_name) {
        Label name_label = portrait.Q<Label>();
        name_label.text = character_name;
    }

    private static void _SetPortraitImage(VisualElement portrait, Texture2D image) {
        // takes a VisualElement and assigns the given image to that portrait
        VisualElement portrait_image = portrait.Q<VisualElement>(PORTRAIT_IMAGE_ELEMENT);
        portrait_image.style.backgroundImage = image;
    }

    private void _SetPortraitFacing(VisualElement portrait, StageDirection facing) {
        // turns the image in a portrait to face the correct direction
        
        if (facing == StageDirection.left) {
            // display mirror image along the X axis
            portrait.Q<VisualElement>(PORTRAIT_IMAGE_ELEMENT).style.scale = new Scale(new Vector3(-1, 1, 1)); 
        }
        else if (facing == StageDirection.right) {
            // do nothing
        } 
        else {
            Debug.LogWarning($"cannot set rotate character portrait '{portrait.name}' to face to the '{facing}'!");
        }
    }

    private const string PORTRAIT_IMAGE_ELEMENT = "portrait";
    private void _SetPortraitSide(VisualElement portrait, StageDirection side) {
        // move a portrait to the correct side of the screen\
        // TODO --- handle moving existing portrait
        if (portrait.parent != null) {
            portrait.parent.Remove(portrait);
        }
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
        character_name.text = "new character";
        character_name.AddToClassList("portrait_name_label");
        portrait.Add(character_name);

        VisualElement portrait_image = new VisualElement();
        portrait_image.name = PORTRAIT_IMAGE_ELEMENT;
        portrait_image.AddToClassList("dialogue_portrait_image");
        portrait.Add(portrait_image);

        return portrait;
    }

    public void SetSpeakerName(string speaker_name) {
        if (speaker_name == null || speaker_name.Equals("*") || speaker_name.Equals("")) {
            speaker_left_element.style.visibility = Visibility.Hidden;
        } else {
            speaker_left_element.style.visibility = Visibility.Visible;
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;  // TODO --- investigate this deeper
            // use underscores for spaces in the name of the speaker. Title Case names
            speaker_label.text = textInfo.ToTitleCase(speaker_name.ToLower().Replace("_", " "));
        }
        // Debug.LogWarning($"Not Implelented: SetSpeakerName('{speaker_name}')");
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