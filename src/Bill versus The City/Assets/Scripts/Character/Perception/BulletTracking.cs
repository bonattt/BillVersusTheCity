using System.Collections.Generic;

using UnityEngine;

public class BulletTracking : MonoBehaviour {

    private static BulletTracking _inst;
    public static BulletTracking inst {
        get {
            return _inst;
        }
    }

    [Tooltip("track bullet hits for this many seconds before discarding them")]
    public float track_hits_for_seconds = 5f;
    private HashSet<IBullet> tracked_bullets = new HashSet<IBullet>();
    // stored hits, implicitly (not explicitly) sorted by time of hit. Most recent hits are stored at the end, oldest hits are stored at the beginning
    private LinkedList<TrackedHit> tracked_hits = new LinkedList<TrackedHit>(); 

    void Awake() {
        _inst = this;
    }

    void Update() {
        RemoveOldHits();
    }

    private void RemoveOldHits() {
        while(tracked_hits.First != null && tracked_hits.First.Value.time < (Time.time - track_hits_for_seconds)) {
            Debug.LogWarning($"{Time.time}: removing {HitOrMissString(tracked_hits.First.Value.hit)} from {tracked_hits.First.Value.time}"); // TODO --- remove debug
            tracked_hits.RemoveFirst();
        }
    }

    public void TrackNewBullet(IBullet bullet) {
        tracked_bullets.Add(bullet);
        Debug.LogWarning($"track bullet {gameObject.name}. {tracked_bullets.Count} bullets now tracked!"); // TODO --- remove debug
    }
    public void UnTrackBullet(IBullet bullet) {
        tracked_bullets.Remove(bullet);
        Debug.LogWarning($"UnTrackBullet {this.gameObject.name}. {tracked_bullets.Count} bullets still tracked!");  // TODO --- remove debug
    }

    public void TrackHit(IBullet bullet, IAttackTarget hit_target, Vector3 hit_location) {
        Debug.LogWarning($"track {HitOrMissString(hit_target)} for {bullet}. Hits tracked: {tracked_hits.Count}");  // TODO --- remove debug
        tracked_hits.AddLast(new TrackedHit(bullet.attacker, Time.time, hit_target, hit_location));
    }

    private static string HitOrMissString(IAttackTarget hit_target) {
        return hit_target == null ? "miss" : "hit";
    }

    public IEnumerable<IBullet> PlayerBullets() {
        foreach (IBullet b in tracked_bullets) {
            if (b.attacker.is_player) {
                yield return b;
            }
        }
    }
    
    public IEnumerable<IBullet> AllBullets() {
        foreach (IBullet b in tracked_bullets) {
            yield return b;
        }
    }

    public IEnumerable<Vector3> AllHitLocations() {
        foreach (TrackedHit hit in tracked_hits) {
            yield return hit.hit_location;
        }
    }

}

public struct TrackedHit {
    public IAttackTarget attacker;
    public float time;
    public IAttackTarget hit;
    public Vector3 hit_location;

    public TrackedHit(IAttackTarget attacker, float time, IAttackTarget hit, Vector3 hit_location) {
        this.attacker = attacker;
        this.time = time;
        this.hit = hit;
        this.hit_location = hit_location;
    }
}


// public struct TrackedBullet {
//     public IBullet bullet;
//     public IAttackTarget shooter
// }