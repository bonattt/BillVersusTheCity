using UnityEngine;


public class BasicMoveAction : IMoveAction {
    public string name = "new action";
    public float movement_speed { get; set; }
    public bool override_move_direction { get; set; }
    public bool crouch { get; set; }
    public Vector3 move_direction { get; set; }
    public MoveActionLookDirection look_direction { get; set; }
    public float duration { get; set; }

    public BasicMoveAction(float movement_speed, float duration) {
        this.movement_speed = movement_speed;
        this.duration = duration;
        this.override_move_direction = false;
        this.look_direction = MoveActionLookDirection.look_direction;
        this.crouch = false;
    }

    public override string ToString() {
        return $"{GetType().Name}({name})";
    }
}




