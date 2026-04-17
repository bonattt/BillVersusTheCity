using System.Collections.Generic;

public interface IEnemyGroup {
    public int total_enemy_count { get; }
    public int defeated_enemy_count { get; }
    public int remaining_enemy_count { get; }
    public void AlertAll();
    public IEnumerable<EnemyNavMeshMovement> AllEnemies();
    public IEnumerable<EnemyNavMeshMovement> DefeatedEnemies();
    public IEnumerable<EnemyNavMeshMovement> RemainingEnemies();
    public void Subscribe(IEnemyGroupSubscriber sub);
    public void Unsubscribe(IEnemyGroupSubscriber sub);
}


public interface IEnemyGroupSubscriber {
    public void EnemyDefeated(IEnemyGroup group);
}

