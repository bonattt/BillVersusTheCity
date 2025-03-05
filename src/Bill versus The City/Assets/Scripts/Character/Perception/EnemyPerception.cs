using System;
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
    public bool disable_spot = false;

    public float _reaction_time = 0.1f;
    public float reaction_time {
        get {
            return _reaction_time * GameSettings.inst.difficulty_settings.GetMultiplier(DifficultySettings.ENEMY_REACTION_TIME);
        }
    }
    private float reaction_time_passed = 0f;

    public float max_alert_vision_range = 24f; // enemies will see the player a this longer range once alerted
    public float max_notice_range = 13f; // enemies will not notice players further than this

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
    private PerceptionState _state = PerceptionState.unaware;
    public PerceptionState state {
        get { return _state; }
        protected set { 
            PerceptionState old_state = _state;
            _state = value; 
            if (old_state != _state) {
                UpdateSubscribers(old_state, _state);
            }
        }
    }

    [SerializeField]
    public float _percent_noticed; // ticks up as the enemy sees the player. at 1f, player is spotted
    public float percent_noticed { 
        get {
            return _percent_noticed;
        }
        protected set {
            _percent_noticed = value;
            if (_percent_noticed <= 0) {
                _percent_noticed = 0;
            } else if (_percent_noticed >= 1.1f) {
                _percent_noticed = 1.1f;
            }

            if (state == PerceptionState.alert || state == PerceptionState.seeing) {
                if (_percent_noticed <= 0) {
                    LosePlayer();
                }
                else if (visible_nodes_this_frame >= 1) {
                    state = PerceptionState.seeing;
                }
            }

            else if (state == PerceptionState.searching || state == PerceptionState.unaware) {
                if (_percent_noticed >= 1) {
                    Alert();
                }
            }

            else {
                Debug.LogError($"unhandled perception state {state}");
            }

            // if (value >= 1f) {
            //     if (_percent_noticed < 1f) {
            //         Alert();
            //     }
            //     // _percent_noticed = 1f;
            // } else if (value <= 0) {
            //     LosePlayer();
            //     _percent_noticed = 0f;
            // } else {
            //     _percent_noticed = value;
            // }

            // _percent_noticed = value;
            // if(_percent_noticed <= 0) {
            //     _percent_noticed = 0f;
            // } 
            // // else if (_percent_noticed >= 1) {
            // //     _percent_noticed = 1f;
            // // }
        }
    }

    public bool player_noticed { get { return state != PerceptionState.unaware; } }
    public bool knows_player_location { get { return state == PerceptionState.seeing; } }
    public Vector3 _last_seen_at;
    public Vector3 last_seen_at { 
        get {
            return _last_seen_at;
        } 
        protected set {
            _last_seen_at = value;
            last_seen_at_investigated = false;
        }
    }
    public bool last_seen_at_investigated = false;
    private bool updated_this_frame = false; // used to avoid calculating visibility more than once per frame

    void Start() {
        last_seen_at = new Vector3(float.NaN, float.NaN, float.NaN);
        visible_nodes_this_frame = 0;
        GetComponent<ICharacterStatus>().Subscribe(this);
    }

    public void Alert() {
        // alerts the enemy
        disable_spot = false;
        _percent_noticed += 0.5f;
        last_seen_at = target.position;
        if (visible_nodes_this_frame >= 1) {
            state = PerceptionState.seeing;
        } else {
            state = PerceptionState.alert;
        }
    }

    public void AlertFrom(Vector3 alert_from) {
        // alert the enemy, but give a waypoint other than the player's current position
        Alert();
        last_seen_at = alert_from;
    }

    public void LosePlayer() {
        // the enemy no longer knows where the player is, if it ever did.
        if (state == PerceptionState.unaware) { return; } // do nothing if the enemy didn't know about the player
        state = PerceptionState.searching;
    }

    public void StatusUpdated(ICharacterStatus status) {
        // instantly notice the player if you take damage
        // if (status.health < status.max_health) {
        //     Alert();
        //     return;
        // }
        // if (status.armor != null) {
        //     if (status.armor.armor_durability < status.armor.armor_max_durability) {
        //         Alert(); 
        //     }
        // }
    }

    public void OnDamage(ICharacterStatus status) {
        Alert();        
    }
    public void OnHeal(ICharacterStatus status) {
        // do nothing
    }

    
    public void OnDeath(ICharacterStatus status) { 
        /* triggers immediately on death */ 
        state = PerceptionState.dead;
    }

    void Update() {
        UpdateLineOfSight(); // sets lazy values for line of sight
        UpdateNoticePlayer();
        UpdatePlayerPosition();
        SetDebugData();
    }

    void UpdatePlayerPosition() {
        if (knows_player_location) {
            last_seen_at = target.position;
        }
    }

    void UpdateNoticePlayer() {
        if (visible_nodes_this_frame == 0) { // && percent_noticed < 1f) {
            percent_noticed -= forget_player_rate * Time.deltaTime;
        } else {
            int effective_nodes = Math.Min(max_vision_nodes, visible_nodes_this_frame);
            float notice_rate = this.notice_player_rate * effective_nodes;
            if (player_noticed) { notice_rate *= 2; } // notice the player faster if the enemy is already alert;
            percent_noticed += notice_rate * Time.deltaTime;
            // updating state is handled by percent_noticed
        }
    }

    void LateUpdate() {
        updated_this_frame = false;
    }

    private void UpdateLineOfSight() {
        // lazy update line of sight once per frame
        // to guarantee consistency, this must be called when LoS is queries, because the order scripts call `Update` is not known. 
        // must also be called once per frame in `Update`, so ensure `saw_target_last_frame` is called
        if (disable_spot || GameSettings.inst.debug_settings.player_invisible) {
            // if spot is disabled, do not visually notice the player
            _seeing_target = false;
            _saw_target_last_frame = false;
            return;
        }
        if (! updated_this_frame) {
            _saw_target_last_frame = _seeing_target;
            visible_nodes_this_frame = VisibleNodes().Count;
            if (visible_nodes_this_frame > max_vision_nodes) {
                visible_nodes_this_frame = max_vision_nodes;
            } 

            if (visible_nodes_this_frame > 0) {
                if (reaction_time_passed >= reaction_time) {
                    _seeing_target = true;
                }
                else {
                    _seeing_target = false;
                    reaction_time_passed += Time.deltaTime;
                }
            }
            else {
                _seeing_target = false;
                reaction_time_passed = 0f;
            }
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

        float vision_range = direction.magnitude;
        if (state == PerceptionState.alert || state == PerceptionState.seeing) {
            vision_range = Mathf.Min(vision_range, max_alert_vision_range);
        } else {
            vision_range = Mathf.Min(vision_range, max_notice_range);
        }
        // Debug.DrawRay(start, direction, Color.red);
        if (Physics.Raycast(start, direction, out hit, vision_range, vision_mask)) {
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

    private List<IPerceptionSubscriber> subscribers = new List<IPerceptionSubscriber>();
    public void Subscribe(IPerceptionSubscriber new_sub) => subscribers.Add(new_sub);
    public void Unsubscribe(IPerceptionSubscriber old_sub) => subscribers.Remove(old_sub);
    public void UpdateSubscribers(PerceptionState old_state, PerceptionState new_state) {
        foreach(IPerceptionSubscriber sub in subscribers) {
            sub.UpdatePerceptionState(old_state, new_state);
        }
    }
}

public enum PerceptionState {
    unaware,  // enemy does not know the Player exists
    searching,  // enemy knows the player exists, AND knows the player is not near the waypoint
    alert,  // enemy is aware the player exists, and has some waypoint they are investigating
    seeing,  // enemy is seeing the player this frame
    dead // enemy has been killed
    
}

public interface IPerceptionSubscriber {
    public void UpdatePerceptionState(PerceptionState previous_state, PerceptionState new_state);
}