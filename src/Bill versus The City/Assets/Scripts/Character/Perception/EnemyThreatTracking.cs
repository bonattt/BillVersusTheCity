using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThreatTracking : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


public struct TrackedThreat {
    public ThreatType type;
    public float time;
    public Vector3 location;
}


public enum ThreatType {
    death,
    bullet
}
