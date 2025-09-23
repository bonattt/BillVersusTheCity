


public class AlertEnemyGroupEvent : AbstractInteractionGameEvent {
    public AbstractEnemyGroup target_enemy_group;
    protected override void Effect() {
        target_enemy_group.AlertAll();
    }
}