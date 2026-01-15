using System;
using System.Collections;
// using System.Collections.Generic.SortedList;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EnemiesManager : MonoBehaviour, IGenericObservable {
    protected HashSet<NavMeshAgentMovement> enemies = new HashSet<NavMeshAgentMovement>();
    protected HashSet<NavMeshAgentMovement> enemies_defeated = new HashSet<NavMeshAgentMovement>();

    public int remaining_enemy_count {
        get {
            return enemies.Count - enemies_defeated.Count;
        }
    }

    public int total_enemy_count { get { return enemies.Count; } }

    [Tooltip("layer mask used for alerting nearby enemies in line of sight.")]
    public LayerMask blocks_enemy_line_of_sight;

    [Tooltip("number of seconds to wait before revaluating which enemies are closest to the player.")]
    public float evaluation_period;
    private float next_nearest_enemy_at = -1; // time when the nearest enemy will next be evaluated

    [Tooltip("(TODO: reactivate this field) max number of enemies that will directly fight the player at once; other enemies will retreat to cover.")]
    [HideInInspector]
    [SerializeField]
    private int _max_engaged_enemies = 5;
    public const int MAX_ENGAGED_ENEMIES = 3;
    public int max_engaged_enemies {
        // get => _max_engaged_enemies; // TODO --- this will be difficulty adjusted
        get => MAX_ENGAGED_ENEMIES; 
    }

    public static EnemiesManager inst { get; protected set; }
    void Awake() {
        Initialize();
    }

    void Update() {
        if (Time.time >= next_nearest_enemy_at) {
            EvaluateNearestEnemies();
            next_nearest_enemy_at = Time.time + evaluation_period;
        }
        UpdateDebug();
    }

    private void EvaluateNearestEnemies() {
        SortedList<float, NavMeshAgentMovement> sorted_enemies = GetEnemyDistance();
        int i = 0;
        debug.engaged_enemies = 0;
        debug.reserved_enemies = 0;
        // iterate in sorted order
        foreach (NavMeshAgentMovement enemy in sorted_enemies.Values) {
            // use loop counter to track how many enemies were already set to engaged.
            if (i < max_engaged_enemies && EnemyCannotEngagePlayer(enemy, PlayerCharacter.inst.player_transform)) {
                enemy.managed_enemy_state = ManagedEnemyState.reserve;
                debug.reserved_enemies += 1;
                continue;
            }
            i += 1;
            if (i < max_engaged_enemies) {
                enemy.managed_enemy_state = ManagedEnemyState.engaged;
                debug.engaged_enemies += 1;
                Debug.DrawLine(PlayerCharacter.inst.player_transform.position, enemy.transform.position, new Color(1f, 0.65f, 0f), evaluation_period); // ORANGE
            } else {
                enemy.managed_enemy_state = ManagedEnemyState.reserve;
                debug.reserved_enemies += 1;
            }
        }
    }

    public static bool EnemyCannotEngagePlayer(NavMeshAgentMovement enemy, Transform player) {
        if(!enemy.perception.is_alert) return true; // if the enemy is not alert, they cannot engage the player.
        // using hearing like this blocks the enemies from ever persuing the player through a closed door... so I need another solution
        // HearingHit hit = EnemyHearingManager.inst.GetHearingHitWithNavMesh(GetSound(player), enemy.perception);
        // return hit == null; // no hearing hit means the enemy is blocked from engaging
        return false;
    }

    private static ISound GetSound(Transform player) {
        // sound used to check when enemies are blocked from engaging the player
        GameSound sound = new GameSound(player.position, float.PositiveInfinity, 0);
        return sound;
    }

    private SortedList<float, NavMeshAgentMovement> GetEnemyDistance() => GetEnemyDistance(
            PlayerCharacter.inst.player_transform.position, GetRemainingEnemies());
    private SortedList<float, NavMeshAgentMovement> GetEnemyDistance(Vector3 position, HashSet<NavMeshAgentMovement> remaining_enemies) {
        SortedList<float, NavMeshAgentMovement> dist = new SortedList<float, NavMeshAgentMovement>();
        foreach (NavMeshAgentMovement enemy in remaining_enemies) {
            dist.Add(Vector3.Distance(position, enemy.transform.position), enemy);
        }
        return dist;
    }

    private void Initialize() {
        if (inst == null) {
            inst = this;
        }
        if (inst != this) {
            Destroy(this);
        }
    }
    
    public IEnumerable<NavMeshAgentMovement> Iterator() {
        foreach (NavMeshAgentMovement e in enemies_defeated) {
            yield return e;
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
        foreach (NavMeshAgentMovement e in enemies) {
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
        foreach (NavMeshAgentMovement ctrl in GetRemainingEnemies()) {
            TryAlertOneEnemyNear(start, ctrl.GetComponent<EnemyPerception>());
        }
    }


    private void TryAlertOneEnemyNear(Vector3 start, EnemyPerception perception) {
        if (perception == null) {
            Debug.LogError("EnemyPerception is null!");
            return;
        }
        RaycastHit hit;
        Vector3 end = perception.raycast_start.position;
        Vector3 direction = end - start;
        Debug.DrawRay(start, direction, Color.red, 1f); // ray appears for 1 second
        if (Physics.Raycast(start, direction, out hit, direction.magnitude, blocks_enemy_line_of_sight)) {
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
        foreach (IGenericObserver sub in subscribers) {
            sub.UpdateObserver(this);
        }
    }


    public EnemiesManagerDebugger debug;
    private void UpdateDebug() {
# if UNITY_EDITOR
        debug.enemies = new List<string>();
        debug.enemies_remaining = new List<string>();
        debug.enemeis_defeated = new List<string>();
        foreach (NavMeshAgentMovement e in enemies) {
            debug.enemies.Add($"{e}");
        }
        foreach (NavMeshAgentMovement e in enemies_defeated) {
            debug.enemeis_defeated.Add($"{e}");
        }
        foreach (NavMeshAgentMovement e in GetRemainingEnemies()) {
            debug.enemies_remaining.Add($"{e}");
        }
# endif
    }
}

public enum ManagedEnemyState {
    engaged,
    reserve,
}

[Serializable]
public class EnemiesManagerDebugger {
    public int engaged_enemies = -1;
    public int reserved_enemies = -1;
    public List<string> enemies, enemeis_defeated, enemies_remaining;
}