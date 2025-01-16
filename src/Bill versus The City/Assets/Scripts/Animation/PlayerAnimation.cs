
using UnityEngine;

public class PlayerAnimation : MonoBehaviour, IAnimationFacade {

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
    public float hurt_at { get; set; } // time last time the character took damage
    public bool is_killed { get; set; } // time last time the character took damage

    public Animator animator;

    void Start() {
        move_velocity = new Vector3(0f, 0f, 0f);
        forward_direction = new Vector3(0f, 0f, 0f);
        weapon_class = WeaponClass.handgun;
        aim_percent = 0f;
        shot_at = -1f;
        hurt_at = -1f;
        is_killed = false;
    }

    void Update() {
        UpdateAnimationController();
        UpdateDebugFields();
    }


    private void UpdateAnimationController() {
        UpdateMovementFields();
        UpdateCombatFields();
    }

    private void UpdateCombatFields() {
        animator.SetFloat("aim_percent", aim_percent);
        animator.SetBool("is_killed", is_killed);
        animator.SetBool("is_hurt", Time.time + hurt_duration < hurt_at);
        animator.SetBool("is_shooting", Time.time + shot_duration < shot_at);
    }

    private void UpdateMovementFields() {
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
    public bool debug_is_killed;

    public void UpdateDebugFields() {
        debug_velocity = move_velocity;
        debug_forward = forward_direction;
        debug_left = right_direction;
        debug_is_killed = is_killed;
    }
}