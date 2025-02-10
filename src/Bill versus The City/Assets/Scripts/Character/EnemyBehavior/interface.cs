

public interface ISubBehavior {

    public float shooting_rate { get; }
    public bool cancel_reload_with_los { get { return false; } }
    public void SetControllerFlags(EnemyBehavior parent, PlayerMovement player);

    public void AssumeBehavior(EnemyBehavior parent, PlayerMovement player) { /* do nothing by default */ }
    public void EndBehavior(EnemyBehavior parent, PlayerMovement player) { /* do nothing by default */ }

}