
using UnityEngine;

public class PlayerAnimation : MonoBehaviour, IAnimationFacade {

    public Vector3 move_velocity { get; set; }
    public Vector3 forward { get; set; }
    public AnimationActionType action { get; set; }


    private bool _move_left = false;
    public bool move_left { 
        get {
            return _move_left;
        }
        protected set {
            _move_left = value;
            animator.SetBool("move_left", value);
        }
    }
    private bool _move_right = false;
    public bool move_right { 
        get {
            return _move_right;
        }
        protected set {
            _move_right = value;
            animator.SetBool("move_right", value);
        }
    }
    private bool _move_forward = false;
    public bool move_forward { 
        get {
            return _move_forward;
        }
        protected set {
            _move_forward = value;
            animator.SetBool("move_forward", value);
        }
    }
    private bool _move_backward = false;
    public bool move_backward { 
        get {
            return _move_backward;
        }
        protected set {
            _move_backward = value;
            animator.SetBool("move_backward", value);
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
            animator.SetFloat("speed", move_velocity.magnitude);
        }
    }

    public Animator animator;

    void Start() {
        move_velocity = new Vector3(0f, 0f, 0f);
        forward = new Vector3(0f, 0f, 0f);
    }

    void Update() {
        UpdateAnimationController();
    }


    private void UpdateAnimationController() {
        
        speed = move_velocity.magnitude;
        Debug.Log($"{gameObject.name} animation; move speed: {speed}");
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
        float dot_product = Vector3.Dot(move_velocity, forward);
        if (dot_product > 0) {
            move_forward = true;
            move_backward = false;
        } else {
            move_forward = false;
            move_backward = true;
        }
    }
}