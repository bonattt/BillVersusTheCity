using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAwarenessDisplay : MonoBehaviour, IPerceptionSubscriber
{
    GameObject previous_display = null;

    public const string PREFAB_RESOURCE = "EnemyAwarenessUpdate";
    private static Dictionary<PerceptionState, string> display_value = new Dictionary<PerceptionState, string>{
        {PerceptionState.unaware, ""},
        {PerceptionState.searching, "?"},
        {PerceptionState.alert, "?"},
        {PerceptionState.seeing, "!"},
    };
    private static Dictionary<PerceptionState, Color> text_colors = new Dictionary<PerceptionState, Color>{
        {PerceptionState.unaware, Color.black},
        {PerceptionState.searching, Color.gray},
        {PerceptionState.alert, Color.yellow},
        {PerceptionState.seeing, Color.red},
    };

    private static Dictionary<PerceptionState, Color> outline_colors = new Dictionary<PerceptionState, Color>{
        {PerceptionState.unaware, Color.black},
        {PerceptionState.searching, Color.black},
        {PerceptionState.alert, Color.black},
        {PerceptionState.seeing, Color.black},
    };

    void Start() {
        EnemyPerception perception = GetComponent<EnemyPerception>();
        perception.Subscribe(this);
    }

    public void UpdatePerceptionState(PerceptionState previous_state, PerceptionState new_state) {
        if (previous_display != null) {
            Destroy(previous_display);
        }
        if (new_state == PerceptionState.dead || previous_state == PerceptionState.dead) {
            return; // do nothing if dead
        }

        previous_display = Instantiate(Resources.Load<GameObject>(PREFAB_RESOURCE));
        EnemyAwarenessUpdateTextEffect awareness_update = previous_display.GetComponent<EnemyAwarenessUpdateTextEffect>();
        
        awareness_update.text_color = text_colors[new_state];
        awareness_update.outline_color = outline_colors[new_state];
        awareness_update.effect_text = display_value[new_state];
        awareness_update.font_size = 80;
        awareness_update.follow = transform;
        awareness_update.transform.localPosition = Vector3.zero;

    }
    
}
