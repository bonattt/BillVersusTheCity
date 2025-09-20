

using System;
using UnityEngine;
using UnityEngine.AI;

public class LayerMaskSystem : MonoBehaviour {
    // class for using the inspector to configure and access globally reusable LayerMasks.
    private static LayerMaskSystem _inst = null;
    public static LayerMaskSystem inst {
        get {
            if (_inst == null) {
                _inst = Resources.Load<GameObject>("LayerMaskSystem").GetComponent<LayerMaskSystem>();
            }
            return _inst;
        }
    }

    [Tooltip("LayerMask used when performing a raycast to determin if a position has cover or not")]
    public LayerMask has_cover_raycast;


    [Tooltip("LayerMask used when testing if a sound is audible")]
    public LayerMask blocks_sounds;

    // Navmesh area-mask used for sound propegation
    // this field is configured through a NavMeshAgent.AreaMask because that's the only way to get a robust Inspector UI for this, and the documentation to calculate this myself is not very helpful.
    private int? _nav_mesh_sound_area_mask;
    public int nav_mesh_sound_area_mask {
        get {
            if (_nav_mesh_sound_area_mask == null) {
                CaclulateNavMeshAreaMasks();
            }
            return _nav_mesh_sound_area_mask.Value;
        }
    }


    [Tooltip("This field contains references to NavMeshAgents used to configure NavMesh AreaMasks. This hack is the best way I could find to get a UI to avoid calculating these myself, without good documentation.")]
    public LayerMaskNavMeshAgents nav_mesh_agents;

    private void CaclulateNavMeshAreaMasks() {
        _nav_mesh_sound_area_mask = nav_mesh_agents.sound_propegation.areaMask;
    }

}

[Serializable]
public class LayerMaskNavMeshAgents {
    [Tooltip("navmesh agent containing the area_mask used to propegate sound.")]
    public NavMeshAgent sound_propegation;
}