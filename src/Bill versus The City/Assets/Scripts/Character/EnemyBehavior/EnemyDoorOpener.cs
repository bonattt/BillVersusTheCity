
using System.Collections.Generic;
using UnityEngine;

public class EnemyDoorOpener : MonoBehaviour {

    // use random numbers on evaluation time to stager evaluations, so every enemy doens't evaluate doors on the same frame
    private float min_evaluate_interval = 0.075f;
    private float max_evaluate_interval = 0.2f;
    private float next_evaluate = -1f;
    private HashSet<DoorController> tracked_doors = new HashSet<DoorController>();

    [Tooltip("any door within this distance will be opened")]
    public float open_door_range = 2.5f;

    public float GetNextEvaluateTime() {
        return Time.time + Random.Range(min_evaluate_interval, max_evaluate_interval);
    }

    void Update() {
        if (next_evaluate <= Time.time) {
            next_evaluate = GetNextEvaluateTime();
            Evaluate();
        }
    }

    private void Evaluate() {
        List<DoorController> doors_removed = new List<DoorController>();
        foreach (DoorController d in tracked_doors) {
            if (Vector3.Distance(transform.position, d.transform.position) > open_door_range) {
                doors_removed.Add(d);
                d.CloseDoor();
            }
        }
        tracked_doors.RemoveWhere((DoorController item) => doors_removed.Contains(item));

        foreach (DoorController d in ObjectTracker.inst.AllDoors()) {
            if (Vector3.Distance(transform.position, d.transform.position) < open_door_range) {
                tracked_doors.Add(d);
                if (!d.is_door_open) {
                    // open the door if it isn't open
                    d.OpenDoor();
                }
            }
        }
    }

}