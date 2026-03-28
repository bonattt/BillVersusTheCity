

using UnityEngine;

public class BarrierCover : AbstractCover
{
    public GameObject barrier_obj;

    protected override void Activate() {
        barrier_obj.SetActive(true);
    }

    protected override void Deactivate() {
        barrier_obj.SetActive(false);
    }
}