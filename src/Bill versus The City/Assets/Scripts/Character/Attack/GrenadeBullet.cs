using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBullet : MonoBehaviour, ITrackedProjectile
{
    // allows a grenade to be tracked as a threat by bullet tracking, so enemies run away from grenades
    public Transform location { get { return transform; } }
    public bool is_threat { get => true; }

    // Start is called before the first frame update
    void Start() {
        Debug.LogWarning($"tracking self as bullet {gameObject.name}"); // TODO --- remove debug    
        BulletTracking.inst.TrackNewBullet(this);
    }

    void OnDestroy() {
        BulletTracking.inst.UnTrackBullet(this);
    }
}
