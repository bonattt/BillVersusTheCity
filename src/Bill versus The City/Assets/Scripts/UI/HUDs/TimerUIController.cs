using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class TimerUIController : MonoBehaviour
{
    public Color text_color = Color.red;
    public UIDocument ui_doc;
    // public MonoBehaviour init_timer;
    private ITimer timer;

    private VisualElement root;
    private Label digital_clock;
    

    public string GetText() {
        return $"{ZeroPadNumber(timer.remaining_minutes)} : {ZeroPadNumber(timer.remaining_seconds_remainder)}";
    }

    public string ZeroPadNumber(int number) {
        if (number < 10) {
            return $"0{number}";
        }
        return $"{number}";
    }

    // // Start is called before the first frame update
    // void Start()
    // {
    //     if (init_timer == null) {
    //         DetachTimer();
    //     } else {
    //         AttachTimer((ITimer) init_timer);
    //     }
    // }

    void Update() {
        UpdateClock();
    }

    private void UpdateClock(){
        if (digital_clock == null) { return; } // no clock configured, do nothing
        digital_clock.text = GetText();
    }

    public void AttachTimer(ITimer new_timer) {
        this.timer = new_timer;
        if (this.timer == null) {
            Debug.LogWarning("AttachTimer set to null. use DetachTimer instead!");
            DetachTimer();
            return;
        }
        root = ui_doc.rootVisualElement;
        digital_clock = root.Q<Label>("CountdownText");
        digital_clock.style.color = text_color;
        UpdateClock();
    }

    public void DetachTimer() {
        timer = null;
        digital_clock = null;
    }
}
