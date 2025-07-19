using UnityEngine;
using UnityEngine.UIElements;


public enum TimeMode {
    use_unscaled_time,
    use_scaled_time,
}
public class ElementJiggle : MonoBehaviour {

    public float jiggle_intensity = 2f;   // Max pixel offset
    public float jiggle_speed = 30f;      // How fast it jiggles

    [Tooltip("how long should the element jiggle for.")]
    public float jiggle_duration_seconds = 1f;

    public bool jiggle_x = false;
    public bool jiggle_y = true;

    public TimeMode time_mode = TimeMode.use_unscaled_time;

    private VisualElement target;
    private Vector2 base_position;

    private float jiggle_start_time = float.NegativeInfinity;
    public bool is_jiggling {
        get {
            return jiggle_start_time + jiggle_duration_seconds >= GetTime();
        }
    }

    public void SetTarget(VisualElement new_target) {
        if (is_jiggling) {
            Debug.LogWarning($"changed target while active!!");
        }
        target = new_target;
        base_position = target.resolvedStyle.translate;
    }

    // void OnEnable() {
    //     Debug.LogWarning("OnEnable"); // TODO --- remove debug
    //     StartJiggle();
    // }

    public void StartJiggle() {
        jiggle_start_time = GetTime();
        // if (ui_doc == null) {
        //     Debug.LogError("UI Document not assigned.");
        //     return;
        // }

        // target = ui_doc.rootVisualElement.Q(target_name);
        // if (target == null) {
        //     Debug.LogError($"VisualElement '{target_name}' not found.");
        //     return;
        // }

        // Save original position
    }

    public float GetTime() {
        switch (time_mode) {
            case TimeMode.use_unscaled_time:
                return Time.unscaledTime;

            case TimeMode.use_scaled_time:
                return Time.time;

            default:
                Debug.LogWarning($"un-handled time mode!");
                break;
        }
        return Time.unscaledTime;
    }

    void Update() {
        if (is_jiggling) {
            ExecuteJiggle(GetTime());
        }
        UpdateDebug();
    }

    private void ExecuteJiggle(float time) {
        float offset_x;
        if (jiggle_x) {
            offset_x = Mathf.PerlinNoise(time * jiggle_speed, 0f) - 0.5f;
        } else {
            offset_x = 0f;
        }
        float offset_y;
        if (jiggle_y) {
            offset_y = Mathf.PerlinNoise(0f, time * jiggle_speed) - 0.5f;
        } else {
            offset_y = 0f;
        }

        offset_x *= jiggle_intensity * 2f;
        offset_y *= jiggle_intensity * 2f;

        target.style.translate = new StyleTranslate(new Translate(offset_x, offset_y, 0));
    }

    void OnDisable() {
        Debug.LogWarning("OnDisable"); // TODO --- remove debug
        StopJiggle();
    }

    public void StopJiggle() {
        jiggle_start_time = float.NegativeInfinity;
        if (target != null)
            target.style.translate = new StyleTranslate(new Translate(0, 0, 0));
    }


    //////////////////////////////////////////////// DEBUG STUFF ////////////////////////////////////
    public ElementJiggleDebugger _debug;
    
    void UpdateDebug() {
        if (_debug == null) {
            Debug.Log("`debug` not set for displaying debug data");
            return;
        }
        _debug.is_jiggling = is_jiggling;
        _debug.time = GetTime();
        _debug.start_time = jiggle_start_time;
        _debug.finish_time = jiggle_start_time + jiggle_duration_seconds;
    }
}

[Tooltip("data class for ElementJiggle debug data")]
[System.Serializable]
public class ElementJiggleDebugger {
    public bool is_jiggling;

    public float time, start_time, finish_time;

}