

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
    }
    public void Complete() {
        complete = true;
        player_controls.controls_locked = false;
    }

    void Start() {
        active = false;
        complete = false;
    }


    void Update() {
        if (play_on_start) {
            Activate();
            play_on_start = false;
            return;
        }
        if (!active || complete) { return; }

        AbstractChoreographyStep step = choreography_steps[choreography_index];
        if (!step.active) { step.Activate(); }

        if (step.complete) {
            if (OnFinalStep()) {
                Complete();
            } else {
                choreography_index += 1;
            }
        }
    }

    public bool HasNextStep() {
        return choreography_index < choreography_steps.Count - 1;
    }

    public bool OnFinalStep() {
        return choreography_index == choreography_steps.Count - 1;
    }
}