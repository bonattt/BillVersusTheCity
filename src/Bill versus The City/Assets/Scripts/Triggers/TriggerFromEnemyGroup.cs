using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFromEnemyGroup : MonoBehaviour, IEnemyGroupSubscriber {
    public AbstractEnemyGroup target_enemy_group;
    [Tooltip("Script will trigger once this target value of enemies remains in the enemy group.")]
    public int target_remaining_count = 0;
    public bool trigger_only_once = true;
    private bool has_triggered = false;
    public GameObject target_event_obj;
    private IGameEventEffect _event;

    void Start() {
        target_enemy_group.Subscribe(this);
        _event = target_event_obj.GetComponent<IGameEventEffect>();
        if (_event == null) { Debug.LogError($"missing event on {target_event_obj.name} for {gameObject.name}"); }
    }

    public void Trigger() {
        has_triggered = true;

    }

    public void EnemyDefeated(IEnemyGroup group) {
        if (target_enemy_group.remaining_enemy_count <= target_remaining_count && !(has_triggered && trigger_only_once)) {
            has_triggered = true;
            Trigger();
        }
    }
}
