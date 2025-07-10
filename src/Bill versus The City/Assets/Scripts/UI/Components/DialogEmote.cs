using System.IO;
using UnityEngine;
using UnityEngine.UIElements;


public class DialogueEmote : VisualElement {
    public new class UxmlFactory : UxmlFactory<DialogueEmote, UxmlTraits> { }
    private const string EMOTE_TEXTURE_RESOURCE = "emote-bubble";
    public const float WIDTH = 125f;
    public const float HEIGHT = 125f;
    public const float CONTENT_WIDTH = 100f;
    public const float CONTENT_HEIGHT = 100f;
    public const float SPEACH_BUBBLE_TAIL_HEIGHT = 25f;
    public const DialogueEmoteType DEFAULT_EMOTE = DialogueEmoteType.anger;
    private const string EMOTE_IMAGE_RESOURCE = "DialogueEmotes";
    private Sprite _emote_icon;

    public DialogueEmoteType emote { get; private set; }
    public Sprite emote_icon {
        get => _emote_icon;
        set {
            // TODO --- implement
            _emote_icon = value;
        }
    }

    private VisualElement image_child;

    public DialogueEmote() : this(DEFAULT_EMOTE) { /* do nothing */ }
    public DialogueEmote(DialogueEmoteType emote_type) {
        /* do nothing */
        style.backgroundImage = new StyleBackground(Resources.Load<Sprite>(EMOTE_TEXTURE_RESOURCE));
        style.width = WIDTH;
        style.height = HEIGHT;

        image_child = new VisualElement();
        Add(image_child);

        style.justifyContent = Justify.Center;
        image_child.style.alignSelf = Align.Center;
        image_child.style.position = Position.Relative;
        image_child.style.bottom = SPEACH_BUBBLE_TAIL_HEIGHT / 2;
        image_child.style.width = CONTENT_WIDTH;
        image_child.style.height = CONTENT_HEIGHT;
        SetEmoteImage(emote_type);
    }

    public static Sprite SpriteFromEnume(DialogueEmoteType emote_enum) {
        string path = Path.Combine(EMOTE_IMAGE_RESOURCE, $"{emote_enum}");
        Sprite sprite = Resources.Load<Sprite>(path);
        return sprite;
    }

    public static string EmoteToString(DialogueEmoteType emote) {
        return $"{emote}";
    }

    public static DialogueEmoteType StringToEmote(string emote_name) {
        switch (emote_name) {
            case "":
                return DialogueEmoteType.none;
            case "none":
                return DialogueEmoteType.none;
            case "clear":
                return DialogueEmoteType.none;

            case "alarm":
                return DialogueEmoteType.alarm;
            case "anger":
                return DialogueEmoteType.anger;
            case "confusion":
                return DialogueEmoteType.confusion;
            case "love":
                return DialogueEmoteType.love;
            case "stress":
                return DialogueEmoteType.stress;
            case "sweat":
                return DialogueEmoteType.sweat;
            default:
                throw new DialogueActionsException($"string '{emote_name}' cannot be converted to DialogueEmoteType!");
        }
    }

    public void ClearEmoteImage() {
        image_child.style.backgroundImage = null;
    }

    public void SetEmoteImage(DialogueEmoteType emote) {
        this.emote = emote;
        if (emote == DialogueEmoteType.none) {
            // DiaglogueEmote implements this as no image, however, classes above this should remove the emote alltogether for this setting
            Debug.LogWarning("emote set to DialogueEmoteImage.none, emote should be removed!");
            ClearEmoteImage();
        } else {
            SetEmoteImage(SpriteFromEnume(emote));
        }
    }
    private void SetEmoteImage(Sprite emote) {
        image_child.style.backgroundImage = new StyleBackground(emote);
    }
}

// public class DialogueEmoteError : DialogueActionsException {
//     public DialogueEmoteError(string msg) : base(msg) {}
// }


public enum DialogueEmoteType {
    none,
    alarm,
    anger,
    confusion,
    love,
    stress,
    sweat,
}