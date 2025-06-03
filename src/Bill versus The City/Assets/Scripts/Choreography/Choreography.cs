

using System.Collections.Generic;
using UnityEngine;

public class Choreography : MonoBehaviour, IChoreography {
    public List<AbstractChoreographyStep> choreography_steps;

    [Tooltip("the index in `choreography_steps` the choreography is currently handling. (published in inspector for debuging purposes)")]
    [SerializeField]
    private int choreography_index = 0;

    public PlayerControls player_controls;

    public bool play_on_start = false;

    [SerializeField]
    private bool _active = false;
    public bool active {
        get => _active;
        private set { _active = value; }
    }
    
    [SerializeField]
    private bool _complete = false;
    public bool complete {
        get => _complete;
        private set { _complete = value; }
    }
    public void Activate() {
        active = true;
        player_controls.controls_locked = true;
        sequential_choreography.Activate();
    }
    public void Complete() {
        complete = true;
        player_controls.controls_locked = false;
    }

    private SequentialChoreographyStep sequential_choreography;

    void Start() {
        active = false;
        complete = false;
        sequential_choreography = gameObject.AddComponent<SequentialChoreographyStep>();
        sequential_choreography.choreography_steps = choreography_steps; 
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

        if (sequential_choreography.complete) {
            Complete();
        }
    }

    // public bool HasNextStep() {
    //     return choreography_index < choreography_steps.Count - 1;
    // }

    // public bool OnFinalStep() {
    //     return choreography_index == choreography_steps.Count - 1;
    // }
}