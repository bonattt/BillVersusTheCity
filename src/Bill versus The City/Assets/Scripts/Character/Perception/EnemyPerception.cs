using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyPerception : MonoBehaviour, ICharStatusSubscriber
{
    /** 
      * Script controlling if an Enemy can see the player
      */

    public LayerMask vision_mask;
    public Transform raycast_start;

    public int max_vision_nodes = 1; // max number of visible nodes that will add to how quickly the player is noticed
    public float notice_player_rate = 2f;
    public float forget_player_rate = 0.25f;

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
    public int visible_nodes_this_frame { get; protected set; }

    [SerializeField]
    public float _percent_noticed; // ticks up as the enemy sees the player. at 1f, player is spotted
    public float percent_noticed { 
        get {
            return _percent_noticed;
        }
        protected set {
            if (value >= 1f) {
                if (_percent_noticed < 1f) {
                    Alert();
                }
                _percent_noticed = 1f;
            } else if (value <= 0) {
                LosePlayer();
                _percent_noticed = 0f;
            } else {
                _percent_noticed = value;
            }

            _percent_noticed = value;
            if(_percent_noticed <= 0) {
                _percent_noticed = 0f;
            } else if (_percent_noticed >= 1) {
                _percent_noticed = 1f;
            }
        }
    }

    public bool player_noticed { get; protected set; }

    public bool knows_player_location { get; protected set; }

    public Vector3 last_seen_at { get; protected set; }
    private bool updated_this_frame = false;

    void Start() {
        knows_player_location = false;
        last_seen_at = new Vector3(float.NaN, float.NaN, float.NaN);
        visible_nodes_this_frame = 0;
        GetComponent<ICharacterStatus>().Subscribe(this);
    }

    public void Alert() {
        // alerts the enemy
        _percent_noticed = 1f;
        knows_player_location = true;
        player_noticed = true;
    }

    public void LosePlayer() {
        // the enemy no longer knows where the player is, if it ever did.
        if (knows_player_location) {
            last_seen_at = target.position;
        }
        knows_player_location = false;
    }

    public void StatusUpdated(ICharacterStatus status) {
        // instantly notice the player if you take damage
        if (status.health < status.max_health) {
            Alert();
            return;
        }
        if (status.armor != null) {
            if (status.armor.armor_durability < status.armor.armor_max_durability) {
                Alert();
            }
        }
    }

    void Update() {
        UpdateLineOfSight();
        UpdateNoticePlayer();
        SetDebugData();
    }

    void UpdateNoticePlayer() {
        if (visible_nodes_this_frame == 0) { // && percent_noticed < 1f) {
            percent_noticed -= forget_player_rate * Time.deltaTime;
        } else {
            percent_noticed += notice_player_rate * Time.deltaTime * visible_nodes_this_frame;
        }
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
            visible_nodes_this_frame = VisibleNodes().Count;
            if (visible_nodes_this_frame > max_vision_nodes) {
                visible_nodes_this_frame = max_vision_nodes;
            } 
            _seeing_target = visible_nodes_this_frame > 0;
            updated_this_frame = true;
        }
    }

    public Transform target {
        get {
            // if (?PlayerCharacter.inst == null) {
            //     return null;
            // }
            return PlayerCharacter.inst.player_transform;
        }
    }

    private List<Transform> VisibleNodes() {
        List<Transform> visible_nodes = new List<Transform>();
        if (target == null) {
            Debug.LogWarning("no target!");
            return visible_nodes;
        }

        foreach (Transform node in PlayerCharacter.inst.vision_nodes) {
            if (CanSeeNode(node)) {
                visible_nodes.Add(node);
            }
        }
        return visible_nodes;
    }

    // private bool LineOfSightToPlayer() {
    //     if (PlayerCharacter.inst.player_transform == null) {
    //         Debug.LogWarning("no target!");
    //         return false;
    //     }
    //     // RaycastHit hit;
    //     // Vector3 offset = new Vector3(0f, 0.5f, 0f);
    //     // Vector3 start = transform.position + offset;
    //     // Vector3 end = target.position + offset;
    //     // Vector3 direction = end - start;
    //     // Debug.DrawRay(start, direction, Color.red);
    //     // if (Physics.Raycast(start, direction, out hit, direction.magnitude, vision_mask)) {
    //     //     bool los_to_target = hit.transform == target;
    //     //     // Debug.Log($"seeing {hit}");
    //     //     return los_to_target;
    //     // }
    //     // return false;
    //     return VisibleNodes().Count > 0;
    // }

    private bool CanSeeNode(Transform target_node) {
        RaycastHit hit;
        Vector3 start = raycast_start.position;
        Vector3 end = target_node.position;
        Vector3 direction = end - start;
        // Debug.DrawRay(start, direction, Color.red);
        if (Physics.Raycast(start, direction, out hit, direction.magnitude, vision_mask)) {
            bool los_to_target = hit.transform == target_node || hit.transform == target;
            return los_to_target;
        }
        return false;

    }
    
    public bool debug_knows_player_location, debug_player_noticed;
    public void SetDebugData() {
        debug_knows_player_location = knows_player_location;
        debug_player_noticed = player_noticed;
    }
}