using System.IO;
using UnityEngine;
using UnityEngine.UIElements;


public class DialoguePortrait : VisualElement {

    public Texture2D portrait { get; protected set; }
    private const string PORTRAIT_IMAGE_ELEMENT = "portrait";
    private VisualElement image_element;
    private Label name_label;

    public DialogueEmoteType emote_enum { get; private set; }
    private DialogueEmote emote_element;

    private StageDirection _facing = StageDirection.unspecified;
    public StageDirection facing {
        get {
            return _facing;
        }
        set {
            if (image_element == null) {
                throw new DialoguePortraitError("portrait element is missing!");
            }

            if (value == StageDirection.left) {
                // display mirror image along the X axis
                image_element.style.scale = new Scale(new Vector3(-1, 1, 1));
            } else if (value == StageDirection.right) {
                // display image normal (not mirrored)
                image_element.style.scale = new Scale(new Vector3(1, 1, 1));
            } else {
                throw new DialoguePortraitError($"cannot set portrait facing to unspecified: {value}");
            }
            _facing = value;
        }
    }

    public string name_label_text {
        get => name_label.text;
        set { name_label.text = value; }
    }

    // public new class UxmlFactory : UxmlFactory<DialoguePortrait, UxmlTraits> { }
    // public DialoguePortrait() : this("bill") {
    //     // NOTE: this constructor is mainly for testing
    //     Debug.LogWarning("SECOND!!!"); // TODO --- remove debug
    //     Debug.LogWarning("no args DialoguePortrait is mainly for testing, and doesn't support name aliasing!");
    //     Texture2D new_portrait = PortraitSystem.GetPortrait(this.name);
    //     SetPortraitImage(new_portrait);
    //     SetEmote(DialogueEmoteType.anger);
    // }
    public DialoguePortrait(string character_name) {
        this.name = character_name;
        AddToClassList("dialogue_portrait");

        name_label = new Label();
        name_label.text = "new character";
        name_label.AddToClassList("portrait_name_label");
        Add(name_label);

        image_element = new VisualElement();
        image_element.name = PORTRAIT_IMAGE_ELEMENT;
        image_element.AddToClassList("dialogue_portrait_image");
        Add(image_element);
    }

    public void SetPortraitImage(Texture2D image) {
        // NOTE: DialoguePortrait doesn't handle looking up protraits from character_name b/c a dialogue controller has the aliasing that may apply for image looku
        portrait = image;
        image_element.style.backgroundImage = portrait;
    }

    public void ClearEmote() {
        emote_enum = DialogueEmoteType.none;
        if (emote_element != null) {
            Remove(emote_element);
            emote_element = null;
        }
    }

    public void SetEmote(DialogueEmoteType emote_enum) {
        ClearEmote();
        this.emote_enum = emote_enum;
        emote_element = new DialogueEmote(emote_enum);
        Add(emote_element);

        emote_element.style.position = Position.Absolute;
        emote_element.style.top = 0f;
        emote_element.style.right = 0f;
    }
}


public class DialoguePortraitError : DialogueControllerException {
    public DialoguePortraitError(string msg) : base(msg) { /* do nothing */ }
}