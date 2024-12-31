
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesClearedEventTrigger : MonoBehaviour, IGenericObserver {

    public bool was_triggered = false;

    public List<MonoBehaviour> init_game_events;
    private List<IGameEvent> game_events;

    void Start() {
        game_events = new List<IGameEvent>();
        // iterate in guaranteed order
        for(int i = 0; i < init_game_events.Count; i++) {
            try {
                game_events.Add((IGameEvent) init_game_events[i]);
            } catch (InvalidCastException) {
                Debug.LogError($"invalid IGameEvent {init_game_events[i].gameObject.name} included in game events list for {gameObject.name}");
            }
        }

        // if we still don't have any game events, try to find one by GetComponent
        if (game_events.Count == 0) {
            game_events = new List<IGameEvent>();
            IGameEvent game_event = GetComponent<IGameEvent>();
            if (game_event != null) {
                game_events.Add(game_event);
            } else {
                Debug.LogWarning($"unable to initialize game events for {gameObject.name}");
            }
        }
        EnemiesManager.inst.Subscribe(this);
    }

    public void UpdateObserver(IGenericObservable observable) {
        if (! was_triggered && EnemiesManager.inst.remaining_enemies <= 0) {
            was_triggered = true;
            Trigger();
        }
    }

    public void Trigger() {
        // iterate in guaranteed order
        for (int i = 0; i < game_events.Count; i++) {
            game_events[i].ActivateEvent();
        }
        Destroy(this);
    }

    void OnDestroy() {
        EnemiesManager.inst.Unusubscribe(this);
    }
}