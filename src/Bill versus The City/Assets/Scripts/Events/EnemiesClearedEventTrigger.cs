
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemiesClearedEventTrigger : MonoBehaviour, IGenericObserver {

    public bool was_triggered = false;

    public List<MonoBehaviour> init_game_events;
    private List<IGameEventEffect> game_events;
    public bool destroy_after_trigger = true;

    void Start() {
        game_events = new List<IGameEventEffect>();
        // iterate in guaranteed order
        for(int i = 0; i < init_game_events.Count; i++) {
            try {
                game_events.Add((IGameEventEffect) init_game_events[i]);
            } catch (InvalidCastException) {
                Debug.LogError($"invalid IGameEvent {init_game_events[i].gameObject.name} included in game events list for {gameObject.name}");
            }
        }

        // if we still don't have any game events, try to find one by GetComponent
        if (game_events.Count == 0) {
            game_events = new List<IGameEventEffect>();
            IGameEventEffect game_event = GetComponent<IGameEventEffect>();
            if (game_event != null) {
                game_events.Add(game_event);
            } else {
                Debug.LogWarning($"unable to initialize game events for {gameObject.name}");
            }
        }
        EnemiesManager.inst.Subscribe(this);
    }

    public void UpdateObserver(IGenericObservable observable) {
        if (! was_triggered && EnemiesManager.inst.remaining_enemy_count <= 0) {
            was_triggered = true;
            Trigger();
        }
    }

    public void Trigger() {
        // do not trigger if enemies are destroyed b/c of a level restart
        if (ScenesUtil.IsRestartInProgress()) { return; }
        
        // iterate in guaranteed order
        for (int i = 0; i < game_events.Count; i++) {
            game_events[i].ActivateEffect();
        }
        if (destroy_after_trigger) {
            Destroy(this); 
        }
    }

    void OnDestroy() {
        EnemiesManager.inst.Unusubscribe(this);
    }
}