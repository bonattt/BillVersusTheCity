using UnityEngine;
using UnityEngine.UIElements;

public class RadialProgress : VisualElement
{
    // Adapted from example in unity tutorial: https://docs.unity3d.com/6000.0/Documentation/Manual/UIE-radial-progress-use-vector-api.html
    /* An element that displays progress inside a partially filled circle */

    // These are USS class names for the control overall and the label.
    public static readonly string ussClassName = "radial-progress";
    public static readonly string ussLabelClassName = "radial-progress__label";

    // These objects allow C# code to access custom USS properties.
    static CustomStyleProperty<Color> s_TrackColor = new CustomStyleProperty<Color>("--track-color");
    static CustomStyleProperty<Color> s_ProgressColor = new CustomStyleProperty<Color>("--text-color");

    private Color _TrackColor = Color.gray;
    private Color _progress_color = Color.red;

    public RadialProgressText progress_label_type = RadialProgressText.percent;

    // This is the label that displays the percentage.
    private Label _label;

    // This is the number that the Label displays as a percentage.
    private float _progress;

    // A value between 0 and 100
    public float progress
    {
        // The progress property is exposed in C#.
        get => _progress;
        set
        {
            // Whenever the progress property changes, MarkDirtyRepaint() is named. This causes a call to the
            // generateVisualContents callback.
            _progress = value;
            UpdateLabel();
            MarkDirtyRepaint();
        }
    }

    public string _progress_label = "reload";
    public string progress_label {
        get => _progress_label;
        set {
            _progress_label = value;
            UpdateLabel();
        }
    }
    private void UpdateLabel() {
        if (progress_label_type == RadialProgressText.percent) {
            _label.text = Mathf.Clamp(Mathf.Round(_progress), 0, 100) + "%";
        } else if (progress_label_type == RadialProgressText.custom_text) {
            _label.text = progress_label;
        } else if (progress_label_type == RadialProgressText.none) {
            _label.text = "";
        } else {
            Debug.LogError($"unhandled progress_label_type '{progress_label_type}'");
        }
    }

    private void StyleLabelToCenter() {
        // Label
        _label.style.unityTextAlign = TextAnchor.MiddleCenter;
        _label.style.alignSelf = Align.Center;
        _label.style.justifyContent = Justify.Center;
        
        // parent
        this.style.flexDirection = FlexDirection.Column;
        this.style.justifyContent = Justify.Center;
        this.style.alignItems = Align.Center; 
    }

    // This default constructor is RadialProgress's only constructor.
    public RadialProgress()
    {
        // Create a Label, add a USS class name, and add it to this visual tree.
        _label = new Label();
        _label.AddToClassList(ussLabelClassName);
        StyleLabelToCenter();
        Add(_label);
        UpdateLabel();

        // Add the USS class name for the overall control.
        AddToClassList(ussClassName);

        // Register a callback after custom style resolution.
        RegisterCallback<CustomStyleResolvedEvent>(evt => CustomStylesResolved(evt));

        // Register a callback to generate the visual content of the control.
        generateVisualContent += GenerateVisualContent;

        progress = 0.0f;
    }

    static void CustomStylesResolved(CustomStyleResolvedEvent evt)
    {
        RadialProgress element = (RadialProgress)evt.currentTarget;
        element.UpdateCustomStyles();
    }

    // After the custom colors are resolved, this method uses them to color the meshes and (if necessary) repaint
    // the control.
    void UpdateCustomStyles()
    {
        bool repaint = false;
        if (customStyle.TryGetValue(s_ProgressColor, out _progress_color))
            repaint = true;

        if (customStyle.TryGetValue(s_TrackColor, out _TrackColor))
            repaint = true;

        if (repaint)
            MarkDirtyRepaint();
    }

    void GenerateVisualContent(MeshGenerationContext context)
    {
        float width = contentRect.width;
        float height = contentRect.height;

        var painter = context.painter2D;
        painter.lineWidth = 10.0f;
        painter.lineCap = LineCap.Butt;

        // Draw the track
        painter.strokeColor = _TrackColor;
        painter.BeginPath();
        painter.Arc(new Vector2(width * 0.5f, height * 0.5f), width * 0.5f, 0.0f, 360.0f);
        painter.Stroke();

        // Draw the progress
        painter.strokeColor = _progress_color;
        painter.BeginPath();
        painter.Arc(new Vector2(width * 0.5f, height * 0.5f), width * 0.5f, -90.0f, 360.0f * (progress / 100.0f) - 90.0f);
        painter.Stroke();
    }
    public new class UxmlFactory : UxmlFactory<RadialProgress, UxmlTraits> { }
}

public enum RadialProgressText {
    none,
    percent,
    custom_text,
}