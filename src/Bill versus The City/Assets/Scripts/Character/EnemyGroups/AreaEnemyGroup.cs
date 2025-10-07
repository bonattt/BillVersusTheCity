
using System.Collections.Generic;
using UnityEngine;

public class AreaEnemyGroup : AbstractEnemyGroup {
    public Vector3 size = new Vector3(2f, 2f, 2f);
    public Color gizmo_color = Color.green;
    protected override void InitializeEnemies() {
        // looks for every enemy within a collision box
        enemies_defeated = new HashSet<NavMeshAgentMovement>();
        all_enemies = new HashSet<NavMeshAgentMovement>();
        Vector3 half_extents = new Vector3(size.x / 2, size.y / 2, size.z / 2);
        foreach (Collider c in Physics.OverlapBox(transform.position, half_extents)) {
            NavMeshAgentMovement enemy = c.gameObject.GetComponent<NavMeshAgentMovement>();
            if (enemy == null) { continue; }

            all_enemies.Add(enemy);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = gizmo_color;
        Gizmos.DrawWireCube(transform.position, size);
    }
}