using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


 public class EnemiesManager : MonoBehaviour, IGenericObservable
{
    private HashSet<NavMeshAgentMovement> enemies = new HashSet<NavMeshAgentMovement>();
    private HashSet<NavMeshAgentMovement> enemies_defeated = new HashSet<NavMeshAgentMovement>();

    public int remaining_enemies {
        get {
            return enemies.Count - enemies_defeated.Count;
        }
    }

    public int total_enemies { get { return enemies.Count; } }

    public LayerMask layer_mask;

    public static EnemiesManager inst { get; protected set; }
    void Awake() {
        Initialize();
    }

    private void Initialize() {
        if (inst == null) {
            // Debug.LogWarning("setting `inst` to `this`");  // TODO --- remove debug
            inst = this;
        }
        if (inst != this) {
            Destroy(this);
            // Debug.LogWarning("removing redundant EnemiesManager");  // TODO --- remove debug
        } 
    }

    private HashSet<NavMeshAgentMovement> _enemies_remaining = null;
    public HashSet<NavMeshAgentMovement> GetRemainingEnemies() {
        // returns a new HashSet containing all the enemies in the current scene
        if (_enemies_remaining == null) {
            _enemies_remaining = new HashSet<NavMeshAgentMovement>(enemies);
            foreach (NavMeshAgentMovement e in enemies_defeated) {
                if (_enemies_remaining.Contains(e)) {
                    _enemies_remaining.Remove(e);
                }
            }
        }
        return _enemies_remaining;
    }

    public void Reset() {
        // resets the current and total enemies.
        enemies.Clear();
        enemies_defeated.Clear();
        Debug.Log($"Clear(): enemies.Count: {enemies.Count}");
        Debug.Log($"Clear(): enemies_defeated.Count: {enemies_defeated.Count}");
        UpdateSubscribers();
    }

    public void AddEnemy(NavMeshAgentMovement enemy) {
        enemies.Add(enemy);
        UpdateSubscribers();
    }

    // public void DestoryEnemy(EnemyController enemy) {
    //     enemies.Remove(enemy);
    //     enemies_defeated.Remove(enemy);
    //     UpdateSubscribers();
    // }

    public void DebugKillAll() {
        // debug action to kill all enemies
        foreach(NavMeshAgentMovement e in enemies) {
            e.GetStatus().health = -999;
        }
    }

    public void KillEnemy(NavMeshAgentMovement enemy) {
        // only add enemy to defeated enemies if it's actually in the scene
        // if the enemy is not in the scene, it's being cleaned up on a scene load
        if (enemies.Contains(enemy)) {
            enemies_defeated.Add(enemy);
        }
        UpdateSubscribers();
    }

    public void AlertAll() {
        // alerts all enemies currently in the map
        foreach (NavMeshAgentMovement ctrl in GetRemainingEnemies()) {
            EnemyPerception perception = ctrl.GetComponent<EnemyPerception>();
            if (perception == null) {
                Debug.LogError($"EnemyPerception is null for '{ctrl.gameObject}'!");
                continue;
            }
            perception.Alert();
        }
    }

    public void AlertEnemiesNear(Vector3 start) {
        // Alerts all nearby enemies to the given point
        foreach(NavMeshAgentMovement ctrl in GetRemainingEnemies()) {
            TryAlertOneEnemyNear(start, ctrl.GetComponent<EnemyPerception>());
        }
    }


    private void TryAlertOneEnemyNear(Vector3 start, EnemyPerception perception) {
        if (perception == null) {
            Debug.LogError("EnemyPerception is null!");
            return;
        }
        RaycastHit hit;
        Vector3 end =  perception.raycast_start.position;
        Vector3 direction = end - start;
        Debug.DrawRay(start, direction, Color.red, 1f); // ray appears for 1 second
        if (Physics.Raycast(start, direction, out hit, direction.magnitude, layer_mask)) {
            bool los_to_target = hit.transform == perception.transform;
            if (los_to_target) {
                perception.Alert();
            }
        }
        
    }

    private HashSet<IGenericObserver> subscribers = new HashSet<IGenericObserver>();
    public void Subscribe(IGenericObserver sub) => subscribers.Add(sub);
    public void Unusubscribe(IGenericObserver sub) => subscribers.Remove(sub);

    public void UpdateSubscribers() {
        _enemies_remaining = null;  // reset lazy property
        foreach(IGenericObserver sub in subscribers) {
            sub.UpdateObserver(this);
        }
    }


    public List<string> debug_enemies, debug_enemeis_defeated, debug_enemies_remaining;
    void Update() {
        UpdateDebug();
    }

    private void UpdateDebug() {
        debug_enemies = new List<string>();
        debug_enemies_remaining = new List<string>();
        debug_enemeis_defeated = new List<string>();
        foreach (NavMeshAgentMovement e in enemies) {
            debug_enemies.Add($"{e}");
        }
        foreach (NavMeshAgentMovement e in enemies_defeated) {
            debug_enemeis_defeated.Add($"{e}");
        }
        foreach (NavMeshAgentMovement e in GetRemainingEnemies()) {
            debug_enemies_remaining.Add($"{e}");
        }
    }
}