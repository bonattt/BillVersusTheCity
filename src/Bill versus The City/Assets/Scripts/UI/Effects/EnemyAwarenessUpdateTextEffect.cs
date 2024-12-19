using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class EnemyAwarenessUpdateTextEffect : MonoBehaviour
{
    public UIDocument ui_doc;
    public string effect_text;
    public int font_size;
    private Label effect_label;
    public Color text_color = new Color(0.9f, 0.05f, 0.05f);
    public Color outline_color = new Color(0.55f, 0f, 0f);
    public Vector3 start_offset = new Vector3(0f, 0f, 0f);
    public Transform follow;
    public float fade_delay_seconds = 1f;  // how long before the effect starts to fade
    public float fade_duration = 2f; // how long does the effect take to fade once it starts fading
    
    [SerializeField]
    private float _time_created = 0f;


    void Start() {
        SetupEffect();
        UpdateEffect();
        transform.parent = follow;
        transform.localPosition = start_offset;
    }


    public virtual void SetupEffect() {
        text_color.a = 1f;
        outline_color.a = 1f;
        Debug.LogWarning($"ui_doc: {ui_doc}"); // TODO --- remove debug
        effect_label = ui_doc.rootVisualElement.Q<Label>();
        Debug.LogWarning($"effect_label: {effect_label}"); // TODO --- remove debug
        _time_created = Time.time;
    }
    public virtual void UpdateEffect() {
        effect_label.text = effect_text;
        effect_label.style.fontSize = font_size;
        effect_label.style.color = text_color;
        effect_label.style.unityTextOutlineColor = outline_color;
    }
    private void UpdateFade() {
        float effect_time_passed = Mathf.Max(0, Time.time - _time_created - fade_delay_seconds);
        float fade = effect_time_passed / fade_duration;
        float alpha = Mathf.Min(1f, Mathf.Max(0f, 1 - fade));
        text_color.a = alpha;
        outline_color.a = alpha;
    }

    void Update() {
        UpdateFade();
        UpdateEffect();
    }

}
