using System;

using UnityEngine;

public abstract class AbstractCover : MonoBehaviour, ICover
{
    [SerializeField] // for debugging
    private bool _is_cover_active = false;
    public bool is_cover_active { 
        get => _is_cover_active;
        protected set { _is_cover_active = value; }
    }

    void Start()
    {
        Deactivate();
    }

    void Update() {
        is_cover_active = ActivateIfNearby();
    }

    private bool ActivateIfNearby() {
        // iterates over nearby cover activators, and activates the 
        foreach (ICoverActivator ca in CoverActivationRegistry.inst.IterateNearby(transform.position)) {
            if (! ca.is_taking_cover) { continue; } // activator is in range, but not taking cover
            Activate();
            return true; // short circuit: cover should activate with only one activator.
        }
        Deactivate();
        return false;
    }

    protected abstract void Activate();
    protected abstract void Deactivate();
}