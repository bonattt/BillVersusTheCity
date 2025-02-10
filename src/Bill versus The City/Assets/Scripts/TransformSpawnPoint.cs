using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TransformSpawnPoint : MonoBehaviour, ISpawnPoint {
    public Vector3 GetSpawnPosition() {
        return transform.position;
    }
}