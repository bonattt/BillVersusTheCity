
using System.Collections.Generic;
using UnityEngine;

public class ListEnemyGroup : AbstractEnemyGroup {
    public List<NavMeshAgentMovement> init_enemies;
    protected override void InitializeEnemies() {
        all_enemies = new HashSet<NavMeshAgentMovement>(init_enemies);
        enemies_defeated = new HashSet<NavMeshAgentMovement>();
    }
}