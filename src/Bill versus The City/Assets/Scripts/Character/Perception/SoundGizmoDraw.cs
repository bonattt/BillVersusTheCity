

using UnityEngine;

public class SoundEffectDraw : MonoBehaviour, IGlobalSoundsObserver
{
    [Tooltip("how long is a sound shown, when shown on the map")]
    public float sound_duration = 1f;
    public float size = 0.75f;
    public Color color = Color.yellow;

    public float draw_height = 0.1f;


    // class for managing sounds onto the map, for debugging
    void Start()
    {
        EnemyHearingManager.inst.Subscribe(this);
    }

    public void UpdateSound(ISound sound)
    {
        GameObject display = new GameObject();
        SoundGizmo gizmo = display.AddComponent<SoundGizmo>();
        gizmo.color = color;
        gizmo.sound = sound;
        gizmo.transform.parent = transform;
        Vector3 gizmo_pos = new Vector3(sound.origin.x, draw_height, sound.origin.z);
        gizmo.transform.position = gizmo_pos;
        Destroy(gizmo, sound_duration);
    }
    
}