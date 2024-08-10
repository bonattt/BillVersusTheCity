using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Sound Set", menuName ="Data/SoundSet")]
public class SoundSet : ScriptableObject, ISoundSet
{
    public string sound_name = "new sound set";
    public List<AdjustedSound> sounds;

    public List<AdjustedSound> GetSounds() {
        return new List<AdjustedSound>(sounds);
    }
    public AdjustedSound GetSound() {
        return ((ISoundSet) this).GetRandomSound();
    }
}
