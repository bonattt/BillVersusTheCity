using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Sound Set", menuName ="Data/SoundSet")]
public class SoundSet : ScriptableObject, ISoundSet
{
    public string sound_name = "new sound set";
    public List<AdjustedSound> sounds;

    public List<ISound> GetSounds() {
        List<ISound> output = new List<ISound>();
        foreach (AdjustedSound s in sounds) {
            output.Add((ISound) s);
        }
        return output;
    }
    public ISound GetSound() {
        return ((ISoundSet) this).GetRandomSound();
    }
}
