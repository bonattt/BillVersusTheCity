

using System.Collections.Generic;
using UnityEngine;

public class Choreography : MonoBehaviour, IChoreography {

    public static Choreography inst { get; private set; }

    [SerializeField]
    [Tooltip("set the camera mode for this choreography.")]
    private ChoreographyCameraMode _camera_mode = ChoreographyCameraMode.default_camera;
    public ChoreographyCameraMode camera_mode {
        get => _camera_mode;
        set {
            _camera_mode = value;
            SetCameraMode();
        }
    }
    [Tooltip("player camera follow script to manage during this choreography.")]
    public CameraFollowZoom camera_script;

    [Tooltip("the index in `choreography_steps` the choreography is currently handling. (published in inspector for debuging purposes)")]
    [SerializeField]
    private int choreography_index = 0;

    [Tooltip("Target to follow with the camera iff camera mode set to `follow_target`")]
    public Transform _camera_follow_target;
    public Transform camera_follow_target {
        get => _camera_follow_target;
        set {
            _camera_follow_target = value;
            SetCameraMode();
        }
    }

    private Vector2 _cached_camera_offset = new Vector2(0, 0); // cached value from camera before choreography starts, so it can be restored
    private Vector2 _camera_offset = new Vector2(0, 0);
    // current offset to use on the camera during choreography, IFF the camera is set to FOLLOW TARGET mode
    public Vector3 camera_offset {
        get => _camera_offset;
        set {
            _camera_offset = value;
            SetCameraMode();
        }   
    }

    private Transform cached_camera_follow_target = null; // used to store the original camera follow target so it can be restored after being overriden

    public PlayerControls player_controls;
    public LevelConfig level;

    public bool play_on_start = false;

    [SerializeField]
    private bool _active = false;
    public bool active {
        get => _active;
        private set { _active = value; }
    }

    [SerializeField]
    private bool _complete = false;
    public bool effect_completed { get => _complete; }
    
    public List<AbstractChoreographyStep> choreography_steps;

    public bool complete {
        get => _complete;
        private set { _complete = value; }
    }
    private bool cached_combat_enabled;
    public bool debug__cached_combat_enabled;
    public void Activate() {
        inst = this;
        active = true;
        player_controls.controls_locked = true;
        cached_combat_enabled = level.combat_enabled;
        level.combat_enabled = false;
        sequential_choreography.Activate(this);
        SetCameraMode();

        debug__cached_combat_enabled = cached_combat_enabled;
    }
    public void Complete() {
        inst = null;
        complete = true;
        player_controls.controls_locked = false;
        level.combat_enabled = cached_combat_enabled;
        UnsetCameraMode();
    }
    public void ActivateEffect() => Activate();
    public void Interact(GameObject actor) => Activate();

    private SequentialChoreographyStep sequential_choreography;

    void Start() {
        active = false;
        complete = false;
        sequential_choreography = gameObject.AddComponent<SequentialChoreographyStep>();
        sequential_choreography.choreography_steps = choreography_steps;
        _cached_camera_offset = camera_script.camera_offset;
        if (camera_script != null) {
            SetCameraMode();
        }
    }


    void Update() {
        if (play_on_start) {
            Activate();
            play_on_start = false;
            return;
        }
        if (!active || complete) { return; }
        choreography_index = sequential_choreography.choreography_index;
        // AbstractChoreographyStep step = choreography_steps[choreography_index];
        // if (!step.active) { step.Activate(); }

        if (sequential_choreography.choreography_complete) {
            Complete();
        }
    }

    protected void SetCameraMode() {
        if (cached_camera_follow_target != null) {
            camera_script.target = cached_camera_follow_target; // restore follow target
            cached_camera_follow_target = null;
        }
        switch (camera_mode) {
            case ChoreographyCameraMode.default_camera:
                UnsetCameraMode();
                break;

            case ChoreographyCameraMode.follow_player:
                camera_script.override_camera_follow = true;
                camera_script.camera_follow_override = CameraFollowMode.target;
                camera_script.camera_offset = camera_offset;
                break;

            case ChoreographyCameraMode.follow_target:
                camera_script.override_camera_follow = true;
                camera_script.camera_follow_override = CameraFollowMode.target;
                cached_camera_follow_target = camera_script.target;
                camera_script.target = camera_follow_target;
                camera_script.camera_offset = camera_offset;
                break;

            case ChoreographyCameraMode.manual:
                camera_script.override_camera_follow = true;
                camera_script.camera_follow_override = CameraFollowMode.choreography;
                camera_script.camera_offset = camera_offset;
                break;
        }
    }
    protected void UnsetCameraMode() {
        camera_script.camera_offset = _cached_camera_offset;
        if (camera_script != null && cached_camera_follow_target != null) {
            camera_script.target = cached_camera_follow_target;
            cached_camera_follow_target = null;
        }
        if (camera_script != null) {
            camera_script.override_camera_follow = false;
        }
    }

    void OnDestroy() {
        Complete(); // unlock player and camera 
    }

    // public bool HasNextStep() {
    //     return choreography_index < choreography_steps.Count - 1;
    // }

    // public bool OnFinalStep() {
    //     return choreography_index == choreography_steps.Count - 1;
    // }
}

public enum ChoreographyCameraMode {
    default_camera, // do nothing with the camera
    follow_player,
    follow_target,
    manual, // camera should be moved manually during choreography
}