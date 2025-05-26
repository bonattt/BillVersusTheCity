using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAwarenessSound : MonoBehaviour, IPerceptionSubscriber
{
    // private static Dictionary<PerceptionState, string> display_value = new Dictionary<PerceptionState, string>{
    //     {PerceptionState.unaware, ""},
    //     {PerceptionState.searching, "?"},
    //     {PerceptionState.alert, "?"},
    //     {PerceptionState.seeing, "!"},
    // };
    // private static Dictionary<PerceptionState, Color> text_colors = new Dictionary<PerceptionState, Color>{
    //     {PerceptionState.unaware, Color.black},
    //     {PerceptionState.searching, Color.gray},
    //     {PerceptionState.alert, Color.yellow},
    //     {PerceptionState.seeing, Color.red},
    // };

    // private static Dictionary<PerceptionState, Color> outline_colors = new Dictionary<PerceptionState, Color>{
    //     {PerceptionState.unaware, Color.black},
    //     {PerceptionState.searching, Color.black},
    //     {PerceptionState.alert, Color.black},
    //     {PerceptionState.seeing, Color.black},
    // };

    public const string DEFAULT_ENEMY_ALERT_SOUND = "enemy_alert_sound";
    
    public List<PerceptionState> start_states = null;
    public List<PerceptionState> target_states = new List<PerceptionState>() {
            PerceptionState.alert, PerceptionState.seeing }; // , PerceptionState.searching };

    public string sound_name;

    void Start() {
        EnemyPerception perception = GetComponent<EnemyPerception>();
        perception.Subscribe(this);
    }
    void OnDestroy()
    {
        EnemyPerception perception = GetComponent<EnemyPerception>();
        perception.Unsubscribe(this);        
    }

    public string GetSoundPath()
    {
        if (sound_name == null || sound_name.Equals(""))
        {
            return DEFAULT_ENEMY_ALERT_SOUND;
        }
        return sound_name;
    }

    protected static bool StateMeetsConditions(ICollection<PerceptionState> conditions, PerceptionState state)
    {
        return conditions == null || conditions.Count == 0 || conditions.Contains(state);
    }

    public void UpdatePerceptionState(PerceptionState previous_state, PerceptionState new_state)
    {
        // if start state is null or empty, it should accept all previous start states (except states in `target_states`)
        bool from_start_state = StateMeetsConditions(start_states, previous_state);
        bool to_end_state = StateMeetsConditions(target_states, new_state) && !target_states.Contains(previous_state);
        if (from_start_state && to_end_state)
        {
            // only play a sound if the target transitioned from a desired start state into a desired target state 
            // Only play sound if the previous state ALSO wasn't one of the target states
            SFXSystem.inst.PlayRandomClip(SFXLibrary.LoadSound(GetSoundPath()), PlayerCharacter.inst.player_transform.position);
        }

    }
    
}
