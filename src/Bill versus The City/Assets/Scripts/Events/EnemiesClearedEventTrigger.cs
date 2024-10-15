

using UnityEngine;

public class EnemiesClearedEventTrigger : MonoBehaviour, IGenericObserver {

    private IGameEvent game_event;
    public bool was_triggered = false;

    void Start() {
        game_event = GetComponent<IGameEvent>();
        if (game_event == null) {
            Debug.LogWarning("no game event found");
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
        game_event.ActivateEvent();
        GameObject prefab = game_event.GetNextEventPrefab();
        Destroy(this);
        if (prefab != null) {
            Instantiate(prefab);
        }
    }

    void OnDestroy() {
        EnemiesManager.inst.Unusubscribe(this);
    }
}