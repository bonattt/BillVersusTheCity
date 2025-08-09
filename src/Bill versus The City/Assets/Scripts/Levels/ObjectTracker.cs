using System.Collections.Generic;
using UnityEngine;

public class ObjectTracker {
    // generic singleton class for tracking collections of objects in a scene. 
    // Provides a single place to reset all object types when a scene is unloaded

    private static ObjectTracker _inst;
    public static ObjectTracker inst {
        get {
            if (_inst == null) {
                _inst = new ObjectTracker();
            }
            return _inst;
        }
    }

    private List<DoorController> doors;

    public ObjectTracker() {
        Reset();
    }

    public void Reset() {
        doors = new List<DoorController>();
    }


    public void TrackObject(DoorController door) => doors.Add(door);
    public void RemoveObject(DoorController door) => doors.Remove(door);
    public IEnumerable<DoorController> AllDoors() { foreach (DoorController d in doors) yield return d; }

}