using UnityEngine;

public interface ISubBehavior {

    public float shooting_rate { get; }
    public bool cancel_reload_with_los { get { return false; } }
    public void SetControllerFlags(EnemyBehavior parent, PlayerMovement player);

    public void AssumeBehavior(EnemyBehavior parent) { Debug.LogWarning($"start behavior {this}"); /* do nothing by default */ }
    public void EndBehavior(EnemyBehavior parent) { Debug.LogWarning($"end behavior {this}"); /* do nothing by default */ }
    public string GetDebugMessage(EnemyBehavior parent) { return ""; }
}