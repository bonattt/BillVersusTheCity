

using UnityEngine;

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

    public LayerMask has_cover_raycast; // LayerMask used when performing a raycast to determin if a position has cover or not
}