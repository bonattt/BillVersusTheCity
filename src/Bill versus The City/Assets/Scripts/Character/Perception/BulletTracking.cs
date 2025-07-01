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
    private HashSet<ITrackedProjectile> tracked_bullets = new HashSet<ITrackedProjectile>();
    // stored hits, implicitly (not explicitly) sorted by time of hit. Most recent hits are stored at the end, oldest hits are stored at the beginning
    private LinkedList<TrackedHit> tracked_hits = new LinkedList<TrackedHit>();

    void Awake() {
        _inst = this;
    }

    void Update() {
        RemoveOldHits();
    }

    private void RemoveOldHits() {
        while (tracked_hits.First != null && tracked_hits.First.Value.time < (Time.time - track_hits_for_seconds)) {
            tracked_hits.RemoveFirst();
        }
    }

    public void TrackNewBullet(ITrackedProjectile bullet) {
        if (bullet == null) { Debug.LogError("added null bullet"); } // TODO --- remove debug
        tracked_bullets.Add(bullet);
    }
    public void UnTrackBullet(ITrackedProjectile bullet) {
        tracked_bullets.Remove(bullet);
    }

    public void TrackHit(ITrackedProjectile bullet, IAttackTarget hit_target, Vector3 hit_location) {
        tracked_hits.AddLast(new TrackedHit(bullet.is_threat, Time.time, hit_target, hit_location));
    }

    private static string HitOrMissString(IAttackTarget hit_target) {
        return hit_target == null ? "miss" : "hit";
    }

    public IEnumerable<ITrackedProjectile> PlayerBullets() {
        // TODO --- refactor: rename this
        foreach (ITrackedProjectile b in tracked_bullets) {
            if (b.is_threat) {
                yield return b;
            }
        }
    }

    public IEnumerable<ITrackedProjectile> AllBullets() {
        foreach (ITrackedProjectile b in tracked_bullets) {
            yield return b;
        }
    }

    public IEnumerable<Vector3> AllHitLocations() {
        foreach (TrackedHit hit in tracked_hits) {
            yield return hit.hit_location;
        }
    }



    public IEnumerable<ITrackedProjectile> NearbyBullets(Vector3 position, float distance_threshold = 1f) {
        foreach (ITrackedProjectile bullet in AllBullets()) {
            float distance = FlatDistance(position, bullet.location.position);
            if (distance <= distance_threshold) {
                yield return bullet;
            }
        }
    }


    public IEnumerable<ITrackedProjectile> NearbyPlayerBullets(Vector3 position, float distance_threshold = 1f) {
        foreach (ITrackedProjectile bullet in PlayerBullets()) {
            float distance = FlatDistance(position, bullet.location.position);
            if (distance <= distance_threshold) {
                yield return bullet;
            }
        }
    }

    private static float FlatDistance(Vector3 a, Vector3 b) {
        a = new Vector3(a.x, 0, a.z);
        b = new Vector3(b.x, 0, b.z);
        return Vector3.Distance(a, b);
    }

    public static Vector3 GetClosestProjectile(Vector3 position, IEnumerable<ITrackedProjectile> threats) {
        // takes a list of tracked bullets, and returns the closest one to the given position.
        // if the given list is empty, returns a point at positive infinity
        Vector3 closest = new Vector3(float.PositiveInfinity, 0, float.PositiveInfinity);
        float closest_distance = float.PositiveInfinity;
        foreach (ITrackedProjectile p in threats) {
            float dist = Vector3.Distance(position, p.location.position);
            if (dist <= closest_distance) {
                closest = p.location.position;
            }
        }
        return closest;
    }
    
    public static Vector3 GetThreatCentroid(Vector3 position, IEnumerable<ITrackedProjectile> threats) {
        float total_weight = 0f;
        float sum_x = 0f;
        float sum_y = 0f;
        float sum_z = 0f;
        foreach (ITrackedProjectile p in threats) {
            float weight = Vector3.Distance(position, p.location.position) * p.threat_level;
            total_weight += weight;
            sum_x += p.location.position.x * weight;
            sum_y += p.location.position.y * weight;
            sum_z += p.location.position.z * weight;
        }
        Vector3 final_position = new Vector3(sum_x, 0f, sum_z) / total_weight;
        return final_position;
    }

}

public struct TrackedHit {
    public bool is_threat;
    public float time;
    public IAttackTarget hit;
    public Vector3 hit_location;

    public TrackedHit(bool is_threat, float time, IAttackTarget hit, Vector3 hit_location) {
        this.is_threat = is_threat;
        this.time = time;
        this.hit = hit;
        this.hit_location = hit_location;
    }
}


// public struct TrackedBullet {
//     public ITrackedProjectile bullet;
//     public IAttackTarget shooter
// }