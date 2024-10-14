using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


 public class EnemiesManager : MonoBehaviour, IGenericObservable
{
    private HashSet<EnemyController> enemies = new HashSet<EnemyController>();
    private HashSet<EnemyController> enemies_defeated = new HashSet<EnemyController>();

    public int remaining_enemies {
        get {
            return enemies.Count - enemies_defeated.Count;
        }
    }

    private int _total_enemies = 0;
    public int total_enemies { get { return enemies.Count; } }

    public LayerMask layer_mask;

    public static EnemiesManager inst { get; protected set; }
    void Awake() {
        Initialize();
        
    }

    private void Initialize() {
        if (inst == null) {
            Debug.LogWarning("setting `inst` to `this`");  // TODO --- remove debug
            inst = this;
        }
        if (inst != this) {
            Destroy(this);
            Debug.LogWarning("removing redundant EnemiesManager");  // TODO --- remove debug
        } 
    }

    private HashSet<EnemyController> _enemies_remaining = null;
    public HashSet<EnemyController> GetRemainingEnemies() {
        // returns a new HashSet containing all the enemies in the current scene
        if (_enemies_remaining == null) {
            _enemies_remaining = new HashSet<EnemyController>(enemies);
            foreach (EnemyController e in enemies_defeated) {
                if (_enemies_remaining.Contains(e)) {
                    _enemies_remaining.Remove(e);
                }
            }
        }
        Debug.Log($"_enemies_remaining: {_enemies_remaining.Count}");
        return _enemies_remaining;
    }

    public void Reset() {
        // resets the current and total enemies.
        enemies = new HashSet<EnemyController>();
        enemies_defeated = new HashSet<EnemyController>();
        UpdateSubscribers();
    }

    public void AddEnemy(EnemyController enemy) {
        enemies.Add(enemy);
        UpdateSubscribers();
    }

    public void DestoryEnemy(EnemyController enemy) {
        enemies.Remove(enemy);
        enemies_defeated.Remove(enemy);
        UpdateSubscribers();
    }

    public void KillEnemy(EnemyController enemy) {
        enemies_defeated.Add(enemy);
        UpdateSubscribers();
    }

    public void AlertEnemiesNear(Vector3 start) {
        // Alerts all nearby enemies to the given point
        foreach(EnemyController ctrl in GetRemainingEnemies()) {
            TryAlertOneEnemy(start, ctrl.GetComponent<EnemyPerception>());
        }
    }


    private void TryAlertOneEnemy(Vector3 start, EnemyPerception perception) {
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
}