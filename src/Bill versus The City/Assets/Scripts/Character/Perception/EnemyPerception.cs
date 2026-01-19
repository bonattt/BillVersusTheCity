using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Serialization;

public class EnemyPerception : MonoBehaviour, ICharStatusSubscriber, ISuppressionObserver
{
    /** 
      * Script controlling if an Enemy can see the player
      */

    public LayerMask vision_mask;
    public Transform raycast_start;

    public int max_vision_nodes = 1; // max number of visible nodes that will add to how quickly the player is noticed
    [Tooltip("multiplier that effects how quickly an enemy will detect the player.")]
    [FormerlySerializedAs("notice_player_rate")]
    [SerializeField]
    private float _notice_player_rate = 2f;
    public float notice_player_rate {
        get {
            if (is_flashbang_dazed) {
                return _notice_player_rate / 2;
            }
            return _notice_player_rate;
        }
    }
    public float forget_player_rate = 0.25f;
    
    [SerializeField]
    [Tooltip("Permanently blinds this enemy.")]
    private bool _disable_spot = false;
    public bool disable_spot { get => _disable_spot || alert_by_script || GameSettings.inst.debug_settings.GetBool("player_invisible"); }

    [SerializeField]
    [Tooltip("Permanently deafens this enemy.")]
    private bool _disable_hearing = false;
    public bool disable_hearing { get => _disable_hearing || alert_by_script || GameSettings.inst.debug_settings.GetBool("player_invisible"); }

    [Tooltip("Set this flag to manage the enemy's alertness by triggers, instead of allowing them to notice the player on their own. Will be automatically disabled, once the enemy is alerted by script.")]
    public bool alert_by_script = false;
    
    [Tooltip("offsets the test point to check if the enemy is on screen toward the player to adjust how much of the enemy must be on screen")]
    public float on_screen_test_offset = -0.5f; 

    [Tooltip("if the enemy is offscreen, notice rate is miltiplied by this value")]
    public float offscreen_notice_multiplier = 0.05f;

    [Tooltip("multiplies the duration of flashbang effects")]
    [SerializeField]
    private float _flashbang_effectiveness = 2f;
    public float flashbang_effectiveness {
        get {
            float difficulty_mod = GameSettings.inst.difficulty_settings.GetFloat("enemy_flashbang_effectiveness");
            return _flashbang_effectiveness * difficulty_mod;
        }
    }

    private Camera visiblity_camera;

