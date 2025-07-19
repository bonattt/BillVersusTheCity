using System.IO;
using UnityEngine;
using UnityEngine.UIElements;


public class DialoguePortrait : VisualElement {

#if UNITY_EDITOR
    // only supported in unity editor, the factory requires a no-args constructor, which should not be used in actual game code. 
    // It's only here as a convenience so I can drop this into the UI Editot
    public new class UxmlFactory : UxmlFactory<DialoguePortrait, UxmlTraits> { }
# endif

    public const float IMAGE_WIDTH = 300f;
    public const float IMAGE_HEIGHT = 400f;

    public Texture2D portrait { get; protected set; }
    private const string PORTRAIT_IMAGE_ELEMENT = "portrait";
    private VisualElement image_element;

    public float actual_position_left;

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
#if UNITY_EDITOR
    public DialoguePortrait() : this("bill") {
        // used for testing, game code should not use this constructor
    }
# endif
    
    public DialoguePortrait(string character_name) {
        this.name = character_name;
        AddToClassList("dialogue_portrait");

        image_element = new VisualElement();
        image_element.name = PORTRAIT_IMAGE_ELEMENT;
        image_element.style.width = IMAGE_WIDTH;
        image_element.style.height = IMAGE_HEIGHT;

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

    public void SetEmote(DialogueEmoteType emote_enum, float jiggle_duration=0.20f) {
        ClearEmote();
        this.emote_enum = emote_enum;
        emote_element = new DialogueEmote(emote_enum);
        Add(emote_element);

        emote_element.style.position = Position.Absolute;
        emote_element.style.top = -75f;
        emote_element.style.right = -25f;
        emote_element.JiggleFor(jiggle_duration);
    }
}


public class DialoguePortraitError : DialogueControllerException {
    public DialoguePortraitError(string msg) : base(msg) { /* do nothing */ }
}