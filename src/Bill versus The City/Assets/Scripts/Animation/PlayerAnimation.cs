using System.Collections.Generic;

using UnityEngine;

public class PlayerAnimation : MonoBehaviour, IAnimationFacade, ICharStatusSubscriber {

    public readonly Dictionary<WeaponClass, int> weapon_enum_to_int = new Dictionary<WeaponClass, int>{
        {WeaponClass.handgun, 0},
        {WeaponClass.empty, 1},
        {WeaponClass.rifle, 2},
        {WeaponClass.shotgun, 3}
    };
    public Vector3 move_velocity { get; set; }
    public Vector3 forward_direction { get; set; }
    public Vector3 right_direction { get; set; }
    public AnimationActionType action { get; set; }
    public WeaponClass weapon_class { get; set; }

    public float _velocity_forward;
    public float velocity_forward { 
        get {
            return _velocity_forward;
        }
        set {
            _velocity_forward = value;
            animator.SetFloat("velocity_forward", value);
        }
    }
    public float _velocity_right;
    public float velocity_right { 
        get {
            return _velocity_right;
        }
        set {
            _velocity_right = value;
            animator.SetFloat("velocity_right", value);
        }
    }

    private bool _move_left = false;
    public bool move_left { 
        get {
            return _move_left;
        }
        protected set {
            _move_left = value;
            // animator.SetBool("move_left", value);
        }
    }
    private bool _move_right = false;
    public bool move_right { 
        get {
            return _move_right;
        }
        protected set {
            _move_right = value;
            // animator.SetBool("move_right", value);
        }
    }
    private bool _move_forward = false;
    public bool move_forward { 
        get {
            return _move_forward;
        }
        protected set {
            _move_forward = value;
            // animator.SetBool("move_forward", value);
        }
    }
    private bool _move_backward = false;
    public bool move_backward { 
        get {
            return _move_backward;
        }
        protected set {
            _move_backward = value;
            // animator.SetBool("move_backward", value);
        }
    }

    private bool _is_moving = false;
    public bool is_moving {
        get { return _is_moving; }
        set {
            _is_moving = value;
            animator.SetBool("is_moving", value);
        }
    }

    public bool is_sprinting { get; set; }

    private float _speed = 0f;
    public float speed {
        get {
            return _speed;
        }
        set {
            _speed = value;
            // animator.SetFloat("speed", move_velocity.magnitude);
        }
    }

    // set to true while the character is aiming
    public float aim_percent { get; set; } 

    // how long should the shooting animation play for
    public float _shot_duration = 0.1f; // start value settable in inspector
    public float shot_duration { 
        get {
            return _shot_duration;
        }
        set {
            _shot_duration = value;
        }
    }
    
    public float shot_at { get; set; } // time of the last gunshot
    
    // how long should the hurt animation play for
    public float _hurt_duration = 0.1f; // start value settable in inspector
    public float hurt_duration { 
        get {
            return _hurt_duration;
        }
        set {
            _hurt_duration = value;
        }
    }
    private float _hurt_at = float.NegativeInfinity;
    private int n_hurt_animations = 3;
    public float hurt_at { // time last time the character took damage
        get {
            return _hurt_at;
        }
        set {
            if (value != _hurt_at) {
                // pick a random hurt animation
                animator.SetInteger("hurt_animation", Random.Range(0, n_hurt_animations));
            }
            _hurt_at = value;
        }
    } 
    public bool is_killed { get; set; } // time last time the character took damage

    public float crouch_percent { get; set; }
    public bool crouch_dive { get; set; } // character is performing a crouch dive
    public bool is_reloading { get; set; } // character is reloading

    public Animator animator;
    
    private float previous_health = -1f;
    private ICharacterStatus status;
    public void StatusUpdated(ICharacterStatus status) {
        if(previous_health > status.health) {
            animator.SetTrigger("trigger_hurt");
        }
        previous_health = status.health;
    }
    public void OnDeath(ICharacterStatus status) {
        // triggers immediately on death
        animator.SetTrigger("trigger_killed");
    } 

    void Start() {
        move_velocity = new Vector3(0f, 0f, 0f);
        forward_direction = new Vector3(0f, 0f, 0f);
        weapon_class = WeaponClass.empty;
        aim_percent = 0f;
        shot_at = -1f;
        hurt_at = -1f;
        is_killed = false;
        crouch_percent = 0f;
        crouch_dive = false;

        // TODO --- refactor this
        // status = GetComponent<ICharacterStatus>();
        // status.Subscribe(this);
    }

    void Update() {
        UpdateAnimationController();
        UpdateDebugFields();
    }


    private void UpdateAnimationController() {
        UpdateMovementFields();
        UpdateCombatFields();
    }

    public bool is_hurt {
        get { return Time.time - hurt_duration < hurt_at; }
    }

    public bool is_shooting {
        get { return Time.time - shot_duration < shot_at; }
    }

    private void UpdateCombatFields() {
        animator.SetFloat("aim_percent", aim_percent);
        animator.SetBool("is_killed", is_killed);
        animator.SetBool("is_hurt", is_hurt);
        animator.SetBool("is_shooting", is_shooting);
        animator.SetBool("crouch_dive", crouch_dive);
        animator.SetBool("is_reloading", is_reloading);
        if (crouch_dive || is_sprinting) {
            animator.SetBool("is_sprinting", true);
        } else {
            animator.SetBool("is_sprinting", false);
        }
        animator.SetInteger("weapon_class", weapon_enum_to_int[weapon_class]);
    }

    private void UpdateMovementFields() {
        animator.SetFloat("crouch_percent", crouch_percent);
        velocity_forward = Vector3.Dot(move_velocity, forward_direction);
        velocity_right = Vector3.Dot(move_velocity, right_direction);
        speed = move_velocity.magnitude;
        if (move_velocity == new Vector3(0f, 0f, 0)) {
            move_left = false;
            move_right = false;
            move_forward = false;
            move_backward = false;
            is_moving = false;
            return;
        }
        is_moving = true;
        // if moving, determine which directions
        float dot_product = Vector3.Dot(move_velocity, forward_direction);
        if (dot_product > 0) {
            move_forward = true;
            move_backward = false;
        } else {
            move_forward = false;
            move_backward = true;
        }
    }

    //////////////////////////////
    //////// DEBUG FIELDS ////////
    //////////////////////////////
    
    public Vector3 debug_velocity, debug_forward, debug_left;
    public bool debug_is_killed, debug_is_hurt, debug_is_shooting, debug_crouch_dive;
    public float debug_crouch_percent, debug_aim_percent;

    public void UpdateDebugFields() {
        debug_velocity = move_velocity;
        debug_forward = forward_direction;
        debug_left = right_direction;
        debug_is_killed = is_killed;
        debug_is_hurt = is_hurt;
        debug_is_shooting = is_shooting;
        debug_crouch_percent = crouch_percent;
        debug_aim_percent = aim_percent;
        debug_crouch_dive = crouch_dive;
    }
}