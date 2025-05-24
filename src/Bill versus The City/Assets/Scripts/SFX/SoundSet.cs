using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Sound Set", menuName ="Data/SoundSet")]
public class SoundSet : ScriptableObject, ISFXSounds
{
    public string sound_name = "new sound set";
    public List<AdjustedSound> sounds;

    public List<ISingleSFXSound> GetSounds() {
        List<ISingleSFXSound> output = new List<ISingleSFXSound>();
        foreach (AdjustedSound s in sounds) {
            output.Add((ISingleSFXSound) s);
        }
        return output;
    }
    public ISingleSFXSound GetSound() {
        return ((ISFXSounds) this).GetRandomSound();
    }
}
