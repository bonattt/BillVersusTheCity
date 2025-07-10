
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueController : AbstractCloseEventMenu {
    // class controlling a single dialogue session

    private DialogueFile dialogue_file;

    public bool dialogue_completed { get; private set; }

    public UIDocument ui_doc;
    private VisualElement root, portraits_div, speaker_name_element;
    private Label dialogue_text, speaker_label;
    private VisualElement[] portrait_containers;
    private Dictionary<string, DialoguePortrait> character_portraits;

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
        character_portraits = new Dictionary<string, DialoguePortrait>();
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
        dialogue_text = root.Q<Label>("DialogueText");
        // speaker_left_element = root.Q<VisualElement>("Speaker");
        speaker_name_element = root.Q<VisualElement>("SpeakerName");
        speaker_label = speaker_name_element.Q<Label>();
        ConfigurePortraits();
        NextDialogueStep();  // assumeds `StartDialogue(file_path)` was called before first frame with dialouge open 
    }

    private void ConfigurePortraits() {
        portraits_div = root.Q<VisualElement>("PortraitDiv");
        portrait_containers = new VisualElement[4];

        // List<VisualElement> left_containers = new List<VisualElement>(left_portraits.Children());
        List<VisualElement> containers = portraits_div.Children().ToList();
        portrait_containers[0] = containers[0];
        portrait_containers[1] = containers[1];
        portrait_containers[2] = containers[2];
        portrait_containers[3] = containers[3];
        // left_portraits.Clear();
        // right_portraits.Clear();
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
        if (!character_portraits.ContainsKey(character_name)) {
            Debug.LogWarning($"cannot remove '{character_name}' from dialogug because it is not present!");
            return;
        }
        DialoguePortrait portrait = character_portraits[character_name];
        if (portrait.parent != null) {
            portrait.parent.Remove(portrait);
        }
    }

    public void UpdatePose(string character_name, string pose) {
        // updates just the pose on a character already in a scene

        if (!character_portraits.ContainsKey(character_name)) {
            throw new DialogueActionsException($"cannot update the pose for a character that's not in the dialogue '{character_name}'!");
        }
        DialoguePortrait portrait = character_portraits[character_name];
        Texture2D image = this.GetPortrait(character_name, pose);

        portrait.SetPortraitImage(image);
    }

    public void SetBlocking(StageDirection side, List<string> character_names) {
        // validate all characters exist in dialogue
        throw new NotImplementedException("Blocking actions are a WIP");
    }

    public Texture2D GetPortrait(string character_name) {
        // returns a portrait from PortraitSystem, using character aliases to look up the portrait name if there is an alias
        if (!portrait_aliases.ContainsKey(character_name)) {
            return PortraitSystem.GetPortrait(character_name);
        }
        (string portrait_name, string display_name) = portrait_aliases[character_name];
        return PortraitSystem.GetPortrait(portrait_name);
    }

    public Texture2D GetPortrait(string character_name, string pose) {
        // returns a portrait from PortraitSystem, using character aliases to look up the portrait name if there is an alias
        if (!portrait_aliases.ContainsKey(character_name)) {
            return PortraitSystem.GetPortrait(character_name, pose);
        }
        (string portrait_name, string display_name) = portrait_aliases[character_name];
        return PortraitSystem.GetPortrait(portrait_name, pose);
    }

    public VisualElement SetPortrait(string character_name, StagePosition position, StageDirection facing) {
        Texture2D portrait_image = null;
        if (!character_portraits.ContainsKey(character_name)) {
            // if the character is not already in the scene, load their default portrait.
            // Otherwise, leave as null to preserve their previous pose
            portrait_image = this.GetPortrait(character_name);
        }
        return _SetPortrait(portrait_image, character_name, position, facing);
    }

    public VisualElement SetPortrait(string character_name, string pose, StagePosition position, StageDirection facing) {
        Texture2D portrait_image = this.GetPortrait(character_name, pose);
        return _SetPortrait(portrait_image, character_name, position, facing);
    }

    private VisualElement _SetPortrait(Texture2D image, string character_name, StagePosition position, StageDirection facing) {
        // handles setting the portrait for the given character, whether they're in the scene or not
        // TODO --- handle character already in scene
        DialoguePortrait portrait;
        if (character_portraits.ContainsKey(character_name)) {
            portrait = character_portraits[character_name];
        } else {
            if (image == null) {
                throw new DialogueActionsException($"new characters in a scene cannot use a null image! '{character_name} {position} facing {facing}'");
            }
            // add new character to scene
            portrait = new DialoguePortrait(character_name);
            character_portraits[character_name] = portrait;
        }
        // pass image as null to preserve the previous image
        if (image != null) {
            portrait.SetPortraitImage(image);
        }

        // SetPortraitName(portrait, character_name); // handled by constructor
        SetPortraitPosition(portrait, position);
        if (facing == StageDirection.unspecified) {
            facing = GetUnspecifiedDefault(position);
        }
        portrait.facing = facing;
        return portrait;
    }

    public static StageDirection GetUnspecifiedDefault(StagePosition position) {
        switch (position) {
            case StagePosition.unspecified:
                Debug.LogWarning("default StageDirection for StagePosition.unspecified is `unspecifed`");
                return StageDirection.unspecified;
            case StagePosition.far_left | StagePosition.center_left:
                return StageDirection.left;
            case StagePosition.far_right | StagePosition.center_right:
                return StageDirection.right;

            default:
                Debug.LogError($"unhandled StagePosition `{position}`defaults to StageDirection.unspecified");
                return StageDirection.unspecified;
        }
    }

    public void UpdateEmote(string character_name, DialogueEmoteType emote) {
        if (!character_portraits.ContainsKey(character_name)) {
            throw new DialogueActionsException($"cannot update the emote for a character that's not in the dialogue '{character_name}'!");
        }
        DialoguePortrait character = character_portraits[character_name];
        UpdateEmote(character, emote);
    }
    public void UpdateEmote(DialoguePortrait actor, DialogueEmoteType emote) {
        if (emote == DialogueEmoteType.none) {
            actor.ClearEmote();
        } else {
            actor.SetEmote(emote);
        }
    }

    // private void SetPortraitName(DialoguePortrait portrait, string character_name) {
    //     _SetPortraitName(portrait, character_name);
    // }

        // private static void _SetPortraitName(DialoguePortrait portrait, string character_name) {
        //     Label name_label = portrait.Q<Label>();
        //     name_label.text = character_name;
        // }

        // private static void _SetPortraitImage(DialoguePortrait portrait, Texture2D image) {
        //     // takes a VisualElement and assigns the given image to that portrait
        //     VisualElement portrait_image = portrait.Q<VisualElement>(PORTRAIT_IMAGE_ELEMENT);
        //     portrait_image.style.backgroundImage = image;
        // }

        // private void _SetPortraitFacing(DialoguePortrait portrait, StageDirection facing) {
        //     // turns the image in a portrait to face the correct direction

        //     if (facing == StageDirection.left) {
        //         // display mirror image along the X axis
        //         portrait.Q<VisualElement>(PORTRAIT_IMAGE_ELEMENT).style.scale = new Scale(new Vector3(-1, 1, 1));
        //     } else if (facing == StageDirection.right) {
        //         // do nothing
        //     } else {
        //         Debug.LogWarning($"cannot set rotate character portrait '{portrait.name}' to face to the '{facing}'!");
        //     }
        // }

        // private const string PORTRAIT_IMAGE_ELEMENT = "portrait";

    private void SetPortraitPosition(DialoguePortrait portrait, StageDirection side) {
        SetPortraitPosition(portrait, GetPositionFromSide(side));
    }

    private void SetPortraitPosition(DialoguePortrait portrait, StagePosition position) {
        // int index = DialogueActionUtil.GetStagePositionIndex(position);
        if (position == StagePosition.unspecified) {
            if (portrait.parent == null) {
                Debug.LogError($"cannot set position of new portrait to {position}");
            } else {
                portrait.parent.Remove(portrait);
                Debug.LogWarning($"removing portrait {portrait} from dialogue because it was moved to position {position}");
            }
        } else {
            int index = DialogueActionUtil.GetStagePositionIndex(position);
            SetPortraitPosition(portrait, index);
        }
    }

    private void SetPortraitPosition(DialoguePortrait portrait, int index) {
        VisualElement container = GetPortraitContainer(index);
        if (container.childCount != 0) {
            Debug.LogWarning($"OVERWRITING PORTRAIT AT INDEX {index}");
            container.Clear();
        }
        container.Add(portrait);
    }

    private StagePosition GetPositionFromSide(StageDirection side) {
        switch (side) {
            case StageDirection.unspecified:
                return StagePosition.unspecified;
            case StageDirection.left:
                if (PositionOccupied(StagePosition.far_left)) {
                    return StagePosition.center_left;
                }
                return StagePosition.far_left;
            case StageDirection.right:
                if (PositionOccupied(StagePosition.far_right)) {
                    return StagePosition.center_right;
                }
                return StagePosition.far_right;
            default:
                throw new DialogueActionsException($"Unhandled case! side = {side}");
        }
    }

    public bool PositionOccupied(StagePosition position) {
        if (position == StagePosition.unspecified) {
            throw new DialogueActionsException($"unhandled StagePosition {position}");
        }
        int index = DialogueActionUtil.GetStagePositionIndex(position);
        return portrait_containers[index].childCount > 0;
    }

    // private VisualElement GetPortraitContainer(StagePosition position) {
    //     int index = DialogueActionUtil.GetStagePositionIndex(position);
    //     return GetPortraitContainer(index);
    // }
    private VisualElement GetPortraitContainer(int index) {
        return portrait_containers[index];
    }

    // public static VisualElement GetEmptyPortrait(string name) {
    //     // gets a portrait, with no image set
    //     VisualElement portrait = new VisualElement();
    //     portrait.name = name;
    //     portrait.AddToClassList("dialogue_portrait");

    //     Label character_name = new Label();
    //     character_name.text = "new character";
    //     character_name.AddToClassList("portrait_name_label");
    //     portrait.Add(character_name);

    //     VisualElement portrait_image = new VisualElement();
    //     portrait_image.name = PORTRAIT_IMAGE_ELEMENT;
    //     portrait_image.AddToClassList("dialogue_portrait_image");
    //     portrait.Add(portrait_image);

    //     return portrait;
    // }

    public void SetSpeakerName(string speaker_name) {
        if (speaker_name == null || speaker_name.Equals("*") || speaker_name.Equals("")) {
            speaker_name_element.style.visibility = Visibility.Hidden;
        } else {
            speaker_name_element.style.visibility = Visibility.Visible;
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;  // TODO --- investigate this deeper
            string speaker_display = speaker_name;
            if (portrait_aliases.ContainsKey(speaker_name)) {
                (string _, string _character_display) = portrait_aliases[speaker_name];
                speaker_display = _character_display;
            }

            // use underscores for spaces in the name of the speaker. Title Case names
            speaker_label.text = textInfo.ToTitleCase(speaker_display.ToLower().Replace("_", " "));
            _UpdateSpeakerLabelPosition(speaker_name);
        }
        // Debug.LogWarning($"Not Implelented: SetSpeakerName('{speaker_name}')");
    }
    private void _UpdateSpeakerLabelPosition(string character_name) {
        // takes the name of a speaker, and updates the position of the speaker label to be under that character's portrait. If the character 
        //   is not in the dialogue, the label is moved to the center
        int index = GetCharacterIndex(character_name);
        float per_index_percent = 100f / portrait_containers.Length; // percent out of 100, not ratio of 1
        float start_offset = -(per_index_percent * ((portrait_containers.Length/2) - 0.5f)); // offset to position label in the center of index 0's portrait
        float percent_from_left;
        if (index <= -1) {
            // if the character is not on screen, position label at the center
            // percent_from_left = zero_offset + (per_index_percent * ((portrait_containers.Length/2) - 0.5f)); // works for even numbers of portraits to put the label between middle 2 portraits // works if centered from left...
            percent_from_left = 0f; // label position is center justified
        } else if (index < portrait_containers.Length) {
            percent_from_left = start_offset + (per_index_percent * index);
        } else {
            Debug.LogError($"character index calculated to be greater than number of portraits!");
            percent_from_left = 0f; // center
        }
        speaker_name_element.style.position = Position.Relative;
        speaker_name_element.style.left = new Length(percent_from_left, LengthUnit.Percent);
    }

    private int GetCharacterIndex(string character_name) {
        // returns the index (StagePosition) the given character is currently positioned at.
        // if the given name is not on-screen, return -1

        if (!character_portraits.ContainsKey(character_name)) {
            return -1;
        }
        VisualElement target_portrait = character_portraits[character_name];
        for (int i = 0; i < portrait_containers.Length; i++) {
            if (portrait_containers[i] == target_portrait.parent) {
                return i;
            }
        }
        Debug.LogError($"Character {character_name} found in dict, but cannot match portrait index!");
        return -1; 
    }

    public void SetText(string new_dialouge) {
        dialogue_text.text = new_dialouge;
    }

    public void DialogueFinished() {
        dialogue_completed = true;
        foreach (IGameEventEffect e in dialogue_callbacks) {
            e.ActivateEffect();
        }

        // WARNING: the following block of code is depricated
        if (dialogue_finished != null) {
            IGameEventEffect g_event = (IGameEventEffect)dialogue_finished;
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