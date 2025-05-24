

using UnityEngine;

public class SoundGizmo : MonoBehaviour
{
    public ISound sound;
    public Color color;
    public float size;

    void OnDrawGizmos()
    {
        Gizmos.color = this.color;
        Gizmos.DrawSphere(transform.position, size);
        Gizmos.DrawWireSphere(transform.position, sound.range);
    }
}