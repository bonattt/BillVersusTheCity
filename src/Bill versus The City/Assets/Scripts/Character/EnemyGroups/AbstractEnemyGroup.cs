


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AbstractEnemyGroup : MonoBehaviour, IEnemyGroup, ICharStatusSubscriber {

    protected HashSet<NavMeshAgentMovement> all_enemies = new HashSet<NavMeshAgentMovement>();
    protected HashSet<NavMeshAgentMovement> enemies_defeated = new HashSet<NavMeshAgentMovement>();

    /// 
    /// IEnemyGroup
    /// 
    public virtual int total_enemy_count { get => all_enemies.Count; }
    public virtual int defeated_enemy_count { get => enemies_defeated.Count; }
    public virtual int remaining_enemy_count { get => all_enemies.Count - enemies_defeated.Count; }

    protected abstract void InitializeEnemies(); // populate all_enemies

    public void AlertAll() {
        foreach (NavMeshAgentMovement enemy in RemainingEnemies()) {
            EnemyPerception perception = enemy.GetComponent<EnemyPerception>();
            if (perception == null) {
                Debug.LogError($"EnemyPerception is null for '{enemy.gameObject.name}'!");
                continue;
            }
            perception.Alert();
        }
    }
    public virtual IEnumerable<NavMeshAgentMovement> AllEnemies() {
        foreach (NavMeshAgentMovement e in all_enemies) {
            yield return e;
        }
    }
    public virtual IEnumerable<NavMeshAgentMovement> DefeatedEnemies() {
        foreach (NavMeshAgentMovement e in enemies_defeated) {
            yield return e;
        }
    }
    public virtual IEnumerable<NavMeshAgentMovement> RemainingEnemies() {
        foreach (NavMeshAgentMovement e in all_enemies) {
            if (enemies_defeated.Contains(e)) { continue; }
            yield return e;
        }
    }

    /// 
    /// ICharStatusSubscriber
    /// 
    public void StatusUpdated(ICharacterStatus status) { /* do nothing */ }
    public void OnDamage(ICharacterStatus status) { /* do nothing */ }
    public void OnHeal(ICharacterStatus status) { /* do nothing */ }
    public void OnDeath(ICharacterStatus status) => TrackAsDefeated(status); // triggers immediately on death
    public void DelayedOnDeath(ICharacterStatus status) { /* do nothing by default */ } // triggers after a death animation finishes playing
    public void OnDeathCleanup(ICharacterStatus status) { /* do nothing by default */ } // triggers some time after death to despawn the character

    private void TrackAsDefeated(ICharacterStatus status) {
        NavMeshAgentMovement e = GetEnemyFromStatus(status);
        if (e == null) {
            Debug.LogError($"defeated enemy '{((MonoBehaviour)status).gameObject.name}' not found in tracked enemies!");
            return;
        }
        if (enemies_defeated.Contains(e)) {
            Debug.LogWarning($"defeated enemy '{((MonoBehaviour)status).gameObject.name}' already tracked as defeated!");
        }
        enemies_defeated.Add(e);
        EnemyDefeated();
    }

    protected NavMeshAgentMovement GetEnemyFromStatus(ICharacterStatus status) {
        foreach (NavMeshAgentMovement e in AllEnemies()) {
            if (e.GetStatus() == status) {
                return e;
            }
        }
        return null;
    }

    private List<IEnemyGroupSubscriber> subscribers = new List<IEnemyGroupSubscriber>();
    public void Subscribe(IEnemyGroupSubscriber sub) => subscribers.Add(sub);
    public void Unsubscribe(IEnemyGroupSubscriber sub) => subscribers.Remove(sub);
    public void EnemyDefeated() {
        foreach (IEnemyGroupSubscriber sub in subscribers) {
            sub.EnemyDefeated(this);
        }
    }

    protected virtual void Start() {
        InitializeEnemies();
        foreach (NavMeshAgentMovement e in AllEnemies()) {
            e.GetStatus().Subscribe(this);
        }
    }

    protected virtual void OnDestroy() {
        foreach (NavMeshAgentMovement e in AllEnemies()) {
            e.GetStatus().Unsubscribe(this);
        }
    }

    protected virtual void Update() {
        UpdateDebug();
    }

    [SerializeField]
    private EnemyGroupDebug debug = new EnemyGroupDebug();
    public void UpdateDebug() {
        if (!debug.update_debug) { return; }

        if (debug.last_updated_time + debug.debug_interval_seconds <= Time.time) {
            debug.last_updated_time = Time.time;
            debug.all_enemies = AllEnemies().ToList();
            debug.enemies_remaining = RemainingEnemies().ToList();
            debug.enemies_defeated = DefeatedEnemies().ToList();
        }
    }

}

[Serializable]
public class EnemyGroupDebug {
    public bool update_debug = true;
    public float debug_interval_seconds = 0.25f;
    public float last_updated_time = -1f;
    public List<NavMeshAgentMovement> all_enemies, enemies_remaining, enemies_defeated;
}