    public float _reaction_time = 0.1f;
    public float reaction_time {
        get {
            float total_reaction_time = _reaction_time * GameSettings.inst.difficulty_settings.GetMultiplier(DifficultySettings.ENEMY_REACTION_TIME);
            if (is_flashbang_dazed) {
                total_reaction_time = total_reaction_time * 2;
            }
            return total_reaction_time;
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

    public bool is_alert { get => state == PerceptionState.alert | state == PerceptionState.seeing; }

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

            if (is_alert) { // state == PerceptionState.alert | state == PerceptionState.seeing;
                if (_percent_noticed <= 0) {
                    LosePlayer();
                }
                else if (visible_nodes_this_frame >= 1) {
                    state = PerceptionState.seeing;
                }
            }
            else if(state == PerceptionState.searching | state == PerceptionState.unaware) {
                if (_percent_noticed >= 1) {
                    Alert();
                }
            }
            else if(state == PerceptionState.dead) {
                _percent_noticed = 0f;
            }
            else {
                Debug.LogError($"unhandled perception state {state}");
            }
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

    public bool is_flashbang_blinded => Time.time < _flashbang_blinded_until; 
    public bool is_flashbang_dazed => Time.time < _flashbang_dazed_until; 
    private float _flashbang_blinded_until = -1f;
    private float _flashbang_dazed_until = -1f;
    [SerializeField] private Transform daze_particles_follow;

    void Start() {
        last_seen_at = new Vector3(float.NaN, float.NaN, float.NaN);
        visible_nodes_this_frame = 0;
        GetComponent<ICharacterStatus>().Subscribe(this);
        GetComponent<ISuppressionManager>().Subscribe(this);
        visiblity_camera = Camera.main; // cache Camera.main to avoid repeated slow lookups by tag
    }

    public void Alert() {
        // alerts the enemy
        alert_by_script = false;
        _percent_noticed += 0.5f;
        last_seen_at = target.position;
        if (visible_nodes_this_frame >= 1) {
            state = PerceptionState.seeing;
            SwarmIntelligence.inst.SeePlayer(last_seen_at, Time.time);
        } else {
            state = PerceptionState.alert;
        }
    }

    // public void AlertFrom(Vector3 alert_from) {
    //     // alert the enemy, but give a waypoint other than the player's current position
    //     Alert();
    //     last_seen_at = alert_from;
    // }

    public void LosePlayer() {
        // the enemy no longer knows where the player is, if it ever did.
        if (state == PerceptionState.unaware) { return; } // do nothing if the enemy didn't know about the player
        state = PerceptionState.searching;
        SwarmIntelligence.inst.SeePlayer(last_seen_at, Time.time);
    }


    public void UpdateRecoveredFromSuppression() { /* do nothing */ }
    public void UpdateBecomeSuppressed() => Alert();

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
        if (status.adjusting_difficulty) { return; } // don't alert enemies because you lowered the difficulty, which reduces their health
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
        if (!LevelConfig.inst.combat_enabled) { return; } // do nothing when combat is not enabled
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
        if (!_seeing_target) {
            percent_noticed -= forget_player_rate * Time.deltaTime;
        } else {
            int effective_nodes = Math.Min(max_vision_nodes, visible_nodes_this_frame);
            float notice_rate = this.notice_player_rate * effective_nodes;
            if (!IsOnScreen()) {
                notice_rate *= offscreen_notice_multiplier;
            }
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
        if (!LevelConfig.inst.combat_enabled || disable_spot) {
            // if spot is disabled, do not visually notice the player
            _seeing_target = false;
            _saw_target_last_frame = false;
            return;
        }
        if (! updated_this_frame) {
            _saw_target_last_frame = _seeing_target;
            if (is_flashbang_blinded) {
                _seeing_target = false;
                updated_this_frame = true;
                return; // blinded by flashbang, don't check for visibility
            }
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

    private float DistanceToPlayer(Transform target_node) {
        Vector3 start = raycast_start.position;
        Vector3 end = target_node.position;
        Vector3 direction = end - start;
        return direction.magnitude;
    }

    private bool CanSeeNode(Transform target_node) {
        RaycastHit hit;
        Vector3 start = raycast_start.position;
        Vector3 end = target_node.position;
        Vector3 direction = end - start;

        float vision_range;
        if (state == PerceptionState.alert || state == PerceptionState.seeing) {
            vision_range = Mathf.Min(DistanceToPlayer(target_node), max_alert_vision_range);
        } else {
            vision_range = Mathf.Min(DistanceToPlayer(target_node), max_notice_range);
        }
        // Debug.DrawRay(start, direction, Color.red);
        if (Physics.Raycast(start, direction, out hit, vision_range, vision_mask)) {
            bool los_to_target = hit.transform == target_node || hit.transform == target;
            return los_to_target;
        }
        return false;

    }

    private List<IPerceptionSubscriber> subscribers = new List<IPerceptionSubscriber>();
    public void Subscribe(IPerceptionSubscriber new_sub) => subscribers.Add(new_sub);
    public void Unsubscribe(IPerceptionSubscriber old_sub) => subscribers.Remove(old_sub);
    public void UpdateSubscribers(PerceptionState old_state, PerceptionState new_state) {
        foreach(IPerceptionSubscriber sub in subscribers) {
            sub.UpdatePerceptionState(old_state, new_state);
        }
    }

    public bool IsOnScreen() {
        // returns true if this enemy is visible on-screen
        Vector3 test_point = GetOnScreenTestPoint();
        Vector3 viewport_position = visiblity_camera.WorldToViewportPoint(test_point);
        // NOTE: do not check `viewport_position.z < 1`, z < 0 is behind camera, > 0 is in front of the camera. other dimensions check if point is 
        //   off either side/top/bottom of the screen
        return viewport_position.z > 0 && viewport_position.x > 0 && viewport_position.x < 1 && viewport_position.y > 0 && viewport_position.y < 1;
    }

    private Vector3 GetOnScreenTestPoint() {
        // shifts the test point slightly off the enemy, to make sure the WHOLE enemy is on screen
        Vector3 towards_player = target.position - transform.position;
        towards_player = new Vector3(towards_player.x, 0f, towards_player.z).normalized;
        return  transform.position + (towards_player * on_screen_test_offset);
    }

    public void HearSound(HearingHit hit)
    {
        if (is_flashbang_blinded) { 
            Debug.Log($"{gameObject.name} ignored sound while deafened by flashbang.");
            return; 
        } // cannot hear while blind and deff from a flashbang
        if (hit.enemy != this) { Debug.LogWarning($"Enemy {gameObject.name} heard HearingHit not for itself, intended for {hit.enemy.gameObject.name}!"); }
        if (!LevelConfig.inst.combat_enabled || disable_hearing) { return; } 
        float alert_level = hit.sound.alarm_level * notice_player_rate;
        if (hit.sound.adjust_alarm_based_on_distance)
        {
            alert_level = alert_level / Mathf.Sqrt(hit.effective_range);
        }
        bool already_alert = is_alert;
        percent_noticed += alert_level;
        if (!seeing_target)
        {
            // if the enemy is alerted by this sound, they shouldn't know where the player is
            _last_seen_at = hit.sound.origin;
            // LosePlayer(); 
        }
    }
    public void FlashBangHit(Vector3 flashbang_position, float intensity) {
        if(state == PerceptionState.dead) { return; } // cannot be flashbanged if dead
        intensity *= flashbang_effectiveness;
        if (!is_alert) {
            Alert();
        }
        if (!seeing_target) {
            last_seen_at = flashbang_position;
        }
        LosePlayer();
        _flashbang_blinded_until = intensity + Time.time;
        _flashbang_dazed_until = (3 * intensity) + Time.time;
        DisplayFlashbangHit();
    }

    private void DisplayFlashbangHit() {
        GameObject particles_prefab = Resources.Load<GameObject>("FlashbangDazeParticles");
        GameObject daze_particles = Instantiate(particles_prefab);
        FlashbangDazeParticles particles_script = daze_particles.GetComponent<FlashbangDazeParticles>();
        particles_script.perception = this;
        particles_script.follow = daze_particles_follow;
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.gray;
    //     Gizmos.DrawSphere(GetOnScreenTestPoint(), 0.25f);
    // }

    /////////////////////////////// DEBUG FIELDS //////////////////////////////////
    
    public bool debug__flashbang_now = false;
    public EnemyPerceptionDebugger debug;
    public void SetDebugData() {
        if(debug__flashbang_now) {
            debug__flashbang_now = false;
            FlashBangHit(transform.position, 4f);
        }
        debug.knows_player_location = knows_player_location;
        debug.player_noticed = player_noticed;
        debug.on_screen = IsOnScreen();
        debug.distance_from_player = DistanceToPlayer(PlayerCharacter.inst.vision_nodes[0]);
        debug.is_flashbang_blinded = is_flashbang_blinded;
        debug.is_flashbang_dazed = is_flashbang_dazed;
        debug.flashbang_blinded_until = _flashbang_blinded_until;
        debug.flashbang_dazed_until = _flashbang_dazed_until;
    }
}

[Serializable]
public class EnemyPerceptionDebugger {
    public bool knows_player_location, player_noticed, on_screen, is_flashbang_dazed, is_flashbang_blinded;
    public float distance_from_player, flashbang_blinded_until, flashbang_dazed_until;
}

public enum PerceptionState {
    unaware,  // enemy does not know the Player exists
    searching,  // enemy knows the player exists, AND knows the player is not near the waypoint
    alert,  // enemy is aware the player exists, and has some waypoint they are investigating
    seeing,  // enemy is seeing the player this frame
    dead // enemy has been killed
    
}

public interface IPerceptionSubscriber {
    // this interface is for subscribing to updates from an enemy's perception state. (EG. to display that the enemy has seen the player)
    public void UpdatePerceptionState(PerceptionState previous_state, PerceptionState new_state);
}