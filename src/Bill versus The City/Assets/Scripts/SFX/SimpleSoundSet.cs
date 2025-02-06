using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Sound Set", menuName ="Data/SimpleSoundSet")]
public class SimpleSoundSet : ScriptableObject, ISounds
{
    public string sound_name = "new sound set";
    public List<AudioClip> sounds;
    public float volume = 1f;
    public SoundCategory default_category = SoundCategory.sound_effect;

    public List<ISingleSound> GetSounds() {
        List<ISingleSound> output_sounds = new List<ISingleSound>();
        for(int i = 0; i < sounds.Count; i++) {
            AudioClip clip  = sounds[i];
            GenericSound new_sound = new GenericSound();
            new_sound.sound_name = $"sound from '{name}' {i}";
            new_sound.clip = clip;
            new_sound.volume = volume;
            new_sound.default_category = default_category;
            output_sounds.Add(new_sound);
        }
        return output_sounds;
    }
    // public GenericSound GetSound() {
    //     return ((ISoundSet) this).GetRandomSound();
    // }
}