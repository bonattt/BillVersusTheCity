

public interface ISubBehavior {

    public float shooting_rate { get; }
    public void SetControllerFlags(EnemyBehavior parent, PlayerMovement player);

}