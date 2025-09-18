using System.Collections.Generic;

public interface IEnemyGroup {
    public int total_enemy_count { get; }
    public int defeated_enemy_count { get; }
    public int remaining_enemy_count { get; }
    public IEnumerable<NavMeshAgentMovement> AllEnemies();
    public IEnumerable<NavMeshAgentMovement> DefeatedEnemies();
    public IEnumerable<NavMeshAgentMovement> RemainingEnemies();
}