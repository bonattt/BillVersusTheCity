using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    /** 
      * Script controlling if an Enemy can see the player
      */

    public LayerMask vision_mask;

    protected bool _saw_target_last_frame = false; // saw the target last frame
    public bool saw_target_last_frame { 
        get { 
            UpdateLineOfSight();
            return _saw_target_last_frame; 
        }
    } 
    protected bool _seeing_target = false; // seeing the target last frame
    public bool seeing_target { 
        get { 
            UpdateLineOfSight();
            return _seeing_target; 
        }
    }

    public Vector3 last_seen_at { get; protected set; }
    private bool updated_this_frame = false;

    void Start() {

    }

    void Update() {
        UpdateLineOfSight();
    }

    void LateUpdate() {
        updated_this_frame = false;
    }

    private void UpdateLineOfSight() {
        // lazy update line of sight once per frame
        // to guarantee consistency, this must be called when LoS is queries, because the order scripts call `Update` is not known. 
        // must also be called once per frame in `Update`, so ensure `saw_target_last_frame` is called
        if (! updated_this_frame) {
            _saw_target_last_frame = _seeing_target;
            _seeing_target = LineOfSightToTarget();
            updated_this_frame = true;
        }
    }

    public Transform target {
        get {
            if (PlayerMovement.inst == null) {
                return null;
            }
            return PlayerMovement.inst.transform;
        }
    }

    public bool LineOfSightToTarget() {
        if (PlayerMovement.inst == null) {
            Debug.LogWarning("no target!");
            return false;
        }
        RaycastHit hit;
        Vector3 offset = new Vector3(0f, 0.5f, 0f);
        Vector3 start = transform.position + offset;
        Vector3 end = target.position + offset;
        Vector3 direction = end - start;
        Debug.DrawRay(start, direction, Color.red);
        if (Physics.Raycast(start, direction, out hit, direction.magnitude, vision_mask)) {
            bool los_to_target = hit.transform == target;
            // Debug.Log($"seeing {hit}");
            return los_to_target;
        }
        Debug.Log("I SEE NOTHING!");
        return false;
    }
}