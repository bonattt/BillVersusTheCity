using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class AudioSettingsDebugger : MonoBehaviour {

    public bool write = false;

    public float master_volume = 1f;
    public float menu_volume = 1f;
    public float music_volume = 1f;
    public float sfx_volume = 1f;
    
    void Update() {
        AudioSettings settings = GameSettings.inst.audio_settings;
        if (write) {
            settings.master_volume = this.master_volume;
            settings.SetVolumeSetting(SoundCategory.menu, this.menu_volume);
            settings.SetVolumeSetting(SoundCategory.music, this.music_volume);
            settings.SetVolumeSetting(SoundCategory.sound_effect, this.sfx_volume);
        } else {
            master_volume = settings.master_volume;
            menu_volume = settings.GetVolumeSetting(SoundCategory.menu);
            music_volume = settings.GetVolumeSetting(SoundCategory.music);
            sfx_volume = settings.GetVolumeSetting(SoundCategory.sound_effect);
        }
            
    }
}