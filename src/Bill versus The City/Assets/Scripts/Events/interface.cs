using System;
using System.Collections.Generic;

using UnityEngine;

public interface IGameEventEffect {
    // TODO ---

    public bool effect_completed { get; }
    public void ActivateEffect();
    // public GameObject GetNextEventPrefab() { return null; }
}


public static class GameEventEffectUtil {
    public static List<IGameEventEffect> LoadEventsFromMonoBehaviour(List<MonoBehaviour> init_events) {
        List<IGameEventEffect> result = new List<IGameEventEffect>();
        for(int i = 0; i < init_events.Count; i++) {
            MonoBehaviour b = init_events[i];
            try {
                IGameEventEffect e = (IGameEventEffect) b;
                result.Add(e);
            } catch (InvalidCastException) {
                Debug.LogWarning($"script {b} cannot be cast to IGameEventEffect");
            }
        }
        return result;
    }
}