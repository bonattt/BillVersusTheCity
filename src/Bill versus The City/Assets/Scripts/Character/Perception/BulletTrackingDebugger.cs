
using System.Collections.Generic;

using UnityEngine;


public class BulletTrackingDebugger : MonoBehaviour {

    public List<string> player_bullets, bullets_tracked, bullet_hits;

    void Update() {
        player_bullets = new List<string>();
        bullets_tracked = new List<string>();
        bullet_hits = new List<string>();
        foreach (IBullet b in BulletTracking.inst.AllBullets()) {
            bullets_tracked.Add(DisplayBullet(b));
        }
        foreach (IBullet b in BulletTracking.inst.PlayerBullets()) {
            player_bullets.Add(DisplayBullet(b));
        }
        foreach (Vector3 hit in BulletTracking.inst.AllHitLocations()) {
            bullet_hits.Add($"hit at {hit}");
        }
    }


    public static string DisplayBullet(IBullet b) {
        return $"{b.attacker.GetHitTarget().name}'s bullet at {b.location.position}";
    }

}