

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FlashbangedUI : MonoBehaviour {
    
    public UIDocument ui_doc;
    public float blind_until { get; private set; }
    public float dazed_until { get; private set; }

    private VisualElement panel;

    public bool active => true;


    public static FlashbangedUI inst { get; private set; }
    void Awake() {
        inst = this;
    }

    void Start() {
        Init();
        UpdateOpacity();
    }

    void Update() {
        if (active) {
            UpdateOpacity();
        }
        UpdateDebug();
    }

    private void Init() {
        panel = ui_doc.rootVisualElement.Q<VisualElement>("flashbang");

    }

    private void UpdateOpacity() {
        Color c =  new Color(1f, 1f, 1f, GetPercent());
        Debug.LogWarning("UpdateOpacity"); // TODO --- remove debug
        panel.style.backgroundColor = c;
    }

    private float GetPercent() {
        if(!active) { return 0f; }
        if (blind_until >= Time.time) { return 1f; }
        if (dazed_until <= Time.time) { return 0f; }
        
        float time_recovering = Time.time - blind_until; // seconds after blind starts fading
        float period = dazed_until - blind_until; // time it takes to go from fully opaque at the midpoint to fully transparent

        float percent = (period - time_recovering) / period;
        return percent;
    }

    public void FlashbangUntil(float blind_duration) {
        Debug.LogWarning($"blind duration: {blind_duration}");
        blind_until = Time.time + blind_duration;
        dazed_until = Time.time + (blind_duration * 3);
        UpdateDebug(); // TODO --- remove debug
        EditorApplication.isPaused = true; // TODO --- remove debug
    }


    public FlashbangedUIDebugger debug;
    private void UpdateDebug() {
        debug.percent_blind = GetPercent();
        debug.blind_until = blind_until;
        debug.dazed_until = dazed_until;
        debug.blind_remaining = Mathf.Max(0, blind_until - Time.time);
        debug.daze_remaining =  Mathf.Max(0, dazed_until - Time.time);
    }

}

[Serializable]
public class FlashbangedUIDebugger {
    public float percent_blind, blind_until, dazed_until, blind_remaining, daze_remaining;
}