
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DialogueController : AbstractCloseEventMenu {
    // class controlling a single dialogue session

    private DialogueFile dialogue_file;

    public bool dialogue_completed { get; private set; }

    public UIDocument ui_doc;
    private VisualElement root, left_portraits, right_portraits, speaker_left_element;
    private Label dialogue_text, speaker_label;
    private Dictionary<string, VisualElement> character_portraits;

    // character_name -> (portrait_name, display_name)
    private static readonly Dictionary<string, (string, string)> DEFAULT_PORTRAIT_ALIASES = new Dictionary<string, (string, string)>();
    private Dictionary<string, (string, string)> _portrait_aliases = null;
    public Dictionary<string, (string, string)> portrait_aliases {
        get {
            if (_portrait_aliases == null) {
                ResetAliases();
            }
            return _portrait_aliases;
        }
    }

    // flexible assignment for effects after finishing the dialogue.
    // if null, nothing happens.
    // if it's not null, try to use it as an IGameEvent. If that doesn't work, use it as a prefab.
    // TODO --- depricated, use ICloseEventMenu.AddCloseCallback
    public MonoBehaviour dialogue_finished = null; // WARNING: Depricated, use AddDialogueCallback(IGameEventEffect)
    private List<IGameEventEffect> dialogue_callbacks = new List<IGameEventEffect>(); // TODO --- depricated, use ICloseEventMenu.AddCloseCallback
    public void AddDialogueCallback(IGameEventEffect new_effect) => dialogue_callbacks.Add(new_effect);  // TODO --- depricated, use ICloseEventMenu.AddCloseCallback


    private void ResetAliases() {
        // resets aliases to the default aliases
        _portrait_aliases = new Dictionary<string, (string, string)>(DEFAULT_PORTRAIT_ALIASES);
    }

    public void AddAlias(string character_name, string portrait_name, string display_name) {
        // adds a portrait allias. Portrait alliases are used to display multiple distinct characters using the same character art.
        // `character_name` is the name used to reference the character for blocking
        // `portrait_name` is the name used to reference the character's sprites
        // `display_name` is the name used to show the character in name tags
        portrait_aliases[character_name] = (portrait_name, display_name);
    }

    public void StartDialogue(string file_path) {
        dialogue_completed = false;
        character_portraits = new Dictionary<string, VisualElement>();
        dialogue_file = new DialogueFile(file_path);
        dialogue_file.ParseFile();
    }

    public void NextDialogueStep() {
        List<IDialogueAction> actions = dialogue_file.GetNextActionsList();
        if (actions.Count == 0) {
            // no actions, dialogue is over
            MenuManager.inst.CloseMenu();
            DialogueFinished();
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

        NextDialogueStep();  // assumeds `StartDialogue(file_path)` was called before first frame with dialouge open 
    }

    public override void MenuNavigation() {
        if (InputSystem.current.MenuNextInput()) {
            MenuManager.PlayMenuSound("menu_click");
            NextDialogueStep();
        } else {
            MenuManager.inst.TryOpenPauseMenu();
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
            throw new DialogueActionsException($"cannot update the pose for a character that's not in the dialogue '{character_name}'!");
        }
        VisualElement portrait = character_portraits[character_name];
        Texture2D image = this.GetPortrait(character_name, pose);
        
        _SetPortraitImage(portrait, image);
    }

    public void SetBlocking(StageDirection side, List<string> character_names) {
        // validate all characters exist in dialogue
        List<VisualElement> portraits_to_move = new List<VisualElement>();
        foreach (string cn in character_names) {
            if (! character_portraits.ContainsKey(cn)) {
                throw new DialogueActionsException("cannot set blocking for character not in scene");
            }
            else {
                portraits_to_move.Add(character_portraits[cn]);
            }
        }

        //

        foreach (VisualElement p in portraits_to_move) {
            p.parent.Remove(p);
        }
        
        if (side == StageDirection.left) {
            for (int i = portraits_to_move.Count - 1; i > -1; i--) {
                left_portraits.Add(portraits_to_move[i]);
            }
        } else if (side == StageDirection.right) {
            for (int i = 0; i < portraits_to_move.Count; i++) {
                right_portraits.Add(portraits_to_move[i]);
            }
        }
        else {
            throw new DialogueActionsException($"side must be either left or right, not {side}");
        }
    }

    public Texture2D GetPortrait(string character_name) {
        // returns a portrait from PortraitSystem, using character aliases to look up the portrait name if there is an alias
        if (! portrait_aliases.ContainsKey(character_name)) {
            return PortraitSystem.GetPortrait(character_name);
        }
        (string portrait_name, string display_name) = portrait_aliases[character_name];
        return PortraitSystem.GetPortrait(portrait_name);
    }

    public Texture2D GetPortrait(string character_name, string pose) {
        // returns a portrait from PortraitSystem, using character aliases to look up the portrait name if there is an alias
        if (! portrait_aliases.ContainsKey(character_name)) {
            return PortraitSystem.GetPortrait(character_name, pose);
        }
        (string portrait_name, string display_name) = portrait_aliases[character_name];
        return PortraitSystem.GetPortrait(portrait_name, pose);
    }

    public VisualElement SetPortrait(string character_name, StageDirection side, StageDirection facing) {
        Texture2D portrait_image = null;
        if (! character_portraits.ContainsKey(character_name)) {
            // if the character is not already in the scene, load their default portrait.
            // Otherwise, leave as null to preserve their previous pose
            portrait_image = this.GetPortrait(character_name);;
        }
        return _SetPortrait(portrait_image, character_name, side, facing);
    }

    public VisualElement SetPortrait(string character_name, string pose, StageDirection side, StageDirection facing) {
        Texture2D portrait_image = this.GetPortrait(character_name, pose);
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
        SetPortraitName(portrait, character_name);
        _SetPortraitSide(portrait, side);
        _SetPortraitFacing(portrait, facing);
        return portrait;
    }

    private void SetPortraitName(VisualElement portrait, string character_name) {
        string character_title = character_name;
        if (portrait_aliases.ContainsKey(character_name)) {
            (string _, string character_display) = portrait_aliases[character_name];
            character_title = character_display;
        }
        _SetPortraitName(portrait, character_title);
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

    public void DialogueFinished() {
        dialogue_completed = true;
        Debug.LogWarning("DialogueFinished!"); // TODO --- remove debug
        foreach (IGameEventEffect e in dialogue_callbacks) {
            e.ActivateEffect();
        }

        // WARNING: the following block of code is depricated
        if (dialogue_finished != null) {
            IGameEventEffect g_event = (IGameEventEffect) dialogue_finished;
            if (g_event != null) {
                g_event.ActivateEffect();
            } else {
                Debug.LogError("unable to use IGameEvent dialogue_finished");
            }
        }
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