
using System.Collections.Generic;
using UnityEngine;

public class ListEnemyGroup : AbstractEnemyGroup {
    public List<EnemyNavMeshMovement> init_enemies;
    protected override void InitializeEnemies() {
        all_enemies = new HashSet<EnemyNavMeshMovement>(init_enemies);
        enemies_defeated = new HashSet<EnemyNavMeshMovement>();
    }
}