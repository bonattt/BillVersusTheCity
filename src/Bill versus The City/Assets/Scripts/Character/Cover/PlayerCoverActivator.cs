

using System;
using UnityEditor.EditorTools;
using UnityEngine;

public class PlayerCoverActivator : MonoBehaviour, ICoverActivator
{
    public CharCtrl character;
    [Tooltip("if the target character's `crouch_percent` equals or exceeds this value, the character will be considered as \"taking cover\" and will activate nearby cover.")]
    public float crouch_threshold = 0.1f;
    public bool is_taking_cover { get => character.crouch_percent >= crouch_threshold; }

    [Tooltip("while \"taking cover\", this character activates cover within this range.")]
    [SerializeField]
    private float _activate_cover_range = 5f;
    public float activate_cover_range { get => _activate_cover_range; }

    
    public Vector3 activate_cover_position => transform.position;

    void Start() {
        CoverActivationRegistry.inst.AddActivator(this);
    }

    void OnDestroy() {
        CoverActivationRegistry.inst.RemoveActivator(this);
    }

# if UNITY_EDITOR
    void Update() {
        UpdateDebug();
    }

    public PlayerCoverActivatorDebugger debug = new PlayerCoverActivatorDebugger();
    private void UpdateDebug() {
        debug.is_taking_cover = is_taking_cover;
    }
}

[Serializable]
public class PlayerCoverActivatorDebugger {
    public bool is_taking_cover = false;
# endif
}