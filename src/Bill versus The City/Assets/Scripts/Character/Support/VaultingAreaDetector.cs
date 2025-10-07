using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VaultingAreaDetector : MonoBehaviour {

    private HashSet<VaultOverCoverZone> cached_areas = new HashSet<VaultOverCoverZone>();

    public Collider target_collider;

    public VaultOverCoverZone GetVaultOverCoverZone() {
        VaultOverCoverZone result = null;
        float least_distance = float.PositiveInfinity;
        foreach (VaultOverCoverZone zone in cached_areas) {
            float distance = Vector3.Distance(transform.position, zone.transform.position);
            if (distance <= least_distance) {
                result = zone;
                least_distance = distance;
            }
        }
        return result;
    }

    void OnCollisionEnter(Collision c) {
        Debug.LogWarning("VaultingAreaDetector.OnCollisionEnter.Called");
        HandleEnter(c.collider);
    }

    void OnCollisionExit(Collision c) {
        HandleEnter(c.collider);
    }

    void OnTriggerEnter(Collider other) {
        HandleEnter(other);
    }

    void OnTriggerExit(Collider other) {
        HandleExit(other);
    }

    void HandleEnter(Collider other) {
        VaultOverCoverZone zone = other.gameObject.GetComponent<VaultOverCoverZone>();
        if (zone != null) {
            cached_areas.Add(zone);
        } else { Debug.LogWarning("HandleEnter handled a collider without a VaultOverCoverZone!"); } // TODO --- remove debug
    }

    void HandleExit(Collider other) {
        VaultOverCoverZone zone = other.gameObject.GetComponent<VaultOverCoverZone>();
        if (zone != null && cached_areas.Contains(zone)) {
            cached_areas.Remove(zone);
        } else { Debug.LogError("HandleExit handled a collider that wasn't tracked!"); } // TODO --- remove debug
    }

    void Update() {
        UpdateDebug();
    }

    public DebugVaultingAreaDetector debug = new DebugVaultingAreaDetector();
    
    private void UpdateDebug() {
        debug.cached_areas = cached_areas.ToList();
    }
}


[Serializable]
public class DebugVaultingAreaDetector {
    public List<VaultOverCoverZone> cached_areas = new List<VaultOverCoverZone>();


}