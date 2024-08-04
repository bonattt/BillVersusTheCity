using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Sound", menuName ="Data/Sound")]
public class AdjustedSound : ScriptableObject
{
    public string sound_name = "new sound";
    public AudioClip clip;

    public float volume = 1f;

    public SoundCategory default_category = SoundCategory.sound_effect;

}

public interface ISound {
    public AdjustedSound GetSound();
}