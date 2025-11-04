

public class AimAction : AbstractPercentSpeedAction
{
    public ManualCharacterMovement character;
    public AimAction(ManualCharacterMovement character, float base_movement_speed, float blended_speed, float duration) : base(base_movement_speed, blended_speed, duration) {
        this.character = character;
    }
    public override float GetPercent() {
        return character.attack_controller.aim_percent;
    }
}