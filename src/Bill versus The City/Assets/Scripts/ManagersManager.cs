using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagersManager : MonoBehaviour
{
    // this class creates a singleton for the managers prefab, so it doesn't get reinstantiated on level load if it already exists.
    public static ManagersManager inst { get; private set; }

    void Awake() {
        inst = this;
    }
}
