using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBullet : MonoBehaviour, ITrackedProjectile
{
    // allows a grenade to be tracked as a threat by bullet tracking, so enemies run away from grenades

    public float _threat_level = 20; 
    public float threat_level { get => _threat_level; } 
    public Transform location { get { return transform; } }
    public bool is_threat { get => true; }

    // Start is called before the first frame update
    void Start() {
        BulletTracking.inst.TrackNewBullet(this);
    }

    void OnDestroy() {
        BulletTracking.inst.UnTrackBullet(this);
    }
}
