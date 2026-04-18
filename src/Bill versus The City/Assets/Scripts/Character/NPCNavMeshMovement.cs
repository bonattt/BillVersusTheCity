using UnityEngine;

 public class NPCNavMeshMovement : SimpleNavMeshAgentMovement
{
    public override bool is_civilian { get => true; }
    public override void DelayedOnDeath(ICharacterStatus status) {
        // triggers after a death animation finishes playing
        LevelConfig.inst.FailLevel(LevelFailureReason.npc_killed);
    }

    // public override void FlashBangHit(Vector3 flashbang_position, float intensity) {
    //     Debug.LogWarning($"flash bang hit: '{intensity}'");
    // }
}
