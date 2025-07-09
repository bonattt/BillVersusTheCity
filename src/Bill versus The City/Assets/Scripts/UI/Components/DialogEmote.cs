using System.IO;
using UnityEngine;
using UnityEngine.UIElements;


public class DialogueEmote : VisualElement {

    private const string EMOTE_TEXTURE_RESOURCE = "emote-bubble";
    public const float WIDTH = 188f;
    public const float HEIGHT = 188f;
    public const float CONTENT_WIDTH = 100f;
    public const float CONTENT_HEIGHT = 100f;
    public const float SPEACH_BUBBLE_TAIL_HEIGHT = 25f;
    public const DialogueEmoteImage DEFAULT_EMOTE = DialogueEmoteImage.anger;
    private const string EMOTE_IMAGE_RESOURCE = "DialogueEmotes";
    private Sprite _emote_icon;

    public DialogueEmoteImage emote { get; private set; }
    public Sprite emote_icon {
        get => _emote_icon;
        set {
            // TODO --- implement
            _emote_icon = value;
        }
    }

    private VisualElement image_child;

    public DialogueEmote() {
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
        SetEmoteImage(DEFAULT_EMOTE);
    }

    public static Sprite SpriteFromEnume(DialogueEmoteImage emote_enum) {
        string path = Path.Combine(EMOTE_IMAGE_RESOURCE, $"{emote_enum}");
        Sprite sprite = Resources.Load<Sprite>(path);
        return sprite;
    }

    public void ClearEmoteImage() {
        image_child.style.backgroundImage = null;
    }

    public void SetEmoteImage(DialogueEmoteImage emote) {
        this.emote = emote;
        SetEmoteImage(SpriteFromEnume(emote));
    }
    private void SetEmoteImage(Sprite emote) {
        image_child.style.backgroundImage = new StyleBackground(emote);
    }

    public new class UxmlFactory : UxmlFactory<DialogueEmote, UxmlTraits> { }
}

public enum DialogueEmoteImage {
    alarm,
    anger,
    confusion,
    love,
    stress,
    sweat,
}