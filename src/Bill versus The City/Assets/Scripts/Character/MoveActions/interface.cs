using UnityEngine;

public interface IMoveAction
{
    public float movement_speed { get; }
    public bool override_move_direction { get; }
    public bool crouch { get; }
    public Vector3 move_direction { get; }
    public MoveActionLookDirection look_direction { get; }
    public float duration { get; set; }
}
public enum MoveActionLookDirection {
    look_direction,
    move_direction,
}