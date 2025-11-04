

public class CrouchAction : AbstractPercentSpeedAction
{
    public ManualCharacterMovement character;
    public CrouchAction(ManualCharacterMovement character, float base_movement_speed, float blended_speed, float duration) : base(base_movement_speed,blended_speed,  duration) {
        this.character = character;
    }
    public override float GetPercent() {
        return character.crouch_percent;
    }
}