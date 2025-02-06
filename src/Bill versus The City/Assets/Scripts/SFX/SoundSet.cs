using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Sound Set", menuName ="Data/SoundSet")]
public class SoundSet : ScriptableObject, ISounds
{
    public string sound_name = "new sound set";
    public List<AdjustedSound> sounds;

    public List<ISingleSound> GetSounds() {
        List<ISingleSound> output = new List<ISingleSound>();
        foreach (AdjustedSound s in sounds) {
            output.Add((ISingleSound) s);
        }
        return output;
    }
    public ISingleSound GetSound() {
        return ((ISounds) this).GetRandomSound();
    }
}
