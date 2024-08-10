using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoundSet : ISound {
    public List<AdjustedSound> GetSounds();

    public AdjustedSound GetRandomSound() {
        List<AdjustedSound> sounds = GetSounds();
        int i = Random.Range(0, sounds.Count);
        return sounds[i];
    }
}