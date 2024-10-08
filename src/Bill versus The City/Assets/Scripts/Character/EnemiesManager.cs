using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

 public class EnemiesManager : MonoBehaviour
{
    public List<EnemyController> enemies;

    public LayerMask layer_mask;

    public static EnemiesManager inst { get; protected set; }
    void Start() {
        if (inst != null) {
            Debug.LogWarning("replacing existing EnemiesManager");
        }
        inst = this;
    }


    public List<EnemyController> GetEnemies() {
        // returns a new list containing all the enemies in the current scene
        return new List<EnemyController>(enemies);
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
            else {
                Debug.LogWarning($"raycast hit {hit.transform.gameObject}");
            }
        } else {
            Debug.Log("raycast missed!");
        }
        
    }
}