using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

 public class EnemiesManager : MonoBehaviour
{
    private List<EnemyController> enemies = new List<EnemyController>();

    public LayerMask layer_mask;

    public static EnemiesManager inst { get; protected set; }
    void Awake() {
        Debug.Log($"Awake(): inst {inst}");  // TODO --- remove debug
        Initialize();
        Debug.Log($"AFTER Awake(): inst {inst}");  // TODO --- remove debug
    }

    void Start() {
        Debug.Log($"Start(): inst {inst}");  // TODO --- remove debug
        Initialize();
        Debug.Log($"AFTER Start(): inst {inst}");  // TODO --- remove debug
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


    public List<EnemyController> GetEnemies() {
        // returns a new list containing all the enemies in the current scene
        return new List<EnemyController>(enemies);
    }

    public void AddEnemy(EnemyController enemy) {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(EnemyController enemy) {
        enemies.Remove(enemy);
    }

    public void AlertEnemiesNear(Vector3 start) {
        // Alerts all nearby enemies to the given point
        foreach(EnemyController ctrl in enemies) {
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
}