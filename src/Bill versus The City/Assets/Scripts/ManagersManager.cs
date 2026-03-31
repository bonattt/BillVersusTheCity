using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagersManager : MonoBehaviour
{
    // this class creates a singleton for the managers prefab, so it doesn't get reinstantiated on level load if it already exists.
    public static ManagersManager inst { get; private set; }

    
    private List<Action> post_load_setup = new List<Action>();
    public void AddPostLoadAction(Action a) => post_load_setup.Add(a);

    public bool post_load_setup_already_run { get; private set; }
    public bool statics_loaded { get; private set; }

    void Awake() {
        statics_loaded = false;
        post_load_setup_already_run = false;
        inst = this;
    }

    void Start() {
        statics_loaded = true;
        PostLoadSetup();
        post_load_setup_already_run = true;
    }

    public void PostLoadSetup() {
        foreach(Action PostSetupAction in post_load_setup) {
            PostSetupAction();
        }
    }
}
