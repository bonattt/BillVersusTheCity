
using UnityEngine;

public interface IAnimationFacade {
    public Vector3 move_velocity { get; set; }
    public Vector3 forward { get; set; }
    public AnimationActionType action { get; set; }
}

public enum AnimationActionType {
    idle,
    shooting
}