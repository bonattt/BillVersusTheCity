using UnityEngine;

public class GarbageBin
{
    /* class provides a single global GameObject used to parent any game object which is 
         1) large in quantity
         2) lacks an obvious or easy natural parent
         3) self-deleting after a short time
       EG. A particle system which has been stopped, and set to Destroy, but has been temporarily left alive so particles can wind down naturally.
       This is purely for avoiding inspector clutter. 
     */

    private static Transform _transform;
    public static Transform transform
    {
        get  {
            if (_transform == null) {
                _transform = CreateGarbageBin().transform;
            }
            return _transform;
        }
    }

    private static GameObject CreateGarbageBin() {
        /* creates a GameObject to be used as a garbage bin */
        GameObject garbage_bin = new GameObject();
        garbage_bin.name = "Garbage Bin";
        _transform = garbage_bin.transform;

        EditorComment comment = garbage_bin.AddComponent<EditorComment>();
        comment.comment = "This object is used as a parent to declutter the inspector when cleaning up self destroying objects, such as a ParticleSystem which has already had `Destroy(GameObject, float)` called on it.";

        return garbage_bin;
    }
}