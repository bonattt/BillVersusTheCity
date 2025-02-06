using System.Collections;
using System.Collections.Generic;
using System.Linq; 

using UnityEngine;

public class InteractionSoundEffect : MonoBehaviour, IInteractionEffect
{
    public string sound_path;

    public void Interact(GameObject obj) {
        ISounds sounds = SFXLibrary.LoadSound(sound_path);
        SFXSystem.inst.PlaySound(sounds, transform.position);
    }

}
