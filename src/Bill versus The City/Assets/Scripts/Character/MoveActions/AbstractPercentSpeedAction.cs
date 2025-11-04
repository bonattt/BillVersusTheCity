using UnityEngine;

public abstract class AbstractPercentSpeedAction : IMoveAction {
    public string name = "new action";
    private float base_movement_speed;
    public float movement_speed {
        get {
            float percent = GetPercent();
            if (percent > 1f || percent < 0f) { Debug.LogWarning($"percent should be a value between 0 and 1, but it was {percent}"); }
            float result = (base_movement_speed * (1 - percent)) + (blended_speed * percent);
            Debug.LogWarning($"({base_movement_speed} * {1 - percent}) + ({blended_speed} * {percent}): {(base_movement_speed * (1 - percent)) + (blended_speed * percent)}");
            return result;
        }
    }
    public bool override_move_direction { get; set; }
    public bool crouch { get; set; }
    public Vector3 move_direction { get; set; }
    public MoveActionLookDirection look_direction { get; set; }
    public float duration { get; set; }

    public abstract float GetPercent();

    protected float blended_speed;
    public AbstractPercentSpeedAction(float base_movement_speed, float blended_speed, float duration) {
        this.base_movement_speed = base_movement_speed; // speed when percent is zero
        this.blended_speed = blended_speed; // speed when percent is full (one)
        this.duration = duration;
        this.override_move_direction = false;
        this.look_direction = MoveActionLookDirection.look_direction;
        this.crouch = false;
    }

    public override string ToString() {
        return $"{GetType().Name}({name})";
    }
}
