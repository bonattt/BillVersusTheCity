using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagersManager : MonoBehaviour
{
    // TODO --- remove this
    public static ManagersManager inst { get; private set; }

    void Awake() {
        inst = this;
    }
}
