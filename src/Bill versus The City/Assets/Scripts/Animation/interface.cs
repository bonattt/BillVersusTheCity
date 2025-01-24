
using UnityEngine;

public interface IAnimationFacade {
    public Vector3 move_velocity { get; set; }
    public Vector3 forward_direction { get; set; }
    public Vector3 right_direction { get; set; }
    public AnimationActionType action { get; set; }
    public WeaponClass weapon_class { get; set; }

    public float aim_percent { get; set; } // moves towards 0 when not aiming, moves toward 1 while aim is held.
    public float shot_duration { get; set; } // how long should the shooting animation play for
    public float shot_at { get; set; } // time of the last gunshot
    public float hurt_duration { get; set; } // how long the hurt animation should play for
    public float hurt_at { get; set; } // time last time the character took damage
    public bool is_killed { get; set; } // time last time the character took damage
    public float crouch_percent { get; set; } // percentage crouched
    public bool crouch_dive { get; set; } // character is performing a crouch dive
    public bool is_sprinting { get; set; }

}

public enum AnimationActionType {
    idle,
    shooting
}