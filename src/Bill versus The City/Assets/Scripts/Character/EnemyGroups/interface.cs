using System.Collections.Generic;

public interface IEnemyGroup {
    public int total_enemy_count { get; }
    public int defeated_enemy_count { get; }
    public int remaining_enemy_count { get; }
    public void AlertAll();
    public IEnumerable<NavMeshAgentMovement> AllEnemies();
    public IEnumerable<NavMeshAgentMovement> DefeatedEnemies();
    public IEnumerable<NavMeshAgentMovement> RemainingEnemies();
    public void Subscribe(IEnemyGroupSubscriber sub);
    public void Unsubscribe(IEnemyGroupSubscriber sub);
}


public interface IEnemyGroupSubscriber {
    public void EnemyDefeated(IEnemyGroup group);
}

