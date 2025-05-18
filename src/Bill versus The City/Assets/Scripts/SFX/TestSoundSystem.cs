using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSoundSystem : MonoBehaviour
{
    public AudioClip clip;
    public string sound_path;
    public float volume = 1f;

    // Start is called before the first frame update
    void Start()
    {
        if (StringIsSet(sound_path)) {
            ISFXSounds sound = SFXLibrary.LoadSound(sound_path);
            SFXSystem.inst.PlaySound(sound, transform.position);
        } else {
            SFXSystem.inst.PlaySound(clip, transform.position, volume);
        }
        Destroy(gameObject);
    }

    private bool StringIsSet(string path) {
        // returns true if the string was set to a value
        return path != null && !path.Equals("");
    }
}
