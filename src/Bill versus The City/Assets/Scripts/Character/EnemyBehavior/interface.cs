

public interface ISubBehavior {

    public float shooting_rate { get; }
    public bool cancel_reload_with_los { get { return false; } }
    public void SetControllerFlags(EnemyBehavior parent, PlayerMovement player);

}